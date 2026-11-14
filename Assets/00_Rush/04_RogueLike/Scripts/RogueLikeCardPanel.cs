using LegionKnight;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class RogueLikeCardPanel : PanelView
    {
        [SerializeField] private RLCardView[] m_CardViews;
        [SerializeField] private RectTransform m_Sheet;
        [SerializeField] private ScrollRect m_CardScroll;
        [SerializeField] private Button m_RerollButton;
        [SerializeField] private TMP_Text m_RerollLabel;
        private bool m_RerollUsed;
        private RogueLikeManager m_Handler;
        private bool m_IsSelecting;
        private int m_SelectionVersion;
        private StageManager m_StageManager;
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
            m_StageManager = RushGameManager.Instance.StageManager;
            if (m_StageManager != null) m_StageManager.OnStageStart.AddListener(OnStageStartedInternal);
        }
        private void OnDisable() => InvalidateRewardRequestInternal();
        private void OnDestroy()
        {
            InvalidateRewardRequestInternal();
            if (m_Handler != null) m_Handler.OnPlayerLevelChanged.RemoveListener(OnPlayerLevelChanged);
            if (m_StageManager != null) m_StageManager.OnStageStart.RemoveListener(OnStageStartedInternal);
        }
        private void OnStageStartedInternal(StageConfig stage) => InvalidateRewardRequestInternal();
        private void InvalidateRewardRequestInternal() => m_SelectionVersion++;
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
        private void OnRectTransformDimensionsChange() => FitSheetInternal();

        private void FitSheetInternal()
        {
            if (m_Sheet == null) return;
            Rect space = ((RectTransform)m_Sheet.parent).rect;
            if (space.width <= 0 || space.height <= 0) return;
            float scale = Mathf.Min(space.width / 480f, space.height / 580f);
            m_Sheet.localScale = Vector3.one * scale;
            m_Sheet.sizeDelta = new Vector2(454f, Mathf.Min(851f, space.height / scale - 28f));
        }

        private void SetRerollUsedInternal(bool used)
        {
            m_RerollUsed = used;
            if (m_RerollButton != null) m_RerollButton.interactable = !used;
            if (m_RerollLabel != null) m_RerollLabel.text = used ? "Reroll used" : "Watch Ad to Reroll";
        }

        protected override void ShowInternal()
        {
            if (IsShown || Handler.PendingCardChoices <= 0) return;
            if (!RushPlayer.Instance.Unit.HasBind(out Damageable damageable) || !damageable.IsAlive) return;
            base.ShowInternal();
            if (IsShown)
            {
                FitSheetInternal();
                SetRerollUsedInternal(false);
                RefreshCardsInternal();
            }
        }
        protected override void HideInternal()
        {
            InvalidateRewardRequestInternal();
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
            if (!isActiveAndEnabled || !IsShown || Handler.PendingCardChoices <= 0 || m_IsSelecting || m_RerollUsed) return;
            var ads = UnityService.Instance?.UnityAdsManager;
            if (ads == null || ads.LevelPlayService != null && ads.LevelPlayService.HasActiveRequest) return;
            int selectionVersion = ++m_SelectionVersion;
            ads.ShowRewardedAd(() => ApplyRewardedRerollInternal(selectionVersion));
        }
        private void ApplyRewardedRerollInternal(int selectionVersion)
        {
            if (this == null || !isActiveAndEnabled || selectionVersion != m_SelectionVersion ||
                !IsShown || m_RerollUsed || Handler.PendingCardChoices <= 0) return;
            SetRerollUsedInternal(true);
            RefreshCardsInternal();
        }
        public void RefreshCards() => RefreshCardsInternal();
        private void RefreshCardsInternal()
        {
            if (!IsShown || Handler.PendingCardChoices <= 0) return;
            InvalidateRewardRequestInternal();
            CardConfig[] configs = Handler.GetRandomAvailableCards(m_CardViews.Length).ToArray();
            ShowCards(configs);
            if (m_CardScroll != null)
            {
                m_CardScroll.StopMovement();
                m_CardScroll.content.anchoredPosition = Vector2.zero;
            }
            if (configs.Length == 0)
            {
                Debug.LogWarning("Roguelike has no cards available; skipping pending choices.");
                while (Handler.PendingCardChoices > 0) Handler.CompleteCardChoice();
                HideInternal();
            }
        }
    }
}
