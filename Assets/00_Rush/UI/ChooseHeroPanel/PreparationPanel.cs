using UnityEngine;
using LegionKnight;
using UnityEngine.UI;

namespace Rush
{
    public class PreparationPanel : PanelView
    {
        [SerializeField]
        private SelectCharacterMode m_SelectCharacterMode = SelectCharacterMode.Hero;
        public SelectCharacterMode SelectCharacterMode => m_SelectCharacterMode;

        [SerializeField]
        private HeroSelectTabView m_HeroTabView;
        [SerializeField]
        private CardSelectTabView m_CardTabView;
        [SerializeField]
        private TabGroup m_TabGroup;

        [SerializeField]
        private Button m_HeroButton;
        [SerializeField]
        private Button m_CardButton;

        [SerializeField] private PreparationHeroLayout m_HeroLayout;
        public HeroSelectTabView HeroTabView => m_HeroTabView;
        public CardSelectTabView CardTabView => m_CardTabView;
        protected override void ShowInternal()
        {
            base.ShowInternal();
            m_SelectCharacterMode = SelectCharacterMode.Hero;
            m_HeroLayout?.ResetMode();
            m_TabGroup.OnTabClicked(0);
            var heroes = Player.Instance.HeroesCollection;
            if (heroes.SelectedHero == null && heroes.UsedHero != null)
                heroes.SetSelectedHero(heroes.UsedHero);
            m_CardTabView.Refresh();
            m_HeroTabView.Refresh();
            Adjust();
        }
        protected override void HideInternal()
        {
            base.HideInternal();
            SetSelectMode(SelectCharacterMode.Hero);
        }
        private void SetSelectMode(SelectCharacterMode mode)
        {
            m_SelectCharacterMode = mode;
            m_HeroLayout?.ResetMode();
            Adjust();
        }

        private void Start()
        {
            m_HeroButton.onClick.RemoveListener(OnHeroButtonClickedInternal);
            m_CardButton.onClick.RemoveListener(OnCardButtonClickedInternal);
            m_HeroButton.onClick.AddListener(OnHeroButtonClickedInternal);
            m_CardButton.onClick.AddListener(OnCardButtonClickedInternal);
        }

        private void Adjust()
        {
            if (m_SelectCharacterMode == SelectCharacterMode.Hero)
            {
                m_CardTabView.HideAllCard();
                m_HeroTabView.ShowAllCard();
            }
            else
            {
                m_HeroTabView.HideAllCard();
                m_CardTabView.ShowAllCard();
            }
        }
        public void ShowRarity(RarityConfig rarityConfig)
        {
            if (m_SelectCharacterMode == SelectCharacterMode.Hero)
                m_HeroTabView.ShowRarity(rarityConfig);
            else
                m_CardTabView.ShowRarity(rarityConfig);
        }
    

        private void OnHeroButtonClickedInternal()
        {
            SetSelectMode(SelectCharacterMode.Hero);
        }

        private void OnCardButtonClickedInternal()
        {
            SetSelectMode(SelectCharacterMode.Card);
        }

        private void OnDestroy()
        {
            if (m_HeroButton != null)
            {
                m_HeroButton.onClick.RemoveListener(OnHeroButtonClickedInternal);
            }

            if (m_CardButton != null)
            {
                m_CardButton.onClick.RemoveListener(OnCardButtonClickedInternal);
            }
        }
}
    public enum SelectCharacterMode
    {
        Hero = 0,
        Card = 1
    }
}
