using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class PlatformView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_PlatformNameText;
        [SerializeField]
        private TextMeshProUGUI m_PlatformDescriptionText;
        [SerializeField]
        private Image m_PlatformBigIcon;
        [SerializeField]
        private UnityEvent<PlatformConfig> m_OnPlatformSelected = new();
        private void Start()
        {
            InitInternal();
        }
        private void InitInternal()
        {
            m_PlatformBigIcon.sprite = Player.Instance.PlatformDeck.GetUsedStanbyPlatform().CollectibleField.SplashImage;
            m_PlatformNameText.text = Player.Instance.PlatformDeck.GetUsedStanbyPlatform().BaseInfo.Name;
            m_PlatformDescriptionText.text = Player.Instance.PlatformDeck.GetUsedStanbyPlatform().BaseInfo.Description;
            OnPlatformSelectedInvoke(Player.Instance.PlatformDeck.GetUsedStanbyPlatform());
        }
        public void SetPlatformSelected(PlatformConfig platformConfig)
        {
            m_PlatformBigIcon.sprite = platformConfig.CollectibleField.SplashImage;
            m_PlatformNameText.text = platformConfig.BaseInfo.Name;
            m_PlatformDescriptionText.text = platformConfig.BaseInfo.Description;
            OnPlatformSelectedInvoke(platformConfig);
        }
        private void OnPlatformSelectedInvoke(PlatformConfig platformConfig)
        {
            m_OnPlatformSelected?.Invoke(platformConfig);
        }
    }
    public partial class HeroPanel
    {
        private PlatformView GetPlatformView()
        {
            return GetBinding<PlatformView>();
        }

        public void SetPlatformSelected(PlatformConfig platformConfig)
        {
            GetPlatformView().SetPlatformSelected(platformConfig);
        }
    }
    public partial class CanvasManager
    {
        public void SetPlatformSelected(PlatformConfig platformConfig)
        {
            GetPanelInternal<HeroPanel>().SetPlatformSelected(platformConfig);
        }
    }
}
