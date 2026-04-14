using UnityEngine;
using LegionKnight;
using UnityEngine.UI;
using UnityEditor;

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

        public HeroSelectTabView HeroTabView => m_HeroTabView;
        public CardSelectTabView CardTabView => m_CardTabView;
        protected override void ShowInternal()
        {
            base.ShowInternal();
            m_TabGroup.OnTabClicked(0);
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
            Adjust();
        }

        private void Start()
        {
            m_HeroButton.onClick.RemoveAllListeners();
            m_CardButton.onClick.RemoveAllListeners();
            m_HeroButton.onClick.AddListener(() => SetSelectMode(SelectCharacterMode.Hero));
            m_CardButton.onClick.AddListener(() => SetSelectMode(SelectCharacterMode.Card));
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
            m_HeroTabView.ShowRarity(rarityConfig);
            m_CardTabView.ShowRarity(rarityConfig);
            Adjust();
        }
    }
    public enum SelectCharacterMode
    {
        Hero = 0,
        Card = 1
    }
}
