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
        private string m_GrantedButtonLabel = "Claimed";

        private int m_Index;
        private bool m_IsClaimed;

        public Image RewardIcon => m_RewardIcon;
        public TextMeshProUGUI RewardNameText => m_RewardNameText;
        public Button WatchButton => m_WatchButton;

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

        public void Init(DailyAdsBundleConfig config, int index)
        {
            InitInternal(config, index);
        }

        private void InitInternal(DailyAdsBundleConfig config, int index)
        {
            m_Config = config;
            m_Index = index;

            m_RewardIcon.sprite = m_Config.CollectibleField.Icon;
            m_Frame.color = m_Config.CollectibleField.RarityConfig.Color;

            int rewardAmount = m_Config.RewardAmount;

            m_RewardNameText.text =
                $"{m_Config.Reward.BaseInfo.Name} x{rewardAmount}";

            m_WatchButton.onClick.RemoveListener(WatchAds);
            m_WatchButton.onClick.AddListener(WatchAds);

            RefreshState();
        }
        private void RefreshState()
        {
            m_IsClaimed =
                RushPlayer.Instance.DailyAdsBundleManager.IsBundleClaimed(m_Index);

            if (m_IsClaimed)
            {
                m_WatchButton.interactable = false;
                m_ButtonText.text = m_GrantedButtonLabel;
            }
            else
            {
                m_WatchButton.interactable = true;
                m_ButtonText.text = m_WatchButtonLabel;
            }
        }

        private void WatchAds()
        {
            var manager = RushPlayer.Instance.DailyAdsBundleManager;
            manager.Init();
            if (manager.IsBundleClaimed(m_Index)) return;
            var bundles = manager.SelectedBundles;
            if (m_Index < 0 || m_Index >= bundles.Length || bundles[m_Index] != m_Config) return;
            int index = m_Index;
            var date = DateTime.Now.Date;
            string source = m_Config.BaseInfo.Id;
            var result = BuildResultData();
            bool completed = false;
            UnityService.Instance.ShowRewardedAd(() =>
            {
                if (completed) return;
                manager.GrantRewardSnapshot(date, index, source, result);
                completed = true;
                // The grant is independent of the view's current binding and lifetime.
                if (this == null) return;
                RefreshState();
                if (result.RewardsGranted) ResultPanel?.Show(result);
            });
        }

        private CollectibleResultData BuildResultData()
        {
            var data = new CollectibleResultData();

            data.AddEntry(m_Config.Reward, m_Config.RewardAmount);

            return data;
        }
    

private void OnDestroy()
        {
            if (m_WatchButton != null)
            {
                m_WatchButton.onClick.RemoveListener(WatchAds);
            }
        }
}
}
