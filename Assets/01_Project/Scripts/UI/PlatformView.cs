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
            m_PlatformBigIcon.sprite = Player.Instance.GetUsedStanbyPlatform().BigIcon;
            m_PlatformNameText.text = Player.Instance.GetUsedStanbyPlatform().BaseInfo.Name;
            m_PlatformDescriptionText.text = Player.Instance.GetUsedStanbyPlatform().BaseInfo.Description;
            OnPlatformSelectedInvoke(Player.Instance.GetUsedStanbyPlatform());
        }
        public void SetPlatformSelected(PlatformConfig platform)
        {
            m_PlatformBigIcon.sprite = platform.BigIcon;
            m_PlatformNameText.text = platform.BaseInfo.Name;
            m_PlatformDescriptionText.text = platform.BaseInfo.Description;
            OnPlatformSelectedInvoke(platform);
        }
        private void OnPlatformSelectedInvoke(PlatformConfig platform)
        {
            m_OnPlatformSelected?.Invoke(platform);
        }
    }
    public partial class CharacterPanel
    {
        private PlatformView GetPlatformView()
        {
            return GetBinding<PlatformView>();
        }

        public void SetPlatformSelected(PlatformConfig platform)
        {
            GetPlatformView().SetPlatformSelected(platform);
        }
    }
    public partial class CanvasManager
    {
        public void SetPlatformSelected(PlatformConfig platform)
        {
            GetPanelInternal<CharacterPanel>().SetPlatformSelected(platform);
        }
    }
}
