using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class HeroView : MonoBehaviour
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
        private StarGroupView m_StarGroupView;
        [SerializeField]
        private HeroStatusView m_HeroStatusView;
        [SerializeField]
        private UseButton m_UseButton;
        [SerializeField]
        private UpgradeView m_UpgradeView;
        [SerializeField]
        private UpgradeButton m_UpgradeButton;
        [SerializeField]
        private BreakThroughButton m_BreakThroughButton;
        [SerializeField]
        private AvatarSpineUI m_AvatarSpineUI;

        [Header("Skills")]
        [SerializeField]
        private AbilityView m_BasicAttack;
        [SerializeField]
        private AbilityView m_Skilll;
        [SerializeField]
        private AbilityView m_Platform;
        [SerializeField]
        private AbilityView m_Passive;

        private HeroUnitConfig m_Config;
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
            HeroUnitConfig selected = Player.Instance.HeroesCollection.SelectedHero;
            if (selected == null) return;
            SetHeroSelectedInternal(selected);
            OnInitInvoke(selected);

        }
        private void InitInternal()
        {
            if (Player.Instance.HeroesCollection.SelectedHero == null) return;
            HeroUnitConfig usedHero = Player.Instance.HeroesCollection.UsedHero;
            SetHeroSelectedInternal(usedHero);
            m_StarGroupView.Init(usedHero);
            OnInitInvoke(usedHero);
        }

        private string GetHeroNameTextFormat(HeroUnitConfig config)
        {
            string hex = ColorUtility.ToHtmlStringRGB(config.CollectibleField.RarityConfig.Color);
            return $"{config.BaseInfo.Name} [<color=#{hex}>{config.CollectibleField.RarityConfig.BaseInfo.Name}</color>]"; // Format: "{Rarity} {HeroName}"
        }
        private void SetHeroSelectedInternal(HeroUnitConfig heroConfig)
        {
            m_HeroBigIcon.sprite = heroConfig.CollectibleField.Icon;
            m_HeroNameText.text = GetHeroNameTextFormat(heroConfig);
            OnCharacterSelectedInvoke(heroConfig);
        }
        public void SetHeroSelected(HeroUnitConfig heroConfig)
        {
            SetHeroSelectedInternal(heroConfig);
        }
        private void OnCharacterSelectedInvoke(HeroUnitConfig heroConfig)
        {
            m_OnHeroSelected?.Invoke(heroConfig);
        }
        private void OnInitInvoke(HeroUnitConfig config)
        {
            if (config == null) return;
            m_Config = config;
            m_OnInit?.Invoke(config);

            m_AvatarSpineUI.SetSkeletonAssetData(config.SkeletonDataAsset);
            m_AvatarSpineUI.PlayClip(config.Idle);

            m_StarGroupView.Init(config);
            m_HeroStatusView.Init(config);
            m_UseButton.Init(config);
            m_UpgradeView.Init(Player.Instance.HeroesCollection.GetHeroUnit(config));
            m_UpgradeButton.Init(config);
            m_BreakThroughButton.Init(config);
            m_BasicAttack.Init(config);
            m_Skilll.Init(config);
            m_Platform.Init(config);
            m_Passive.Init(config);

        }
    }
}
