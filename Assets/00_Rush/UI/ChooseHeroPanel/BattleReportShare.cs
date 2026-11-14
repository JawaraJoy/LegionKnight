using System;
using System.Collections.Generic;
using LegionKnight;
using TMPro;
using Unity.Services.CloudCode;
using UnityEngine;
using UnityEngine.Scripting;

namespace Rush
{
    public sealed class BattleReportShare : InstagramReportShare
    {
        private const string c_Endpoint = "BattleReportDailyBonus";
        private const long c_DayMilliseconds = 86400000;
        private const float c_RetrySeconds = 15f;
        private const string c_Offer = "Daily bonus: +10 Diamonds";
        private const string c_Claimed = "Bonus claimed today";
        private const string c_Failure = "Couldn't open Instagram. No Diamonds awarded.";

        [Serializable] private sealed class BonusUI
        {
            [SerializeField] internal TMP_Text button, message;
        }

        [Serializable, Preserve] private sealed class BonusReply
        {
            [Preserve] public bool claimed, expired;
            [Preserve] public int granted;
            [Preserve] public long serverTime, resetAt;
        }

        [SerializeField] private BattleReportCard m_Card;
        [SerializeField] private BonusUI m_BonusUI;
        private string m_Account, m_Notice;
        private bool m_Busy, m_Claimed, m_StatusKnown;
        private double m_NextCheck, m_Reset;
        protected override string RewardAccount => SeasonalRanks.PlayerId;

        public void Show(UnitConfig hero, StageMode mode, int wave, int score)
        {
            m_Notice = null;
            ShowInternal(() => m_Card.Capture(hero, mode, wave, score));
            PaintInternal();
            m_NextCheck = 0;
        }

        private void OnEnable()
        {
            m_StatusKnown = false;
            m_Notice = null;
            m_NextCheck = 0;
            PaintInternal();
        }

        private void Update()
        {
            string account = RewardAccount;
            if (account != m_Account)
            {
                m_Account = account;
                m_StatusKnown = m_Claimed = false;
                m_Notice = null;
                m_NextCheck = 0;
                PaintInternal();
            }
            if (account == null || m_Busy) return;
            double now = Time.realtimeSinceStartupAsDouble;
            if (m_StatusKnown && now >= m_Reset)
            {
                m_StatusKnown = m_Claimed = false;
                m_Notice = null;
                m_NextCheck = 0;
                PaintInternal();
            }
            if (now >= m_NextCheck) SyncInternal(account);
        }

        protected override void HandleLaunch(string account)
        {
            if (string.IsNullOrEmpty(account) || account != RewardAccount) return;
            if (m_Account != account)
            {
                m_Account = account;
                m_StatusKnown = m_Claimed = false;
            }
            m_Notice = "Instagram opened. Checking daily bonus…";
            PaintInternal();
            m_NextCheck = 0;
            if (!m_Busy) SyncInternal(account);
        }

        protected override void HandleFailure(string outcome)
        {
            m_Notice = outcome switch
            {
                "cancelled" => "Sharing cancelled. No Diamonds awarded.",
                "saved" => "Image saved only. No Diamonds awarded.",
                _ => c_Failure
            };
            PaintInternal();
            if (Application.isPlaying && CanvasManager.Instance != null)
                CanvasManager.Instance.GetPanel<TextPopUpPanel>()?.ShowText(m_Notice);
        }

        private void PaintInternal()
        {
            bool signedIn = !Application.isPlaying || !string.IsNullOrEmpty(RewardAccount);
            m_BonusUI.button.text = !signedIn ? "Sign in for +10 Diamonds"
                : m_StatusKnown && m_Claimed ? c_Claimed : c_Offer;
            m_BonusUI.message.text = m_Notice ?? (m_StatusKnown && m_Claimed
                ? c_Claimed : signedIn ? "Open Instagram from Story or Post for 10 Diamonds once a day."
                : "Sign in to earn the daily share bonus.");
        }

        private async void SyncInternal(string account)
        {
            m_Busy = true;
            m_NextCheck = double.PositiveInfinity;
            long stamp = 0;
            try
            {
                stamp = PendingInternal(account);
                var args = new Dictionary<string, object> { ["action"] = "status" };
                var reply = await CloudCodeService.Instance.CallEndpointAsync<BonusReply>(c_Endpoint, args);
                if (this == null || account != RewardAccount) return;
                if (stamp > 0 && reply != null)
                {
                    // Date the launch with server UTC, using elapsed uptime rather than the device's clock.
                    long elapsed = PendingAgeInternal(account, stamp);
                    if (elapsed >= 0)
                    {
                        args["action"] = "claim";
                        args["launchDay"] = (reply.serverTime * 1000L - elapsed) / c_DayMilliseconds;
                        reply = await CloudCodeService.Instance.CallEndpointAsync<BonusReply>(c_Endpoint, args);
                    }
                    else
                    {
                        AcknowledgeInternal(account, stamp);
                        m_Notice = "Share again to check today's bonus.";
                        stamp = 0;
                    }
                }
                if (this == null || account != RewardAccount) return;
                if (reply == null || reply.resetAt <= reply.serverTime) throw new InvalidOperationException("Invalid bonus response.");
                m_StatusKnown = true;
                m_Claimed = reply.claimed;
                m_Reset = Time.realtimeSinceStartupAsDouble + (reply.resetAt - reply.serverTime);
                if (stamp > 0)
                {
                    if (reply.claimed)
                    {
                        var wallet = UnityService.Instance.IAPManager;
                        if (wallet == null || Player.CurrentWallet == null) throw new InvalidOperationException("Wallet not ready.");
                        await wallet.RefreshRankWalletAsync();
                        if (this == null || account != RewardAccount) return;
                    }
                    AcknowledgeInternal(account, stamp);
                    m_Notice = reply.granted > 0 ? "+" + reply.granted + " Diamonds added"
                        : reply.expired ? "Daily bonus expired. Share again for today's bonus." : c_Claimed;
                }
                // A launch may have arrived while the read-only status request was in flight.
                m_NextCheck = PendingInternal(account) > 0 ? 0 : m_Reset;
                PaintInternal();
            }
            catch (Exception error)
            {
                if (this != null && account == RewardAccount)
                {
                    if (stamp > 0) m_Notice = "Instagram opened. Reconnecting to check your bonus…";
                    m_NextCheck = Time.realtimeSinceStartupAsDouble + c_RetrySeconds;
                    PaintInternal();
                    Debug.LogWarning("[Battle Report bonus] " + error.Message);
                }
            }
            finally { m_Busy = false; }
        }

        private long PendingInternal(string account)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var host = player.GetStatic<AndroidJavaObject>("currentActivity");
            using var bridge = new AndroidJavaClass("com.aetherjump.share.BattleReportInstagram");
            return bridge.CallStatic<long>("pendingLaunch", host, account);
#else
            return 0;
#endif
        }

        private long PendingAgeInternal(string account, long stamp)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var host = player.GetStatic<AndroidJavaObject>("currentActivity");
            using var bridge = new AndroidJavaClass("com.aetherjump.share.BattleReportInstagram");
            return bridge.CallStatic<long>("pendingAge", host, account, stamp);
#else
            return -1;
#endif
        }

        private void AcknowledgeInternal(string account, long stamp)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            using var host = player.GetStatic<AndroidJavaObject>("currentActivity");
            using var bridge = new AndroidJavaClass("com.aetherjump.share.BattleReportInstagram");
            bridge.CallStatic("acknowledgeLaunch", host, account, stamp);
#endif
        }
    }
}
