using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class HeroView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_HeroNameText;
        [SerializeField]
        private Image m_HeroBigIcon;
        [SerializeField]
        private Image m_HeroSkillIcon;
        [SerializeField]
        private Image m_HeroUniquePlatformIcon;
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnInit = new();
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterSelected = new();
        private void Start()
        {
            InitInternal();
        }
        private void InitInternal()
        {
            m_HeroBigIcon.sprite = Player.Instance.UsedCharacter.Icon;
            m_HeroNameText.text = Player.Instance.UsedCharacter.name;

            m_HeroSkillIcon.sprite = Player.Instance.UsedCharacter.Passives[0].Icon;
            m_HeroUniquePlatformIcon.sprite = Player.Instance.GetHeroStandbyPlatform().Icon;
            OnInitInvoke(Player.Instance.UsedCharacter);
        }
        public void SetCharacterSelected(CharacterDefinition defi)
        {
            m_HeroBigIcon.sprite = defi.Icon;
            m_HeroNameText.text = defi.name; 
            m_HeroSkillIcon.sprite = defi.Passives[0].Icon;
            m_HeroUniquePlatformIcon.sprite = defi.UniquePlatform.Icon;
            OnCharacterSelectedInvoke(defi);
        }
        private void OnCharacterSelectedInvoke(CharacterDefinition defi)
        {
            m_OnCharacterSelected?.Invoke(defi);
        }
        private void OnInitInvoke(CharacterDefinition defi)
        {
            m_OnInit?.Invoke(defi);
        }
    }
    public partial class CharacterPanel
    {
        private HeroView GetHeroView()
        {
            return GetBinding<HeroView>();
        }

        public void SetCharacterSelected(CharacterDefinition defi)
        {
            GetHeroView().SetCharacterSelected(defi);
        }
    }
    public partial class GameManager
    {
        private CharacterPanel GetCharacterPanel()
        {
            return GetPanel<CharacterPanel>();
        }
        public void SetCharacterSelected(CharacterDefinition defi)
        {
            GetCharacterPanel().SetCharacterSelected(defi);
        }
    }
}
