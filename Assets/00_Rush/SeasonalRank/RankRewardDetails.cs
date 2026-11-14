using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class RankRewardDetails : MonoBehaviour
    {
        [Serializable]
        private sealed class RewardTier
        {
            [SerializeField] internal int lastRank;
            [SerializeField] internal int daily, weekly, seasonal;
            [SerializeField] internal TMP_Text amount;
            [SerializeField] internal GameObject highlight;

            internal int AmountFor(string board) => board switch
            {
                "daily" => daily,
                "weekly" => weekly,
                "seasonal" => seasonal,
                _ => 0
            };
        }

        [Serializable]
        private sealed class Dialog
        {
            [SerializeField] internal GameObject overlay, table;
            [SerializeField] internal RectTransform sheet;
            [SerializeField] internal TMP_Text title, estimate, note;
            [SerializeField] internal Button open, close, backdrop;
            [SerializeField] internal float periodHeight, lifetimeHeight;
        }

        [SerializeField] private Dialog m_Dialog;
        [SerializeField] private RewardTier[] m_Tiers;
        private static readonly CultureInfo s_Numbers = CultureInfo.GetCultureInfo("en-US");
        private string m_Board, m_Viewer;
        private int m_Place;
        private bool m_HasStanding;

        private void Awake()
        {
            m_Dialog.open.onClick.AddListener(OpenInternal);
            m_Dialog.close.onClick.AddListener(CloseInternal);
            m_Dialog.backdrop.onClick.AddListener(CloseInternal);
        }

        private void OnDisable() => CloseInternal();

        internal void SetBoard(string board)
        {
            m_Board = board;
            m_HasStanding = false;
            m_Place = 0;
            m_Viewer = null;
            if (m_Dialog.overlay.activeSelf) RefreshInternal();
        }

        internal void SetStanding(PeriodStanding own)
        {
            m_HasStanding = true;
            m_Viewer = SeasonalRanks.PlayerId;
            m_Place = own != null && own.score > 0 ? own.rank : 0;
            if (m_Dialog.overlay.activeSelf) RefreshInternal();
        }

        private void OpenInternal()
        {
            RefreshInternal();
            m_Dialog.overlay.SetActive(true);
        }

        private void CloseInternal() => m_Dialog.overlay.SetActive(false);

        private void RefreshInternal()
        {
            bool lifetime = m_Board == "all";
            m_Dialog.sheet.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                lifetime ? m_Dialog.lifetimeHeight : m_Dialog.periodHeight);
            m_Dialog.title.text = m_Board switch
            {
                "daily" => "Daily Rewards",
                "weekly" => "Weekly Rewards",
                "seasonal" => "Seasonal Rewards",
                _ => "All Time"
            };
            m_Dialog.table.SetActive(!lifetime);
            if (lifetime)
            {
                m_Dialog.estimate.text = "Lifetime trophy standings";
                m_Dialog.note.text = "All Time never resets and has no Diamond payout.";
                return;
            }

            bool known = m_HasStanding && m_Viewer == SeasonalRanks.PlayerId;
            m_Dialog.estimate.text = known
                ? "Earn Trophies in Colossal to qualify."
                : "Your estimate appears when your rank is available.";
            int first = 1;
            foreach (var tier in m_Tiers)
            {
                int amount = tier.AmountFor(m_Board);
                tier.amount.text = amount.ToString("N0", s_Numbers);
                bool match = known && m_Place >= first && (tier.lastRank == 0 || m_Place <= tier.lastRank);
                tier.highlight.SetActive(match);
                if (match)
                    m_Dialog.estimate.text = $"Your rank #{m_Place.ToString("N0", s_Numbers)}  •  Est. {tier.amount.text} Diamonds";
                first = tier.lastRank + 1;
            }
            m_Dialog.note.text = "Your final rank at reset sets your reward. Send it to your Inbox at login, then claim it there.";
        }
    }
}
