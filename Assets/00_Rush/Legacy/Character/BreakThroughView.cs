using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BreakThroughView : UIView
    {
        [SerializeField] private TextMeshProUGUI m_ShardNameText;
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private CurrencyView m_ShardNeed;
        [SerializeField] private CurrencyView m_CoinNeed;
        [SerializeField] private CurrencyView m_ShardOwned;
        [SerializeField] private CurrencyView m_CoinOwned;
        [SerializeField] private Button m_UpgradeButton;
        [SerializeField] private Button m_QuickAccessButton;

        [SerializeField] private EvolveDetailsView m_Details;
        private HeroUnit m_HeroUnit;
        private CurrenciesControl m_Wallet;
        private HeroesCollection m_Collection;

        private void OnEnable()
        {
            m_UpgradeButton.onClick.AddListener(EvolveHero);
            if (IsShown && m_HeroUnit != null) Listen();
        }

        private void OnDisable()
        {
            m_UpgradeButton.onClick.RemoveListener(EvolveHero);
            StopListening();
        }

        public void Init(HeroUnit unit)
        {
            m_HeroUnit = unit;
            Refresh();
        }

        private void EvolveHero()
        {
            if (m_HeroUnit == null) return;
            if (m_HeroUnit.TryEvolve()) Hide();
            else Refresh();
        }

        protected override void ShowInternal()
        {
            // The tutorial can open this view directly, without passing through the Hero button.
            var collection = Player.Instance.HeroesCollection;
            if (collection.SelectedHero != null)
                m_HeroUnit = collection.GetHeroUnit(collection.SelectedHero);
            base.ShowInternal();
            Listen();
        }

        private void Listen()
        {
            StopListening();
            m_Wallet = Player.Instance.CurrencyControl;
            m_Wallet.OnCurrencyChanged.AddListener(CurrencyChanged);
            m_Collection = Player.Instance.HeroesCollection;
            m_Collection.OnSelectedCharacter.AddListener(SelectionChanged);
            Refresh();
        }

        protected override void HideInternal()
        {
            StopListening();
            base.HideInternal();
        }

        private void StopListening()
        {
            if (m_Wallet != null)
            {
                m_Wallet.OnCurrencyChanged.RemoveListener(CurrencyChanged);
                if (m_Collection != null) m_Collection.OnSelectedCharacter.RemoveListener(SelectionChanged);
            }
            m_Collection = null;
            m_Wallet = null;
        }

        private void SelectionChanged(HeroUnitConfig config)
        {
            if (m_HeroUnit?.HeroConfig != config) Hide();
        }

        private void CurrencyChanged(Currency currency)
        {
            if (currency.ItemConfig == m_HeroUnit?.EvolveShard) Refresh();
        }

        private void Refresh()
        {
            if (m_HeroUnit == null) return;
            var shard = m_HeroUnit.EvolveShard;
            bool max = m_HeroUnit.Star >= HeroEvolution.MaxStage;
            int owned = shard == null ? 0 : Player.Instance.CurrencyControl.GetCurrencyAmount(shard);
            if (m_Details != null)
            {
                m_Details.Display(m_HeroUnit, owned, Player.Instance.BadgeManager?.GetAllBadges(), m_HeroUnit.CanEvolve());
                return;
            }
            int cost = m_HeroUnit.EvolveCost;
            if (m_Title != null)
                m_Title.text = max ? "Evolve 12 / 12" : $"Evolve {m_HeroUnit.Star} → {m_HeroUnit.Star + 1}";
            m_ShardNameText.text = max ? "Max Evolve" : $"Owned {shard?.BaseInfo.Name}";
            m_UpgradeButton.interactable = m_HeroUnit.CanEvolve();
            m_QuickAccessButton.gameObject.SetActive(!max && shard != null && owned < cost);
            m_CoinOwned.gameObject.SetActive(false);
            m_CoinNeed.gameObject.SetActive(false);
            m_ShardOwned.gameObject.SetActive(shard != null && !max);
            m_ShardNeed.gameObject.SetActive(shard != null && !max);
            if (shard == null || max) return;
            m_ShardOwned.SetView(new Currency(shard, owned));
            m_ShardNeed.SetView(new Currency(shard, cost));
        }
    }
}
