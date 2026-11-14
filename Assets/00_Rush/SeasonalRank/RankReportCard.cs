using System.Globalization;
using TMPro;
using UnityEngine;

namespace Rush
{
    public sealed class RankReportCard : InstagramReportCard
    {
        [System.Serializable]
        private sealed class ReportLabels
        {
            [SerializeField] private TMP_Text m_Board, m_Period, m_Player, m_Rank, m_RankShadow, m_Trophies;

            internal void SetStanding(string board, string period, PeriodStanding standing)
            {
                m_Board.text = board + " Leaderboard";
                m_Period.text = period?.ToUpperInvariant();
                m_Player.text = string.IsNullOrWhiteSpace(standing.name) ? "Player" : standing.name;

                string rank = "#" + standing.rank.ToString("N0", s_Numbers);
                m_Rank.text = rank;
                m_RankShadow.text = rank;
                string unit = standing.score == 1 ? " Trophy" : " Trophies";
                m_Trophies.text = standing.score.ToString("N0", s_Numbers) + unit;
            }
        }

        [SerializeField] private ReportLabels m_Labels;
        private static readonly CultureInfo s_Numbers = CultureInfo.GetCultureInfo("en-US");

        internal Texture2D Capture(string board, string period, PeriodStanding standing) =>
            CaptureInternal<RankReportCard>(card => card.m_Labels.SetStanding(board, period, standing));
    }
}
