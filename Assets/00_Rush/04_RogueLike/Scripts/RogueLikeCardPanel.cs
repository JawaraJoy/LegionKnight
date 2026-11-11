using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class RogueLikeCardPanel : PanelView
    {
        [SerializeField] private RLCardView[] m_CardViews;
        private RogueLikeManager m_Handler;
        private bool m_IsSelecting;
        private RogueLikeManager Handler
        {
            get
            {
                if (m_Handler == null) m_Handler = RushGameManager.Instance.RogueLikeManager;
                return m_Handler;
            }
        }

        private void Start()
        {
            foreach (RLCardView view in m_CardViews)
                if (view != null) view.BindSelectionPanel(this);
            Handler.OnPlayerLevelChanged.AddListener(OnPlayerLevelChanged);
        }
        private void OnDestroy()
        {
            if (m_Handler != null) m_Handler.OnPlayerLevelChanged.RemoveListener(OnPlayerLevelChanged);
        }
        private void OnPlayerLevelChanged(int level)
        {
            if (level == 1) HideInternal();
        }
        private void ShowCards(CardConfig[] configs)
        {
            for (int i = 0; i < m_CardViews.Length; i++)
            {
                RLCardView view = m_CardViews[i];
                if (view == null) continue;
                view.BindSelectionPanel(this);
                if (i < configs.Length && configs[i] != null)
                    view.ShowCard(configs[i]);
                else
                    view.ClearCard();
            }
        }
        protected override void ShowInternal()
        {
            if (IsShown || Handler.PendingCardChoices <= 0) return;
            if (!RushPlayer.Instance.Unit.HasBind(out Damageable damageable) || !damageable.IsAlive) return;
            base.ShowInternal();
            if (IsShown) RefreshCardsInternal();
        }
        protected override void HideInternal()
        {
            foreach (RLCardView view in m_CardViews)
                if (view != null) view.ClearCard();
            base.HideInternal();
        }

        public void SelectCard(RLCardView view) => SelectCardInternal(view);
        private void SelectCardInternal(RLCardView view)
        {
            if (m_IsSelecting || !IsShown || view == null || !view.IsShown ||
                !view.SelectButton.interactable || view.CardConfig == null ||
                System.Array.IndexOf(m_CardViews, view) < 0 || Handler.PendingCardChoices <= 0)
                return;
            m_IsSelecting = true;
            try
            {
                view.CardConfig.Collect();
                Handler.CompleteCardChoice();
                if (Handler.PendingCardChoices > 0)
                    RefreshCardsInternal();
                else
                    HideInternal();
            }
            finally
            {
                m_IsSelecting = false;
            }
        }
        public void WatchAds()
        {
            if (IsShown) UnityService.Instance.ShowRewardedAd(RefreshCardsInternal);
        }
        public void RefreshCards() => RefreshCardsInternal();
        private void RefreshCardsInternal()
        {
            if (!IsShown || Handler.PendingCardChoices <= 0) return;
            CardConfig[] configs = Handler.GetRandomAvailableCards(m_CardViews.Length).ToArray();
            ShowCards(configs);
            if (configs.Length == 0)
            {
                Debug.LogWarning("Roguelike has no cards available; skipping pending choices.");
                while (Handler.PendingCardChoices > 0) Handler.CompleteCardChoice();
                HideInternal();
            }
        }
    }
}
