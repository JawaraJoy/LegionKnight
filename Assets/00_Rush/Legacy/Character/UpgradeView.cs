using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class UpgradeView : UIView
    {
        [SerializeField] private TextMeshProUGUI m_ItemNameText;
        [SerializeField] private CurrencyView m_ItemNeed;
        [SerializeField] private CurrencyView m_ItemOwned;
        [SerializeField] private Button m_UpgradeButton;
        [SerializeField] private Button m_QuickAccessButton;
        [SerializeField] private StatView[] m_StatViews;
        [SerializeField] private HeroUpgradeDetailsView m_Details;

        private HeroUnit m_HeroUnit;
        private CurrenciesControl m_Wallet;
        private HeroesCollection m_Collection;

        private void OnEnable()
        {
            m_UpgradeButton.onClick.AddListener(UpgradeHero);
            if (IsShown && m_HeroUnit != null) Listen();
        }

        private void OnDisable()
        {
            m_UpgradeButton.onClick.RemoveListener(UpgradeHero);
            StopListening();
        }

        public void Init(HeroUnit unit)
        {
            m_HeroUnit = unit;
            Refresh();
        }

        public void Refresh()
        {
            var formula = m_HeroUnit?.HeroConfig?.LevelFormulaConfig;
            var item = formula != null ? formula.ItemRequirmentConfig : null;
            bool max = m_HeroUnit != null && m_HeroUnit.Level >= HeroEvolution.MaxLevel;
            int cost = formula != null && !max ? formula.GetCurrentMaxExperience(m_HeroUnit.Level) : 0;
            int owned = item != null ? Player.Instance.CurrencyControl.GetCurrencyAmount(item) : 0;
            bool valid = m_HeroUnit != null && m_HeroUnit.Owned && !max && item != null && cost > 0;
            bool ready = valid && owned >= cost;
            m_UpgradeButton.interactable = ready;
            m_QuickAccessButton.gameObject.SetActive(valid && !ready);
            if (m_HeroUnit?.HeroConfig == null)
            {
                if (IsShown) Hide();
                return;
            }
            if (m_Details != null)
            {
                m_Details.Display(m_HeroUnit, item, cost, owned, ready, max);
                return;
            }
            if (item == null) return;
            m_ItemNeed.SetView(new Currency(item, cost));
            m_ItemOwned.SetView(new Currency(item, owned));
            m_ItemNameText.text = $"Owned {item.BaseInfo.Name}:";
        }

        public void UpgradeHero()
        {
            if (m_UpgradeButton.interactable && m_HeroUnit?.HeroConfig?.LevelFormulaConfig != null)
                m_HeroUnit.TryUpgradeLevel();
            Refresh();
        }

        protected override void ShowInternal()
        {
            var collection = Player.Instance.HeroesCollection;
            if (collection.SelectedHero != null)
                m_HeroUnit = collection.GetHeroUnit(collection.SelectedHero);
            if (m_HeroUnit == null) return;
            base.ShowInternal();
            if (!IsShown) return;
            Listen();
            foreach (var stat in m_StatViews) stat.ShowNextValue();
        }

        protected override void HideInternal()
        {
            StopListening();
            foreach (var stat in m_StatViews) stat.HideNextValue();
            base.HideInternal();
        }

        private void Listen()
        {
            StopListening();
            m_Wallet = Player.Instance.CurrencyControl;
            m_Collection = Player.Instance.HeroesCollection;
            m_Wallet.OnCurrencyChanged.AddListener(OnBalanceChanged);
            m_Collection.OnSelectedCharacter.AddListener(OnSelectionChanged);
            Refresh();
        }

        private void StopListening()
        {
            if (m_Wallet != null) m_Wallet.OnCurrencyChanged.RemoveListener(OnBalanceChanged);
            if (m_Collection != null) m_Collection.OnSelectedCharacter.RemoveListener(OnSelectionChanged);
            m_Wallet = null;
            m_Collection = null;
        }

        private void OnBalanceChanged(Currency balance)
        {
            if (balance.ItemConfig == m_HeroUnit?.HeroConfig?.LevelFormulaConfig?.ItemRequirmentConfig)
                Refresh();
        }

        private void OnSelectionChanged(HeroUnitConfig config)
        {
            if (m_HeroUnit?.HeroConfig != config) Hide();
        }
    }
}
