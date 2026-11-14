using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Scripting;

namespace Rush
{
    [Serializable, Preserve] public sealed class RankReward
    {
        public string id, kind, label, state;
        public int rank, score, amount;
    }

    [Serializable, Preserve] public sealed class PeriodStanding
    {
        public string id, name;
        public int score, rank;
    }

    [Serializable, Preserve] public sealed class PeriodRankResponse
    {
        public long serverTime, ends;
        public string label, cursor, after;
        public PeriodStanding[] rows;
        public PeriodStanding own;
        public RankReward[] mail;
    }

    public sealed class SeasonalRanks : MonoBehaviour
    {
        private const string c_Endpoint = "SeasonalRank";
        private const string c_Prefix = "AJ.SeasonalRank.RunBest.1.";
        private const int c_SecondsPerDay = 86400;
        private const float c_SendInterval = 8f;
        private const float c_RetryInterval = 20f;
        private const float c_MailInterval = 180f;
        private const string c_Guest = "prelogin";
        [Serializable] private sealed class DayScore { public long day; public int best, sent; }
        [Serializable] private sealed class Outbox { public List<DayScore> days = new(); }
        private readonly Dictionary<string, RankReward> m_Mail = new();
        private string m_Owner, m_AuthIdentity;
        private int m_Revision;
        private Outbox m_Outbox;
        private float m_NextSend, m_NextMail;
        private bool m_Sending, m_Loading;
        private double m_ServerOffset;
#if UNITY_EDITOR
        private bool m_EditorWalletLoaded;
#endif
        private void OnApplicationPause(bool paused)
        {
            if(!paused){m_NextMail=0;m_NextSend=0;}
        }
        internal static SeasonalRanks Current { get; private set; }
        internal static string PlayerId => UnityServices.State == ServicesInitializationState.Initialized &&
            AuthenticationService.Instance.IsSignedIn ? AuthenticationService.Instance.PlayerId : null;
        internal string Owner => m_Owner;
        internal IEnumerable<RankReward> Mail => m_Owner == PlayerId ? m_Mail.Values : Enumerable.Empty<RankReward>();
        internal long UtcSeconds => DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (long)m_ServerOffset;
        internal event Action Changed;

        private void Awake()
        {
            Current = this;
            BindAccount();
        }

