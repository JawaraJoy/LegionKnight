using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegionKnight;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace Rush
{
    internal sealed class IAPAccountHistory
    {
        private const string c_Fence = "iap_purchase_fence_v1";
        private const string c_Balance = "iap_diamonds_v2";
        private const string c_Migration = "iap_diamond_import_v2";
        private const int c_Attempts = 8;
        private readonly HashSet<string> m_Purchased = new();
        private readonly bool m_DeviceStore;
        internal IAPAccountHistory(bool useCloudInEditor = false) => m_DeviceStore = !Application.isEditor || useCloudInEditor;
#if UNITY_EDITOR
        internal async Task BindRankPreviewAsync(string account)
        {
            m_PlayerId = account;
            await RefreshBalanceAsync();
        }
#endif
        private string m_PlayerId;
        internal int Balance { get; private set; }
        internal string AccountId => m_DeviceStore ? m_PlayerId : "editor";
        internal bool UsesCloud => m_DeviceStore;
        internal bool IsCurrent => !m_DeviceStore || (!string.IsNullOrEmpty(m_PlayerId) &&
            AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.PlayerId == m_PlayerId);
        internal bool HasPurchased(string sku) => m_Purchased.Contains(sku);
        private static string FirstKey(string sku) => "iap_first_" + sku;
        private static string Bucket(string digest) => "iap_receipts_" + digest.Substring(0, 2);

        internal async Task LoadAsync(IEnumerable<IAPBundleConfig> catalog, IAPPurchaseTracker tracker)
        {
            if (!m_DeviceStore) return;
            var auth = AuthenticationService.Instance;
            if (!auth.IsSignedIn) throw new InvalidOperationException("Waiting for player sign-in.");
            string id = auth.PlayerId;
            m_PlayerId = id;
            tracker.BindAccount(id);
            var keys = new HashSet<string>(catalog.Select(b => FirstKey(b.ProductId))) { c_Fence, c_Balance };
            var player = CloudSaveService.Instance.Data.Player;
            var rows = await player.LoadAsync(keys);
            CheckAccount(id);
            if (!rows.ContainsKey(c_Fence))
                await player.SaveAsync(new Dictionary<string, object> { [c_Fence] = Guid.NewGuid().ToString("N") });
            CheckAccount(id);
            Balance = Number(rows);
            m_Purchased.Clear();
            foreach (var b in catalog)
            {
                if (rows.ContainsKey(FirstKey(b.ProductId))) m_Purchased.Add(b.ProductId);
                else if (tracker.HasPurchased(b)) await RememberAsync(b);
            }
        }

        internal void CheckAccount(string account)
        {
            if (account != AccountId || !IsCurrent) throw new InvalidOperationException("Player account changed.");
        }

        private static int Number(Dictionary<string, Item> rows) =>
            rows.TryGetValue(c_Balance, out var value) ? value.Value.GetAs<int>() : 0;

        private static Dictionary<string, string> Strings(Dictionary<string, Item> rows, string key) =>
            rows.TryGetValue(key, out var value) ? value.Value.GetAs<Dictionary<string, string>>() : new();

        private static SaveItem Write(Dictionary<string, Item> rows, string key, object value) =>
            new(value, rows.TryGetValue(key, out var old) ? old.WriteLock : null);

        // The fence also protects creation of previously absent keys. It never holds business data.
        private async Task<T> ChangeAsync<T>(HashSet<string> keys,
            Func<Dictionary<string, Item>, Dictionary<string, SaveItem>, Task<T>> decide)
        {
            string account = AccountId;
            keys.Add(c_Fence);
            keys.Add(c_Balance);
            var player = CloudSaveService.Instance.Data.Player;
            for (int attempt = 0; attempt < c_Attempts; attempt++)
            {
                CheckAccount(account);
                var rows = await player.LoadAsync(keys);
                CheckAccount(account);
                var writes = new Dictionary<string, SaveItem>();
                T result = await decide(rows, writes);
                CheckAccount(account);
                if (writes.Count != 0)
                {
                    writes[c_Fence] = new SaveItem(Guid.NewGuid().ToString("N"), rows[c_Fence].WriteLock);
                    try { await player.SaveAsync(writes); }
                    catch (CloudSaveConflictException) when (attempt + 1 < c_Attempts) { continue; }
                    CheckAccount(account);
                }
                Balance = writes.TryGetValue(c_Balance, out var next) ? (int)next.Value : Number(rows);
                return result;
            }
            throw new InvalidOperationException("Purchase history is busy.");
        }

        internal Task RefreshBalanceAsync() => ChangeAsync(new HashSet<string>(), (rows, writes) => Task.FromResult(true));

        internal async Task RememberAsync(IAPBundleConfig bundle)
        {
            if (m_DeviceStore && !m_Purchased.Contains(bundle.ProductId))
            {
                string key = FirstKey(bundle.ProductId);
                await ChangeAsync(new HashSet<string> { key }, (rows, writes) =>
                {
                    if (!rows.ContainsKey(key)) writes[key] = Write(rows, key, "legacy");
                    return Task.FromResult(true);
                });
            }
            m_Purchased.Add(bundle.ProductId);
        }

        internal async Task<int> CreditAsync(string digest, Dictionary<string, (int units, int bonus)> rewards)
        {
            string bucket = Bucket(digest);
            var keys = new HashSet<string>(rewards.Keys.Select(FirstKey)) { bucket, c_Migration };
            int credited = await ChangeAsync(keys, (rows, writes) =>
            {
                var receipts = Strings(rows, bucket);
                receipts.TryGetValue(digest, out string prior);
                if (prior == "done" || (prior != null && prior.StartsWith("v2:", StringComparison.Ordinal)))
                    return Task.FromResult(0);
                // A v1 reservation might already be represented in the old device's imported balance.
                bool imported = rows.TryGetValue(c_Migration, out var migration) &&
                    migration.Value.GetAs<string[]>().Contains(digest);
                int amount = 0;
                foreach (var pair in rewards)
                {
                    string first = FirstKey(pair.Key);
                    bool isFirst = !rows.TryGetValue(first, out var owner) || owner.Value.GetAs<string>() == digest;
                    if (owner == null) writes[first] = Write(rows, first, digest);
                    if (!imported) amount = checked(amount + pair.Value.units + (isFirst ? pair.Value.bonus : 0));
                }
                receipts[digest] = "v2:" + amount.ToString(System.Globalization.CultureInfo.InvariantCulture);
                writes[bucket] = Write(rows, bucket, receipts);
                writes[c_Balance] = Write(rows, c_Balance, checked(Number(rows) + amount));
                return Task.FromResult(amount);
            });
            foreach (string sku in rewards.Keys) m_Purchased.Add(sku);
            return credited;
        }

        internal Task ImportLegacyAsync(string installation, int available, string[] delivered)
        {
            return ChangeAsync(new HashSet<string> { c_Migration }, async (rows, writes) =>
            {
                if (rows.ContainsKey(c_Migration)) return false;
                var pending = new List<string> { installation };
                long recovered = 0;
                // Read only old local deliveries. A simultaneous recovery invalidates our fence and retries this scan.
                var groups = delivered.GroupBy(Bucket).ToArray();
                for (int start = 0; start < groups.Length; start += 20)
                {
                    var part = groups.Skip(start).Take(20).ToArray();
                    var buckets = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string>(part.Select(g => g.Key)));
                    foreach (var group in part)
                    {
                        var receipts = Strings(buckets, group.Key);
                        foreach (string digest in group)
                        {
                            receipts.TryGetValue(digest, out string state);
                            if (state == "done") continue;
                            if (state != null && state.StartsWith("v2:", StringComparison.Ordinal))
                                recovered += int.Parse(state.Substring(3), System.Globalization.CultureInfo.InvariantCulture);
                            else pending.Add(digest);
                        }
                    }
                }
                writes[c_Migration] = Write(rows, c_Migration, pending.ToArray());
                writes[c_Balance] = Write(rows, c_Balance, checked(Number(rows) + (int)Math.Max(0L, available - recovered)));
                return true;
            });
        }

        internal Task<bool> ChargeAsync(string id, int amount, bool cancel)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            string bucket = "iap_spend_" + id.Substring(0, 2);
            return ChangeAsync(new HashSet<string> { bucket }, (rows, writes) =>
            {
                var record = rows.TryGetValue(bucket, out var item) ? item.Value.GetAs<Dictionary<string, int>>() : new();
                record.TryGetValue(id, out int prior);
                if (prior != 0 && Math.Abs(prior) != amount) throw new InvalidOperationException("Spend amount mismatch.");
                if (cancel && prior < 0 || !cancel && prior != 0) return Task.FromResult(!cancel && prior > 0);
                int balance = Number(rows);
                if (!cancel && balance < amount) return Task.FromResult(false);
                // A cancellation tombstone also closes a request whose response was lost before it reached the service.
                record[id] = cancel ? -amount : amount;
                writes[bucket] = Write(rows, bucket, record);
                writes[c_Balance] = Write(rows, c_Balance, checked(balance + (cancel ? (prior > 0 ? amount : 0) : -amount)));
                return Task.FromResult(!cancel);
            });
        }
    }
}
