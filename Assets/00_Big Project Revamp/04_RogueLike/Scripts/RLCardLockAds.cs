using UnityEngine;
using LegionKnight;
using UnityEngine.UI;

namespace Rush
{
    public class RLCardLockAds : UIView
    {
        [SerializeField]
        private Button m_ButtonAds;

        private void Start()
        {
            m_ButtonAds.onClick.AddListener(WatchAds);

            RushGameManager.Instance.StageManager.OnStageStart.AddListener((x) => ShowInternal());
        }
        private void WatchAds()
        {
            UnityService.Instance.ShowRewardedAd(OnAdWatched);
        }

        private void OnAdWatched()
        {
            HideInternal();
        }
    }
}
