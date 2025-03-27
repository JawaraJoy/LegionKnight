using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class BannerDrawResultView : ResultView
    {
        [SerializeField]
        private GachaCurrencyCost m_GachaCost;

        [SerializeField]
        private UnityEvent<GachaCurrencyCost> m_OnSetGachaCost = new();
        public void SetGachaCost(GachaCurrencyCost cost)
        {
            m_GachaCost = cost;
            OnSetGachaCost();
        }
        private void OnSetGachaCost()
        {
            m_OnSetGachaCost?.Invoke(m_GachaCost);
            Debug.Log($"Set Gacha Cost");
        }
    }
    public partial class BannerPanel
    {
        public void SetGachaCost(GachaCurrencyCost cost)
        {
            GetBinding<BannerDrawResultView>().SetGachaCost(cost);
        }
        public virtual void ShowResults(List<object> results)
        {
            GetBinding<BannerDrawResultView>().ShowResults(results);
        }
    }

    public partial class GachaManagerAgent
    {
        public void SetGachaCost(GachaCurrencyCost cost)
        {
            GetBannerPanel().SetGachaCost(cost);
        }
        public virtual void ShowResults(List<GachaReward> results)
        {
            List<object> rewards = new();
            foreach(GachaReward r in results)
            {
                rewards.Add(r);
            }
            GetBannerPanel().ShowResults(rewards);
        }
    }
}
