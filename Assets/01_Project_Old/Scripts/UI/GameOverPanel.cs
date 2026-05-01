using UnityEngine;
using UnityEngine.UI;
using Rush;
using TMPro;

namespace LegionKnight
{
    public partial class GameOverPanel : PanelView, IUpdater
    {
        [SerializeField]
        private Button m_PlayAgainButton;
        [SerializeField]
        private Button m_HomeButton;
        [SerializeField]
        private float m_CountDownDuration = 5f;
        [SerializeField]
        private TextMeshProUGUI m_TimerCountDown;
        [SerializeField]
        private Button m_RebornAdsButton;
        [SerializeField]
        private GameObject m_RebornContent;
        [SerializeField]
        private GameStateConfig m_GameStateConfig;
        [SerializeField]
        private GameStateConfig m_HomeStateConfig;

        [SerializeField]
        private LootMonitor m_LootMonitor;
        [SerializeField]
        private PreviousEnergyCost m_PreviousEnergyCost;

        private float m_CurrentCountDownTime;
        public bool IsActive => IsShowInternal;

        private void Awake()
        {
            m_RebornAdsButton.onClick.AddListener(ShowRebornAds);
            m_PlayAgainButton.onClick.AddListener(TryPlayAgain);
            m_HomeButton.onClick.AddListener(BackHome);
        }
        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        private void OnDisable()
        {
            //UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }
        private void ResetTimerInternal()
        {
            m_CurrentCountDownTime = m_CountDownDuration;

            if (m_TimerCountDown != null)
            {
                m_TimerCountDown.text = Mathf.CeilToInt(m_CurrentCountDownTime).ToString();
            }

            if (m_RebornAdsButton != null)
            {
                m_RebornAdsButton.interactable = true;
            }
        }
        protected override void ShowInternal()
        {
            if (IsShowInternal) return;
            base.ShowInternal();

            m_LootMonitor.Show();

            ResetTimerInternal(); // <-- tambahin ini
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
        private void TryPlayAgain()
        {
            m_PreviousEnergyCost.TryPay();
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

        public void Tick()
        {
            if (!IsShowInternal)
            {
                return;
            }

            if (m_CurrentCountDownTime <= 0f)
            {
                return;
            }

            m_CurrentCountDownTime -= Time.deltaTime;

            if (m_CurrentCountDownTime < 0f)
            {
                m_CurrentCountDownTime = 0f;
            }

            // update UI text
            if (m_TimerCountDown != null)
            {
                m_TimerCountDown.text = Mathf.CeilToInt(m_CurrentCountDownTime).ToString();
            }

            // selesai countdown
            if (m_CurrentCountDownTime <= 0f)
            {
                if (m_RebornAdsButton != null)
                {
                    m_RebornAdsButton.interactable = false;
                }

                // optional: ubah text
                if (m_TimerCountDown != null)
                {
                    m_TimerCountDown.text = "0";
                }
            }
        }
    }
}
