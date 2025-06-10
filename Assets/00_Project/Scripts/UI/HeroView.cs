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

        [SerializeField]
        private GameObject m_UniquePlatformContent;
        private void Start()
        {
            InitInternal();
        }
        private void InitInternal()
        {
            CharacterDefinition usedCharacter = Player.Instance.UsedCharacter;

            SetCharacterSelectedInternal(usedCharacter);
            OnInitInvoke(usedCharacter);
        }
        public void SetCharacterSelectedInternal(CharacterDefinition defi)
        {
            m_HeroBigIcon.sprite = defi.Icon;
            m_HeroNameText.text = defi.name;
            m_HeroSkillIcon.sprite = defi.Passives[0].Icon;

            m_UniquePlatformContent.SetActive(defi.UniquePlatform != null);

            if (m_UniquePlatformContent != null)
            {
                m_HeroUniquePlatformIcon.sprite = defi.UniquePlatform.Icon;
            }
            
            OnCharacterSelectedInvoke(defi);
        }
        public void SetCharacterSelected(CharacterDefinition defi)
        {
            SetCharacterSelectedInternal(defi);
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
