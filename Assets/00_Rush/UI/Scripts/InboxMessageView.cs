using System;
using LegionKnight;
using LegionKnight.Prototype;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Rush
{
    public abstract class InboxMessageView : PanelView
    {
        [Serializable]
        private sealed class LayoutField
        {
            [SerializeField] private LootItemView m_CellTemplate;
            [SerializeField] private TMP_Text m_Heading;
            [SerializeField] private TMP_Text m_ClaimLabel;
            [SerializeField] private ScrollRect m_RewardsScroll;
            [SerializeField] private ScrollRect m_LetterScroll;
            [SerializeField] private CanvasGroup m_InboxSurface;

            public LootItemView CellTemplate => m_CellTemplate;
            public TMP_Text Heading => m_Heading;
            public TMP_Text ClaimLabel => m_ClaimLabel;
            public ScrollRect RewardsScroll => m_RewardsScroll;
            public ScrollRect LetterScroll => m_LetterScroll;
            public CanvasGroup InboxSurface => m_InboxSurface;
        }

        private const string c_HeadingFormat = "REWARDS ({0})";
        private const string c_NoRewards = "No rewards";
        private const string c_Claim = "Claim";
        private const string c_Claimed = "Claimed";
        [SerializeField] private AssetReferenceGameObject m_MailItemViewAsset;
        [SerializeField] private MailDefinition m_Definition;
        [SerializeField] private TextMeshProUGUI m_LabelText;
        [SerializeField] private TextMeshProUGUI m_RewardStateText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private Transform m_RewardsViewContainer;
        [SerializeField] private Button m_ClaimButton;
        [SerializeField] private Button m_DeleteButton;
        [SerializeField] private LayoutField m_Layout = new();
        private readonly InboxRewardItems m_Items = new();

        protected void Awake()
        {
            m_ClaimButton.onClick.AddListener(ClaimInternal);
            m_DeleteButton.onClick.AddListener(DeleteInternal);
        }

        public void ReadMail(MailDefinition definition)
        {
            m_Definition = definition;
            ShowInternal();
            RefreshInternal();
            if (m_Layout.CellTemplate != null)
            {
                Canvas.ForceUpdateCanvases();
                ScrollToStartInternal(m_Layout.LetterScroll);
                ScrollToStartInternal(m_Layout.RewardsScroll);
            }
        }

        private static void ScrollToStartInternal(ScrollRect scroll)
        {
            scroll.StopMovement();
            // These lists are top-anchored, even while the popup scales in.
            RectTransform content = scroll.content;
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0f);
        }

        public void Refresh()
        {
            RefreshInternal();
        }

        private void RefreshInternal()
        {
            if (m_Definition == null) return;

            var rewards = m_Definition.Rewards ?? Array.Empty<LootField>();
            bool hasRewards = rewards.Length > 0;
            bool claimed = m_Definition.HasClaim();
            m_LabelText.text = m_Definition.Label;
            m_DescriptionText.text = m_Definition.Description;
            m_RewardStateText.text = hasRewards ? m_Definition.StateClaimRewardText() : c_NoRewards;
            m_ClaimButton.interactable = hasRewards && !claimed;
            m_RewardsViewContainer.gameObject.SetActive(hasRewards);
            if (m_Layout.CellTemplate != null)
            {
                m_Layout.Heading.SetText(c_HeadingFormat, rewards.Length);
                m_Layout.ClaimLabel.text = claimed ? c_Claimed : c_Claim;
            }
            m_Items.SetItems(rewards, m_RewardsViewContainer, m_Layout.CellTemplate, m_MailItemViewAsset);
        }

        private void ClaimInternal()
        {
            m_Definition.ClaimReward();
        }

        private void DeleteInternal()
        {
            m_Definition.DeleteMail();
            HideInternal();
        }

        protected override void OnShowInvoke()
        {
            SetInboxVisibleInternal(false);
            base.OnShowInvoke();
        }

        protected override void OnHideInvoke()
        {
            SetInboxVisibleInternal(true);
            base.OnHideInvoke();
            CanvasManager.Instance.GetPanel<HomePanel>().GetBinding<CommonUIView>().Hide();
        }

        private void SetInboxVisibleInternal(bool visible)
        {
            var surface = m_Layout.InboxSurface;
            if (surface == null) return;
            surface.alpha = visible ? 1f : 0f;
            surface.interactable = visible;
            surface.blocksRaycasts = visible;
        }
    }
}
