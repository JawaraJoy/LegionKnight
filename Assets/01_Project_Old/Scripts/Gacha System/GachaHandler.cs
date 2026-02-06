using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class GachaHandler : MonoBehaviour
    {
        [SerializeField] private List<GachaBanner> m_Banners = new();
        [SerializeField] private UnityEvent<List<GachaReward>> m_OnDrawResult;
        [SerializeField] private UnityEvent<string> m_OnDrawFailed;
        [SerializeField] private UnityEvent m_OnPerformSingleDraw;
        [SerializeField] private UnityEvent m_OnPerformMultiDraw;

        private GachaBanner m_Selected;
        private bool m_IsDrawing;

        public void Init()
        {
            if (m_Banners.Count == 0)
            {
                Debug.LogError("No gacha banners assigned");
                return;
            }

            foreach (var banner in m_Banners)
                banner.Init();

            m_Selected = m_Banners[0];
        }

        public void PerformSingleDraw()
        {
            PerformDraw(1);
            m_OnPerformSingleDraw?.Invoke();
        }

        public void PerformMultiDraw()
        {
            PerformDraw(m_Selected.Definition.MultiDraw);
            m_OnPerformMultiDraw?.Invoke();
        }
        public GachaBanner GetSelectedBanner()
        {
            return m_Selected;
        }

        private void PerformDraw(int count)
        {
            if (m_IsDrawing || m_Selected == null)
                return;

            var cost = ResolveCost(count);
            CurrencyDefinition currencyDefinition = cost.Definition;
            int costAmount = cost.Amount;
            int playerCurrencyAmount = Player.Instance.GetCurrencyAmount(currencyDefinition);

            if (playerCurrencyAmount < cost.Amount)
            {
                m_OnDrawFailed?.Invoke("Not enough currency");
                return;
            }

            Player.Instance.AddCurrencyAmount(cost.Definition, -cost.Amount);

            m_IsDrawing = true;

            List<GachaReward> results = new();
            m_Selected.Draw(count, results);

            foreach (var r in results)
                r.Apply();

            m_OnDrawResult?.Invoke(results);

            m_IsDrawing = false;
        }

        private GachaCurrencyCost ResolveCost(int count)
        {
            var main = m_Selected.Definition.MainCurrency;
            int total = main.Amount * count;

            if (Player.Instance.GetCurrencyAmount(main.Definition) < total)
            {
                var alt = m_Selected.Definition.AlternativeCurrency;
                return new GachaCurrencyCost(alt.Definition, alt.Amount * count);
            }

            return new GachaCurrencyCost(main.Definition, total);
        }
    }
}
