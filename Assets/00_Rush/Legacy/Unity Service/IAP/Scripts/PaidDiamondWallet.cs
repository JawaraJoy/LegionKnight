using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegionKnight;

namespace Rush
{
    // The existing amountdiamonditem key holds local gameplay currency. The paid portion is an account balance.
    internal sealed class PaidDiamondWallet
    {
        private const string c_Item = "diamonditem";
        private const string c_LegacyPrefix = "IAPDelivered_";
        private readonly IAPAccountHistory m_History;
        private readonly SemaphoreSlim m_Gate;
        private int m_Allowance;
        private bool m_Applying;
        private string m_Bound;
        private string SpendKey => "IAPSpendPending_" + m_History.AccountId;
        internal int Visible => m_Bound != null && m_Bound == m_History.AccountId && m_History.IsCurrent ?
            checked(m_History.Balance + m_Allowance) : 0;
        internal static bool IsDiamond(ItemConfig item) => item?.BaseInfo.Id == c_Item;

        internal PaidDiamondWallet(IAPAccountHistory history, SemaphoreSlim gate)
        {
            m_History = history;
            m_Gate = gate;
        }

        internal async Task RestoreAsync(CurrenciesControl wallet, LocalSave save)
        {
            string account = m_History.AccountId;
            string migrated = "IAPDiamondMigration_" + account;
            string snapshot = migrated + "_snapshot";
            if (!save.GetDataValue<bool>(migrated))
            {
                string[] old = save.KeysStartingWith(c_LegacyPrefix).Where(k =>
                    k.Length == c_LegacyPrefix.Length + 64 && save.GetDataValue<bool>(k) &&
                    save.GetDataValue<string>(k + "_player") == account)
                    .Select(k => k.Substring(c_LegacyPrefix.Length)).ToArray();
                bool legacy = old.Length != 0 || (save.GetDataValue<string>("IAPLegacyOwner") == account &&
                    save.KeysStartingWith("IAPTransaction_").Length != 0);
                if (legacy)
                {
                    if (!save.HasData(snapshot))
                    {
                        // Freeze the legacy balance locally before attempting a remotely retriable transfer.
                        int prior = wallet.LocalDiamonds;
                        save.ExecuteTransaction(() =>
                        {
                            save.SaveData(snapshot, prior);
                            wallet.StageLocalDiamondBalance(0);
                        });
                    }
                    string installation = save.GetDataValue<string>("IAPDeliveryInstallation");
                    await m_History.ImportLegacyAsync(installation ?? account, save.GetDataValue<int>(snapshot), old);
                }
                m_History.CheckAccount(account);
                save.ExecuteTransaction(() => { save.SaveData(migrated, true); save.DeleteData(snapshot); });
            }
            await RecoverDebitAsync(save, account);
            m_Bound = account;
            wallet.ShowPaidDiamonds(this);
        }

#if UNITY_EDITOR
        internal async Task BindRankPreviewAsync(CurrenciesControl wallet, LocalSave save, string account)
        {
            await m_History.BindRankPreviewAsync(account);
            await RecoverDebitAsync(save, account);
            m_History.CheckAccount(account);
            m_Bound = account;
            wallet.ShowPaidDiamonds(this);
        }
#endif
        private async Task RecoverDebitAsync(LocalSave save, string account)
        {
            string key = SpendKey;
            string record = save.GetDataValue<string>(key);
            if (string.IsNullOrEmpty(record)) return;
            var parts = record.Split('|');
            int amount = int.Parse(parts[1], CultureInfo.InvariantCulture);
            await m_History.ChargeAsync(parts[0], amount, true);
            m_History.CheckAccount(account);
            save.ExecuteTransaction(() => save.DeleteData(key));
        }

        // Called only inside the shop/draw local transaction after the account debit has succeeded.
        internal int ApplyToDebit(int cost)
        {
            if (!m_Applying) return cost;
            int covered = Math.Min(cost, m_Allowance);
            m_Allowance -= covered;
            return cost - covered;
        }

        internal async Task SpendAsync(CurrenciesControl wallet, LocalSave save, int cost, Func<bool> valid, Action apply)
        {
            if (cost <= wallet.LocalDiamonds && !m_Applying)
            {
                if (!valid()) throw new InvalidOperationException("Offer changed.");
                save.ExecuteTransaction(apply);
                return;
            }
            string account = m_Bound;
            await m_Gate.WaitAsync();
            try
            {
                m_History.CheckAccount(account);
                await RecoverDebitAsync(save, account);
                if (!valid()) throw new InvalidOperationException("Offer changed.");
                int charge = Math.Max(0, cost - wallet.LocalDiamonds);
                if (charge == 0) { save.ExecuteTransaction(apply); return; }
                string id = Guid.NewGuid().ToString("N");
                string key = SpendKey;
                save.ExecuteTransaction(() =>
                {
                    save.SaveData(key, id + "|" + charge.ToString(CultureInfo.InvariantCulture));
                    if (!save.HasData(key)) throw new InvalidOperationException("Local save unavailable.");
                });
                try
                {
                    if (!await m_History.ChargeAsync(id, charge, false))
                        throw new InvalidOperationException("Insufficient account Diamonds.");
                    m_History.CheckAccount(account);
                    m_Applying = true;
                    m_Allowance = charge;
                    if (!valid() || wallet.LocalDiamonds < cost - charge)
                        throw new InvalidOperationException("Offer or balance changed.");
                    save.ExecuteTransaction(() =>
                    {
                        apply();
                        if (m_Allowance != 0) throw new InvalidOperationException("Incomplete Diamond debit.");
                        save.DeleteData(key);
                    });
                }
                catch
                {
                    if (m_History.IsCurrent && m_History.AccountId == account && save.HasData(key))
                        await RecoverDebitAsync(save, account);
                    throw;
                }
                finally { m_Applying = false; m_Allowance = 0; wallet.ShowPaidDiamonds(this); }
            }
            finally { m_Gate.Release(); }
        }
    }
}
