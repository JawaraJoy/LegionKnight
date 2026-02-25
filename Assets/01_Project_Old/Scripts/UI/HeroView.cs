using Rush;
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
        private Image m_TypeIcon;
        [SerializeField]
        private Image m_HeroBigIcon;
        [SerializeField]
        private Image m_HeroSkillIcon;
        [SerializeField]
        private Image m_HeroUniquePlatformIcon;
        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnInit = new();
        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnHeroSelected = new();
        [SerializeField]
        private GameObject m_UniquePlatformContent;
        private void Start()
        {
            InitInternal();
        }
        public void Refresh()
        {
            RefreshInternal();
        }
        private void RefreshInternal()
        {
            HeroUnitConfig selected = Player.Instance.HeroDeck.SelectedHero;
            if (selected == null) return;
            SetCharacterSelectedInternal(selected);
            OnInitInvoke(selected);

        }
        private void InitInternal()
        {
            
            if (Player.Instance.HeroDeck.SelectedHero == null) return;
            HeroUnitConfig usedHero = Player.Instance.HeroDeck.UsedHero;
            SetCharacterSelectedInternal(usedHero);
            OnInitInvoke(usedHero);
            
        }

        private string GetHeroNameTextFormat(HeroUnitConfig config)
        {
            string hex = ColorUtility.ToHtmlStringRGB(config.CollectibleField.RarityConfig.Color);
            return $"{config.BaseInfo.Name} [<color=#{hex}>{config.CollectibleField.RarityConfig.BaseInfo.Name}</color>]"; // Format: "{Rarity} {HeroName}"
        }
        public void SetCharacterSelectedInternal(HeroUnitConfig heroConfig)
        {
            m_HeroBigIcon.sprite = heroConfig.CollectibleField.Icon;
            string heroName = heroConfig.BaseInfo.Name;
            string rarity = heroConfig.CollectibleField.RarityConfig.BaseInfo.Name.ToString();
            m_HeroNameText.text = GetHeroNameTextFormat(heroConfig);
            m_HeroSkillIcon.sprite = heroConfig.Skills[0].CollectibleField.Icon;

            m_UniquePlatformContent.SetActive(heroConfig.UniquePlatforms[0] != null);

            if (heroConfig.UniquePlatforms[0] != null)
            {
                m_HeroUniquePlatformIcon.sprite = heroConfig.UniquePlatforms[0].CollectibleField.Icon;
            }
            
            OnCharacterSelectedInvoke(heroConfig);
        }
        public void SetCharacterSelected(HeroUnitConfig heroConfig)
        {
            SetCharacterSelectedInternal(heroConfig);
        }
        private void OnCharacterSelectedInvoke(HeroUnitConfig heroConfig)
        {
            m_OnHeroSelected?.Invoke(heroConfig);
        }
        private void OnInitInvoke(HeroUnitConfig config)
        {
            if (config == null) return;
            m_OnInit?.Invoke(config);
        }
    }
    public partial class HeroPanel
    {
        private HeroView GetHeroView()
        {
            return GetBinding<HeroView>();
        }

        public void SetHeroSelected(HeroUnitConfig config)
        {
            GetHeroView().SetCharacterSelected(config);
        }
    }
    public partial class CanvasManager
    {
        private HeroPanel GetHeroPanel()
        {
            return GetPanel<HeroPanel>();
        }
        public void SetHeroSelected(HeroUnitConfig defi)
        {
            GetHeroPanel().SetHeroSelected(defi);
        }
    }
}
