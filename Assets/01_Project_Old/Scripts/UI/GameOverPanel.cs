using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string GameOverPanelId = "GameOver";
    }
    public partial class GameOverPanel : PanelView
    {
        public override string UniqueId => PanelId.GameOverPanelId;
        [SerializeField]
        private Button m_PlayAgainButton;
        [SerializeField]
        private Button m_HomeButton;
        [SerializeField]
        private Button m_DoubleRewardButton;

        [SerializeField]
        private LootMonitor m_LootMonitor;

        private void Awake()
        {
            m_PlayAgainButton.onClick.AddListener(StoreLevelScoreInternal);
            m_HomeButton.onClick.AddListener(StoreLevelScoreInternal);
            m_DoubleRewardButton.onClick.AddListener(DoubleReward);
        }
        protected override void ShowInternal()
        {
            if (IsShowInternal) return;
            base.ShowInternal();
        }
        public void PlayAgain()
        {
            //GameManager.Instance.Play();
        }
        protected override void OnShowInvoke()
        {
            base.OnShowInvoke();
            //Player.Instance.SetPause(true);

        }
        protected override void OnHideInvoke()
        {
            base.OnHideInvoke();
            //GameTimeScale.SetTimeScale(1);
            //Player.Instance.SetPause(false);
        }

        private void StoreLevelScoreInternal()
        {
            //GameManager.Instance.StoreLevelScore();
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
                lootMonitor.DoubledCountDownLootAmount();
            }
            else
                            {
                Debug.LogWarning("LootMonitor binding not found in GameOverPanel");
            }
        }
    }
}
