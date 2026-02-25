using Rush;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public enum LastDrawType
    {
        None,
        Single,
        Multi
    }
    public class GachaHandler : MonoBehaviour
    {
        [SerializeField] private List<GachaBanner> m_Banners = new();
        [SerializeField] private UnityEvent<List<GachaRewardConfig>> m_OnDrawResult;
        [SerializeField] private UnityEvent<ItemConfig> m_OnDrawFailed;
        [SerializeField] private UnityEvent m_OnPerformSingleDraw;
        [SerializeField] private UnityEvent m_OnPerformMultiDraw;
        [SerializeField] private UnityEvent<int, int> m_OnDrawGuaraantedCount;

        private List<GachaManagerAgent> m_GachaManagerAgents = new();

        private LastDrawType m_LastDrawType = LastDrawType.None;
        private GachaCurrencyCost m_LastDrawCost;

        public LastDrawType LastDrawType => m_LastDrawType;
        public GachaCurrencyCost LastDrawCost => m_LastDrawCost;

        private GachaBanner m_Selected;

        private GachaBanner GetGachaBanner(BannerDefinition definition)
        {
            foreach (var banner in m_Banners)
            {
                if (banner.Definition.Id == definition.Id)
                    return banner;
            }
            return null;
        }

        public void Init()
        {
            m_GachaManagerAgents = new List<GachaManagerAgent>(FindObjectsByType<GachaManagerAgent>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            if (m_Banners.Count == 0)
            {
                Debug.LogError("No gacha banners assigned");
                return;
            }

            foreach (var banner in m_Banners)
                banner.Init();

            m_Selected = m_Banners[0];
            InitAgentButton();
            m_OnDrawGuaraantedCount?.Invoke(m_Selected.TotalDraws, m_Selected.Definition.GuaranteedDraw);
        }

        private void InitAgentButton()
        {
            foreach (var agent in m_GachaManagerAgents)
            {
                agent.RefreshMultiDrawButton();
                agent.RefreshSingleDrawButton();
            }
        }

        public void PerformSingleDraw()
        {
            PerformDraw(1);
            m_LastDrawType = LastDrawType.Single;
            m_LastDrawCost = ResolveCost(1);
            m_OnPerformSingleDraw?.Invoke();
        }

        public void PerformMultiDraw()
        {
            int count = m_Selected.Definition.MultiDraw;
            PerformDraw(count);
            m_LastDrawType = LastDrawType.Multi;
            m_LastDrawCost = ResolveCost(count);
            m_OnPerformMultiDraw?.Invoke();
        }
        public GachaBanner GetSelectedBanner()
        {
            return m_Selected;
        }

        public void SelectBanner(BannerDefinition banner)
        {
            m_Selected = GetGachaBanner(banner);
            InitAgentButton();
            m_OnDrawGuaraantedCount?.Invoke(m_Selected.TotalDraws, m_Selected.Definition.GuaranteedDraw);
        }

        private void PerformDraw(int count)
        {
            if (m_Selected == null)
                return;

            var cost = ResolveCost(count);
            ItemConfig currencyDefinition = cost.ItemConfig;
            int costAmount = cost.Amount;
            int playerCurrencyAmount = Player.Instance.CurrencyControl.GetCurrencyAmount(currencyDefinition);

            if (playerCurrencyAmount < cost.Amount)
            {
                m_OnDrawFailed?.Invoke(currencyDefinition);
                CanvasManager.Instance.GetPanel<BannerPanel>().OnNotEnoughtCurrencyInvoke(currencyDefinition);
                return;
            }

            Player.Instance.CurrencyControl.AddCurrencyAmount(cost.ItemConfig, -costAmount);

            

            List<GachaRewardConfig> results = new();
            m_Selected.Draw(count, results);

            foreach (var r in results)
                r.Apply();

            m_OnDrawResult?.Invoke(results);
            m_OnDrawGuaraantedCount?.Invoke(m_Selected.TotalDraws, m_Selected.Definition.GuaranteedDraw);

            InitAgentButton();
        }

        private GachaCurrencyCost ResolveCost(int count)
        {
            var main = m_Selected.Definition.MainCurrency;
            int total = main.Amount * count;

            if (Player.Instance.CurrencyControl.GetCurrencyAmount(main.ItemConfig) < total)
            {
                var alt = m_Selected.Definition.AlternativeCurrency;
                return new GachaCurrencyCost(alt.ItemConfig, alt.Amount * count);
            }

            return new GachaCurrencyCost(main.ItemConfig, total);
        }
    }
}
