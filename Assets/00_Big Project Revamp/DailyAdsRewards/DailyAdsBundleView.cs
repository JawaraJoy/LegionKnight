using UnityEngine;
using LegionKnight;
using MoreMountains.Tools;
using UnityEngine.UI;
using TMPro;
namespace Rush
{
    public class DailyAdsBundleView : UIView
    {
        [SerializeField, MMReadOnly]
        private DailyAdsBundleConfig m_Config;
        [SerializeField]
        private Image m_RewardIcon;
        [SerializeField]
        private Image m_Frame;
        [SerializeField]
        private TextMeshProUGUI m_RewardNameText;
        [SerializeField]
        private TextMeshProUGUI m_ButtonText;
        [SerializeField]
        private Button m_WatchButton;
        [SerializeField]
        private string m_WatchButtonLabel = "Watch Ad";
        [SerializeField]
        private string m_GrantedButtonLabel = "Granted!";

        public Image RewardIcon => m_RewardIcon;
        public TextMeshProUGUI RewardNameText => m_RewardNameText;
        public Button WatchButton => m_WatchButton;
        public string WatchButtonText => m_WatchButtonLabel;
        public string GrantedButtonText => m_GrantedButtonLabel;

        private ShopResultPanel m_ResultPanel;
        private ShopResultPanel ResultPanel
        {
            get
            {
                if (m_ResultPanel == null)
                    m_ResultPanel = CanvasManager.Instance.GetPanel<ShopResultPanel>();
                return m_ResultPanel;
            }
        }
        public void Init(DailyAdsBundleConfig config)
        {
            InitInternal(config);
        }

        private void InitInternal(DailyAdsBundleConfig config)
        {
            m_Config = config;
            m_RewardIcon.sprite = m_Config.Reward.CollectibleField.Icon;
            m_Frame.color = m_Config.Reward.CollectibleField.RarityConfig.Color;
            int rewardAmount = m_Config.RewardAmount;
            m_RewardNameText.text = $"{m_Config.Reward.BaseInfo.Name} x{rewardAmount}";
            m_WatchButton.onClick.AddListener(WatchAds);

            m_WatchButton.interactable = false;
            m_ButtonText.text = GrantedButtonText;
        }

        private void WatchAds()
        {
            UnityService.Instance.ShowRewardedAd(GrantReward);
        }

        private void GrantReward()
        {
            m_Config.GrantReward();
            ResultPanel.Show(BuildResultData());
            
        }

        private CollectibleResultData BuildResultData()
        {
            var data = new CollectibleResultData();
            data.AddEntry(m_Config.Reward, m_Config.RewardAmount);
            return data;
        }
    }
}
