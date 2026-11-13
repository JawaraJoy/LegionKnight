using System;
using LegionKnight;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class DailyAdsBundleView : UIView
    {
        [Serializable]
        private sealed class TintField
        {
            [SerializeField] private Graphic m_Target;
            [SerializeField] private Color m_Available = Color.white;
            [SerializeField] private Color m_Claimed = Color.white;
            [SerializeField] private Color m_Featured = Color.white;

            public void Apply(bool featured, bool claimed)
            {
                if (m_Target != null) m_Target.color = featured ? m_Featured : claimed ? m_Claimed : m_Available;
            }
        }

        [Serializable]
        private sealed class CardField
        {
            [SerializeField] private RarityConfig m_FeaturedRarity;
            [SerializeField] private Image m_CardEdge;
            [SerializeField] private Image m_RarityPip;
            [SerializeField] private TextMeshProUGUI m_RarityLabel;
            [SerializeField] private TextMeshProUGUI m_Amount;
            [SerializeField] private GameObject m_FeaturedDetails;
            [SerializeField] private GameObject m_ClaimedBadge;
            [SerializeField] private TextMeshProUGUI m_ClaimedLabel;
            [SerializeField] private TintField[] m_Tints = Array.Empty<TintField>();

            public bool HasAmount => m_Amount != null;
            public bool HasBadge => m_ClaimedBadge != null;
            public bool IsFeatured(RarityConfig rarity) => rarity != null && rarity == m_FeaturedRarity;

            public void SetReward(RarityConfig rarity, int amount)
            {
                Color edge = rarity != null ? rarity.Color : Color.white;
                if (m_CardEdge != null) m_CardEdge.color = edge;
                if (m_RarityPip != null) m_RarityPip.color = edge;
                if (m_RarityLabel != null) m_RarityLabel.text = rarity != null ? rarity.BaseInfo.Name.ToUpperInvariant() : string.Empty;
                if (m_Amount != null) m_Amount.text = $"\u00d7{amount}";
            }

            public void SetState(bool featured, bool claimed, string claimedLabel)
            {
                if (m_FeaturedDetails != null) m_FeaturedDetails.SetActive(featured);
                if (m_ClaimedLabel != null) m_ClaimedLabel.text = claimedLabel;
                if (m_ClaimedBadge != null) m_ClaimedBadge.SetActive(claimed);
                foreach (TintField tint in m_Tints) tint.Apply(featured, claimed);
            }
        }

        [SerializeField, MMReadOnly] private DailyAdsBundleConfig m_Config;
        [SerializeField] private Image m_RewardIcon;
        [SerializeField] private Image m_Frame;
        [SerializeField] private TextMeshProUGUI m_RewardNameText;
        [SerializeField] private TextMeshProUGUI m_ButtonText;
        [SerializeField] private Button m_WatchButton;
        [SerializeField] private string m_WatchButtonLabel = "Watch Ad";
        [SerializeField] private string m_GrantedButtonLabel = "Claimed";
        [SerializeField] private CardField m_Card = new();

        private int m_Index;
        private DateTime m_OfferDate;

        public Image RewardIcon => m_RewardIcon;
        public TextMeshProUGUI RewardNameText => m_RewardNameText;
        public Button WatchButton => m_WatchButton;

        private void Awake()
        {
            if (m_WatchButton != null) m_WatchButton.onClick.AddListener(WatchAdsInternal);
        }

        public void Init(DailyAdsBundleConfig config, int index) => InitInternal(config, index);

        private void InitInternal(DailyAdsBundleConfig config, int index)
        {
            m_Config = config;
            m_Index = index;
            m_OfferDate = DateTime.Today;
            CollectibleConfig reward = config.Reward;
            RarityConfig rarity = reward.CollectibleField?.RarityConfig;
            bool featured = m_Card.IsFeatured(rarity);
            bool claimed = RushPlayer.Instance.DailyAdsBundleManager.IsBundleClaimed(index);

            // The bundle icon can illustrate its quantity. The item actually granted owns the rarity.
            Sprite icon = config.CollectibleField?.Icon ?? reward.CollectibleField?.Icon;
            if (featured && icon == reward.CollectibleField?.Icon && reward.CollectibleField?.SplashImage != null)
                icon = reward.CollectibleField.SplashImage;
            m_RewardIcon.sprite = icon;
            m_RewardIcon.enabled = icon != null;
            m_Frame.color = rarity != null ? rarity.Color : Color.white;
            m_RewardNameText.text = m_Card.HasAmount ? reward.BaseInfo.Name : $"{reward.BaseInfo.Name} x{config.RewardAmount}";
            m_Card.SetReward(rarity, config.RewardAmount);
            m_Card.SetState(featured, claimed, m_GrantedButtonLabel);
            m_ButtonText.text = claimed && !m_Card.HasBadge ? m_GrantedButtonLabel : m_WatchButtonLabel;
            m_WatchButton.interactable = !claimed;
            if (m_Card.HasBadge) m_WatchButton.gameObject.SetActive(!claimed);
        }

        private void WatchAdsInternal()
        {
            var manager = RushPlayer.Instance.DailyAdsBundleManager;
            var offer = m_Config;
            int index = m_Index;
            DateTime date = m_OfferDate;

            // Init can rotate the offers and rebind this same view at midnight.
            manager.Init();
            var bundles = manager.SelectedBundles;
            if (date != DateTime.Today || index < 0 || index >= bundles.Length || bundles[index] != offer
                || offer == null || manager.IsBundleClaimed(index)) return;

            string source = offer.BaseInfo.Id;
            var result = new CollectibleResultData();
            result.AddEntry(offer.Reward, offer.RewardAmount);
            bool completed = false;
            UnityService.Instance.ShowRewardedAd(() =>
            {
                if (completed || manager == null) return;
                manager.GrantRewardSnapshot(date, index, source, result);
                completed = true;
                if (this != null && result.RewardsGranted)
                    CanvasManager.Instance.GetPanel<ShopResultPanel>()?.Show(result);
            });
        }

        private void OnDestroy()
        {
            if (m_WatchButton != null) m_WatchButton.onClick.RemoveListener(WatchAdsInternal);
        }
    }
}
