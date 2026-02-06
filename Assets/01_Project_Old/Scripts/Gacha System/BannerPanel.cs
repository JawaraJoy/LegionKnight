using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string BannerPanelId = "Banner";
    }
    public partial class BannerPanel : PanelView
    {
        public override string UniqueId => PanelId.BannerPanelId;

        [SerializeField]
        private GachaBanner m_SelectedBanner;
        [SerializeField]
        private UnityEvent<GachaBanner> m_OnSetSelectedBanner = new();
        [SerializeField]
        private UnityEvent<GachaBanner> m_OnInitPanel = new();
        [SerializeField]
        private UnityEvent<CurrencyDefinition> m_OnNotEnoughCurrency = new();
        [SerializeField]
        private NotEnoughDrawCostView m_NotEnoughDrawCostView;
        [SerializeField]
        private GachaManagerAgent[] m_GachaManagerAgents;
        public void OnNotEnoughtCurrencyInvoke(CurrencyDefinition definition)
        {
            m_OnNotEnoughCurrency?.Invoke(definition);
            m_NotEnoughDrawCostView.SetShow(definition);
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            InitPanel();
        }
        public void SetSeletedBanner(GachaBanner set)
        {
            m_SelectedBanner = set;
            OnSelectedBannerInvoke();
        }   
        private void OnSelectedBannerInvoke()
        {
            m_OnSetSelectedBanner?.Invoke(m_SelectedBanner); 
        }
        private void InitPanel()
        {
            GachaBanner gachaBanner = GameManager.Instance.GetSelectedBanner();
            m_OnInitPanel?.Invoke(gachaBanner);
            foreach (var agent in m_GachaManagerAgents)
            {
                agent.RefreshMultiDrawButton();
                agent.RefreshSingleDrawButton();
            }
        }

        
    }
}