        private void OnDestroy() { if (Current == this) Current = null; }
        private static string Key(string owner) => c_Prefix + Application.cloudProjectId + "." + owner;
        private static Outbox Load(string key) => JsonUtility.FromJson<Outbox>(PlayerPrefs.GetString(key, "{}")) ?? new Outbox();
        private void Persist()
        {
            PlayerPrefs.SetString(Key(m_Owner), JsonUtility.ToJson(m_Outbox));
            PlayerPrefs.Save();
        }
        private void BindAccount()
        {
            string signedIn = PlayerId;
            string owner = signedIn ?? PlayerPrefs.GetString(Key("owner"), c_Guest);
            if (owner == m_Owner && signedIn == m_AuthIdentity) return;
            m_AuthIdentity = signedIn;
            m_Owner = owner;
            m_Revision++;
            m_Outbox = Load(Key(owner));
            m_Outbox.days ??= new List<DayScore>();
            m_Mail.Clear();
            m_NextSend = m_NextMail = 0;
#if UNITY_EDITOR
            m_EditorWalletLoaded = false;
#endif
            // Adopt initial offline scores only once, before any authenticated account was used.
            if (owner != c_Guest && !PlayerPrefs.HasKey(Key("owner")))
            {
                var guest = Load(Key(c_Guest));
                foreach (var score in guest.days ?? new List<DayScore>())
                {
                    var existing = m_Outbox.days.Find(d => d.day == score.day);
                    if (existing == null) m_Outbox.days.Add(score);
                    else existing.best = Math.Max(existing.best, score.best);
                }
                Persist();
                PlayerPrefs.SetString(Key("owner"), owner);
                PlayerPrefs.DeleteKey(Key(c_Guest)); PlayerPrefs.Save();
            }
            if(PlayerId!=null && PlayerPrefs.GetString(Key("owner"))!=owner){PlayerPrefs.SetString(Key("owner"),owner);PlayerPrefs.Save();}
            Changed?.Invoke();
        }
        internal void Record(int runScore)
        {
            if (runScore <= 0) return;
            BindAccount();
            long day = UtcSeconds / c_SecondsPerDay;
            var entry = m_Outbox.days.Find(d => d.day == day);
            if (entry == null)
            {
                entry = new DayScore { day = day };
                m_Outbox.days.Add(entry);
            }
            if (runScore <= entry.best) return;
            entry.best = runScore;
            Persist();
        }
        private void Update()
        {
            BindAccount();
            if (PlayerId == null) return;
            if (!m_Sending && Time.unscaledTime >= m_NextSend) Flush();
            if (!m_Loading && Time.unscaledTime >= m_NextMail) PollMail();
        }
        private async void Flush()
        {
            m_NextSend = Time.unscaledTime + c_SendInterval;
            var d = m_Outbox.days.Find(s => s.best > s.sent);
            if (d == null) return;
            int revision = m_Revision, best = d.best;
            m_Sending = true;
            try
            {
                await Request(new() { ["op"] = "runBest", ["day"] = d.day, ["total"] = best });
                if (revision != m_Revision) return;
                d.sent = best;
                m_Outbox.days.RemoveAll(s=>s.day < UtcSeconds/c_SecondsPerDay && s.best==s.sent);
                Persist();
            }
            catch (Exception e) { m_NextSend=Time.unscaledTime+c_RetryInterval; Debug.LogWarning("[Seasonal Rank] Trophy sync: "+e.Message); }
            finally { m_Sending=false; }
        }
        internal async Task<PeriodRankResponse> Request(Dictionary<string,object> args)
        {
            string owner = PlayerId;
            if (string.IsNullOrEmpty(owner)) throw new InvalidOperationException("Sign in to view your rank.");
            var result = await CloudCodeService.Instance.CallEndpointAsync<PeriodRankResponse>(c_Endpoint,args);
            if (this == null || PlayerId != owner || result == null) throw new OperationCanceledException();
            m_ServerOffset = result.serverTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return result;
        }
        internal void RefreshMail() => m_NextMail = 0;
        private async void PollMail()
        {
            m_Loading=true;
            int revision=m_Revision;
            try
            {
                string cursor=null,after=null;
                do {
                    var args=new Dictionary<string,object>{{"op","mail"}};
                    if (!string.IsNullOrEmpty(cursor)) args["cursor"]=cursor;
                    if (!string.IsNullOrEmpty(after)) args["after"]=after;
                    var page=await Request(args);
                    if (revision!=m_Revision) return;
                    Merge(page.mail);
                    cursor=page.cursor; after=page.after;
                } while (!string.IsNullOrEmpty(cursor));
#if UNITY_EDITOR
                if(!m_EditorWalletLoaded && m_Mail.Values.Any(m=>m.state=="claimed") && LegionKnight.Player.CurrentWallet!=null)
                {
                    var iap=LegionKnight.UnityService.Instance.IAPManager;
                    if(iap!=null){await iap.RefreshRankWalletAsync();m_EditorWalletLoaded=true;}
                }
#endif
                m_NextMail=Time.unscaledTime+c_MailInterval;
            }
            catch(Exception e) { m_NextMail=Time.unscaledTime+c_RetryInterval; Debug.LogWarning("[Seasonal Rank] Inbox sync: "+e.Message); }
            finally { m_Loading=false; }
        }
        private static int Order(string state) => state=="claimed"?2:state=="inbox"?1:0;
        private void Merge(RankReward[] rows)
        {
            bool changed=false;
            foreach(var row in rows ?? Array.Empty<RankReward>())
                if (!m_Mail.TryGetValue(row.id,out var old)||Order(old.state)<Order(row.state))
                { m_Mail[row.id]=row; changed=true; }
            if(changed) Changed?.Invoke();
        }
        internal async Task Act(string op,string id)
        {
            var result=await Request(new(){{"op",op},{"id",id}});
            Merge(result.mail);
            if(op=="claim") {
                var iap=LegionKnight.UnityService.Instance.IAPManager;
                if(iap!=null)await iap.RefreshRankWalletAsync();
            }
        }
    }
}
