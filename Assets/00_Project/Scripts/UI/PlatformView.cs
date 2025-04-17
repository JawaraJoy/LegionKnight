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
        private Image m_PlatformBigIcon;
        [SerializeField]
        private UnityEvent<StanbyPlatform> m_OnPlatformSelected = new();
        private void Start()
        {
            InitInternal();
        }
        private void InitInternal()
        {
            m_PlatformBigIcon.sprite = Player.Instance.GetUsedStanbyPlatform().Icon;
            m_PlatformNameText.text = Player.Instance.GetUsedStanbyPlatform().Name;
            OnPlatformSelectedInvoke(Player.Instance.GetUsedStanbyPlatform());
        }
        public void SetPlatformSelected(StanbyPlatform platform)
        {
            m_PlatformBigIcon.sprite = platform.Icon;
            m_PlatformNameText.text = platform.Name;
            OnPlatformSelectedInvoke(platform);
        }
        private void OnPlatformSelectedInvoke(StanbyPlatform platform)
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

        public void SetPlatformSelected(StanbyPlatform platform)
        {
            GetPlatformView().SetPlatformSelected(platform);
        }
    }
    public partial class GameManager
    {
        public void SetPlatformSelected(StanbyPlatform platform)
        {
            GetPanel<CharacterPanel>().SetPlatformSelected(platform);
        }
    }
}
