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

        [System.Serializable]
        private sealed class LockedTintField
        {
            [SerializeField] private Graphic m_Graphic;
            [SerializeField] private Color m_LockedColor = Color.white;
            private Color m_OriginalColor;

            public void Apply()
            {
                if (m_Graphic == null) return;
                m_OriginalColor = m_Graphic.color;
                m_Graphic.color = m_LockedColor;
            }

            public void Restore()
            {
                if (m_Graphic != null) m_Graphic.color = m_OriginalColor;
            }
        }

        [SerializeField] private LockedTintField[] m_LockedTints = System.Array.Empty<LockedTintField>();
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
            var ads = UnityService.Instance?.UnityAdsManager;
            if (ads == null || ads.LevelPlayService != null && ads.LevelPlayService.HasActiveRequest) return;
            int requestVersion = ++m_RequestVersion;
            ads.ShowRewardedAd(() => OnAdWatchedInternal(requestVersion));
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
    

        private void OnEnable()
        {
            foreach (var tint in m_LockedTints) tint?.Apply();
        }

        private void OnDisable()
        {
            InvalidateRewardRequestInternal();
            foreach (var tint in m_LockedTints) tint?.Restore();
        }
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
