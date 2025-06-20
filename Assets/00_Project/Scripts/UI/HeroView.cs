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
        private void OnEnable()
        {
            Player.Instance.OnHeroLevelUp.AddListener(OnInitInvoke);
        }
        private void OnDisable()
        {
            Player.Instance.OnHeroLevelUp.RemoveListener(OnInitInvoke);
        }
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

        private string GetHeroNameTextFormat(CharacterDefinition defi)
        {
            string hex = ColorUtility.ToHtmlStringRGB(defi.ColorRarity);
            return $"[<color=#{hex}>{defi.Rarity}</color>] {defi.Label}"; // Format: "{Rarity} {HeroName}"
        }
        public void SetCharacterSelectedInternal(CharacterDefinition defi)
        {
            m_HeroBigIcon.sprite = defi.Icon;
            string heroName = defi.Label;
            string rarity = defi.Rarity.ToString();
            m_HeroNameText.text = GetHeroNameTextFormat(defi);
            m_HeroSkillIcon.sprite = defi.Passives[0].Icon;

            m_UniquePlatformContent.SetActive(defi.UniquePlatform != null);

            if (defi.UniquePlatform != null)
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
