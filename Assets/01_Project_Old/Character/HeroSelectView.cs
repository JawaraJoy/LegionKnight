using MoreMountains.Tools;
using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class HeroSelectView : UIView
    {
        [SerializeField]
        private HeroUnitConfig m_HeroConfig;
        [SerializeField]
        private Image m_UnitIcon;
        [SerializeField]
        private GameObject m_LockIcon;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private UnityEvent<HeroUnitConfig> m_OnCharacterSelected = new();
        public HeroUnitConfig HeroConfig => m_HeroConfig;

        [SerializeField]
        private Image m_Frame;
        [SerializeField]
        private TextMeshProUGUI m_LevelText;

        [SerializeField]
        private StarGroupView m_StarGroupView;
        [SerializeField, MMReadOnly]
        private HeroView m_HeroView;
        private void SelectCharacterInternal()
        {
            Player.Instance.HeroesCollection.SetSelectedHero(m_HeroConfig);
            OnCharacterSelectedInvoke();
        }
        public void Init(HeroView heroView)
        {
            InitInternal(heroView);
        }
        private void InitInternal(HeroView heroView)
        {
            HeroUnit character = Player.Instance.HeroesCollection.GetHeroUnit(m_HeroConfig);
            InitInternal(character, heroView);
            
        }
        private bool m_ButtonalreadyListen = false;
        private void InitInternal(HeroUnit unit, HeroView heroView)
        {
            m_HeroConfig = unit.HeroConfig;
            m_HeroView = heroView;
            m_LockIcon.SetActive(!unit.Owned);
            m_SelectButton.interactable = unit.Owned;
            m_UnitIcon.sprite = unit.HeroConfig.CollectibleField.Icon;
            if (!m_ButtonalreadyListen)
            {
                m_SelectButton.onClick.AddListener(SelectCharacterInternal);
            }
            //m_SelectButton.onClick?.RemoveListener(SelectCharacterInternal);
            

            m_StarGroupView.Init(m_HeroConfig);
            m_Frame.color = unit.HeroConfig.CollectibleField.RarityConfig.Color;
            m_LevelText.text = $"Lv {unit.Level}";
            m_ButtonalreadyListen = true;
        }
        public void Init(HeroUnit unit, HeroView heroView)
        {
            InitInternal(unit, heroView);
        }

        private void OnCharacterSelectedInvoke()
        {
            m_OnCharacterSelected?.Invoke(m_HeroConfig);
            m_HeroView.Refresh();
        }
    }
}
