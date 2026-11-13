using UnityEngine;
using LegionKnight;
using UnityEngine.UI;

namespace Rush
{
    public class RLCardLockAds : UIView
    {
        [SerializeField]
        private Button m_ButtonAds;
        [SerializeField]
        private Button m_ButtonCard;

        private StageManager m_StageManager;
        private int m_RequestVersion;

private void Start()
        {
            m_ButtonAds.onClick.AddListener(WatchAds);
            m_StageManager = RushGameManager.Instance.StageManager;
            m_StageManager.OnStageStart.AddListener(HandleStageStart);
            m_ButtonCard.interactable = false;
        }
        private void WatchAds()
        {
            if (!isActiveAndEnabled || !IsShown || m_ButtonCard.interactable) return;
            int requestVersion = ++m_RequestVersion;
            UnityService.Instance.ShowRewardedAd(() => OnAdWatchedInternal(requestVersion));
        }

        private void OnAdWatchedInternal(int requestVersion)
        {
            if (this == null || !isActiveAndEnabled || requestVersion != m_RequestVersion || !IsShown) return;
            InvalidateRewardRequestInternal();
            HideInternal();
            m_ButtonCard.interactable = true;
        }

        protected override void ShowInternal()
        {
            InvalidateRewardRequestInternal();
            base.ShowInternal();
            m_ButtonCard.interactable = false;
        }
    

        private void OnDisable() => InvalidateRewardRequestInternal();
        private void InvalidateRewardRequestInternal() => m_RequestVersion++;

private void OnDestroy()
        {
            InvalidateRewardRequestInternal();
            if (m_ButtonAds != null)
            {
                m_ButtonAds.onClick.RemoveListener(WatchAds);
            }

            if (m_StageManager != null)
            {
                m_StageManager.OnStageStart.RemoveListener(HandleStageStart);
            }
        }


private void HandleStageStart(StageConfig stageConfig)
        {
            ShowInternal();
        }
}
}
