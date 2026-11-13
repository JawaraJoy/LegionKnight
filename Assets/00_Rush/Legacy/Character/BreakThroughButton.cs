using Rush;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    // The component keeps its original script GUID for serialized Preparation views.
    public class BreakThroughButton : UIView
    {
        [SerializeField] private CurrencyView m_ShardAmountNeed;
        [SerializeField] private CurrencyView m_CoinAmountNeed;
        [SerializeField] private Button m_UpgradeButton;
        [SerializeField] private Button m_QuickAccessButton;
        [SerializeField] private BreakThroughView m_UpgradeView;
        private HeroUnit m_HeroUnit;

        private void OnEnable() => m_UpgradeButton.onClick.AddListener(OpenEvolve);
        private void OnDisable() => m_UpgradeButton.onClick.RemoveListener(OpenEvolve);

        public void Refresh()
        {
            var selected = Player.Instance.HeroesCollection.SelectedHero;
            if (selected != null) Init(selected);
        }

        public void Init(HeroUnitConfig heroConfig)
        {
            m_HeroUnit = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);
            m_UpgradeButton.interactable = m_HeroUnit.Owned;
            bool showCost = m_HeroUnit.EvolveCost > 0 && m_HeroUnit.EvolveShard != null;
            m_ShardAmountNeed.gameObject.SetActive(showCost);
            if (showCost) m_ShardAmountNeed.SetView(new Currency(m_HeroUnit.EvolveShard, m_HeroUnit.EvolveCost));
            m_CoinAmountNeed.gameObject.SetActive(false);
            m_QuickAccessButton.gameObject.SetActive(false);
        }

        private void OpenEvolve()
        {
            if (m_HeroUnit == null || !m_HeroUnit.Owned) return;
            m_UpgradeView.Init(m_HeroUnit);
            m_UpgradeView.Show();
        }
    }
}
