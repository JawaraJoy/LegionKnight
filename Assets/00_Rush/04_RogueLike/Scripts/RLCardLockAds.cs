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

private void Start()
        {
            m_ButtonAds.onClick.AddListener(WatchAds);
            m_StageManager = RushGameManager.Instance.StageManager;
            m_StageManager.OnStageStart.AddListener(HandleStageStart);
            m_ButtonCard.interactable = false;
        }
        private void WatchAds()
        {
            UnityService.Instance.ShowRewardedAd(OnAdWatched);
        }

        private void OnAdWatched()
        {
            HideInternal();
            m_ButtonCard.interactable = true;
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            m_ButtonCard.interactable = false;
        }
    

private void OnDestroy()
        {
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
