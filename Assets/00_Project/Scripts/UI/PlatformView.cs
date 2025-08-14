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
        private UnityEvent<StandbyPlatformDefinition> m_OnPlatformSelected = new();
        private void Start()
        {
            InitInternal();
        }
        private void InitInternal()
        {
            m_PlatformBigIcon.sprite = Player.Instance.GetUsedStanbyPlatform().Icon;
            m_PlatformNameText.text = Player.Instance.GetUsedStanbyPlatform().Label;
            m_PlatformDescriptionText.text = Player.Instance.GetUsedStanbyPlatform().Description;
            OnPlatformSelectedInvoke(Player.Instance.GetUsedStanbyPlatform());
        }
        public void SetPlatformSelected(StandbyPlatformDefinition platform)
        {
            m_PlatformBigIcon.sprite = platform.Icon;
            m_PlatformNameText.text = platform.Label;
            m_PlatformDescriptionText.text = platform.Description;
            OnPlatformSelectedInvoke(platform);
        }
        private void OnPlatformSelectedInvoke(StandbyPlatformDefinition platform)
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

        public void SetPlatformSelected(StandbyPlatformDefinition platform)
        {
            GetPlatformView().SetPlatformSelected(platform);
        }
    }
    public partial class GameManager
    {
        public void SetPlatformSelected(StandbyPlatformDefinition platform)
        {
            GetPanel<CharacterPanel>().SetPlatformSelected(platform);
        }
    }
}
