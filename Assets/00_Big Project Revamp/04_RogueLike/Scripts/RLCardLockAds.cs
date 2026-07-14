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

        private void Start()
        {
            m_ButtonAds.onClick.AddListener(WatchAds);

            RushGameManager.Instance.StageManager.OnStageStart.AddListener((x) => ShowInternal());
            m_ButtonCard.interactable = false;
        }
        private void WatchAds()
        {
            OnAdWatched();
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
    }
}
