using UnityEngine;

namespace Rush
{
    public sealed class RankReportShare : InstagramReportShare
    {
        [SerializeField] private RankReportCard m_Card;
        private PeriodStanding m_Standing;
        private string m_Board, m_Period;
        private long m_Ends;

        protected override void Awake()
        {
            base.Awake();
            OpenButton.onClick.AddListener(OpenInternal);
        }

        internal void ResetStanding()
        {
            m_Standing = null;
            OpenButton.interactable = false;
            CloseInternal();
        }

        internal void SetStanding(string board, PeriodRankResponse response)
        {
            ResetStanding();
            var own = response.own;
            if (own == null || own.rank <= 0 || string.IsNullOrEmpty(own.id) ||
                own.id != SeasonalRanks.PlayerId) return;

            m_Board = board;
            m_Period = response.label;
            m_Ends = response.ends;
            m_Standing = own;
            OpenButton.interactable = true;
        }

        private bool IsCurrent => m_Standing != null && m_Standing.id == SeasonalRanks.PlayerId &&
            (m_Ends == 0 || (SeasonalRanks.Current != null && SeasonalRanks.Current.UtcSeconds < m_Ends));

        private void Update()
        {
            if (m_Standing != null && !IsCurrent) ResetStanding();
        }

        private void OpenInternal()
        {
            if (!IsCurrent)
            {
                ResetStanding();
                return;
            }
            ShowInternal(() => m_Card.Capture(m_Board, m_Period, m_Standing));
        }
    }
}
