using UnityEngine;
using UnityEngine.UI;
using Rush;

namespace LegionKnight
{
    public partial class GameOverPanel : PanelView
    {
        [SerializeField]
        private Button m_PlayAgainButton;
        [SerializeField]
        private Button m_HomeButton;
        
        [SerializeField]
        private Button m_RebornAdsButton;
        [SerializeField]
        private Button m_DoubleRewardButton;
        [SerializeField]
        private GameObject m_RebornContent;
        [SerializeField]
        private GameStateConfig m_GameStateConfig;
        [SerializeField]
        private GameStateConfig m_HomeStateConfig;

        [SerializeField]
        private LootMonitor m_LootMonitor;

        private void Awake()
        {
            m_DoubleRewardButton.onClick.AddListener(DoubleReward);
            m_RebornAdsButton.onClick.AddListener(ShowRebornAds);
            m_PlayAgainButton.onClick.AddListener(PlayAgain);
            m_HomeButton.onClick.AddListener(BackHome);
        }
        protected override void ShowInternal()
        {
            if (IsShowInternal) return;
            base.ShowInternal();
            m_LootMonitor.Show();
            RebornButtonStateCheck();
        }

        private void RebornButtonStateCheck()
        {
            bool canReborn = RushPlayer.Instance.Reborn.CanForceReborn;
            m_RebornContent.SetActive(canReborn);
            if (canReborn)
            {
                // anything else here
            }
            else
            {
                UnityService.Instance.ShowInterstitialAd();
            }
        }
        private void PlayAgain()
        {
            RushGameManager.Instance.GameStateManager.ChangeState(m_GameStateConfig);
            HideInternal();
        }
        private void BackHome()
        {
            RushGameManager.Instance.GameStateManager.ChangeState(m_HomeStateConfig);
            HideInternal();
        }
        private void ShowRebornAds()
        {
            UnityService.Instance.ShowRewardedAd(RebornAds);
        }
        private void RebornAds()
        {
            HideInternal();
            CanvasManager.Instance.GetPanel<RevivePanel>().Show();
            RushPlayer.Instance.Reborn.ForcingReborn(1f);
        }

        private void DoubleReward()
        {
            UnityService.Instance.ShowRewardedAd(DoubleRewardAction);
        }

        private void DoubleRewardAction()
        {
            LootMonitor lootMonitor = m_LootMonitor;
            if (lootMonitor != null)
            {
                Debug.Log("Doubling Reward Loots");
                //lootMonitor.DoubledCountDownLootAmount();
                GameManager.Instance.LootStorageManager.StartDoubleStoredLoots();
            }
            else
            {
                Debug.LogWarning("LootMonitor binding not found in GameOverPanel");
            }
        }
    }
}
