using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace LegionKnight
{
    [System.Serializable]
    public partial class GachaBanner
    {
        [SerializeField]
        private int m_TotalDraws = 0;
        [SerializeField]
        private bool m_UseAltermatifCurrencyToDraw;

        //public PlayableDirector timeline;

        [SerializeField]
        private BannerDefinition m_Definition;
        [SerializeField]
        private bool m_SkipTimeline = false;
        [SerializeField]
        private DrawDiscount m_SingleDrawDiscount;
        [SerializeField]
        private DrawDiscount m_MultipleDrawDiscount;
        [SerializeField]
        private UnityEvent<List<GachaReward>> m_OnDrawResultSuccess = new();
        [SerializeField]
        private UnityEvent<CurrencyDefinition> m_OnDrawResultFail = new();
        public string PromoText => m_Definition.PromoText;
        private int MultiDrawInternal => m_Definition.MultiDraw;
        public int MultiDraw => MultiDrawInternal;
        private int GuaranteedDrawInternal => m_Definition.GuaranteedDraw;
        private List<GachaReward> MainRewardInternal => m_Definition.MainRewards;
        public int PlayerCurrencyAmountInternal => Player.Instance.GetCurrencyAmount(GetPlayerCurrency());
        private List<GachaReward> GachaRewardsInternal => m_Definition.GachaRewards;
        public BannerDefinition Definition => m_Definition;
        public int TotalDraws => m_TotalDraws;
        public bool SkipTimeline => m_SkipTimeline;
        public Sprite VisualBanner => m_Definition.VisualBanner;
        public Sprite SmallVisualBanner => m_Definition.SmallVisualBanner;

        public float GetDrawCountRate()
        {
            return (float) m_TotalDraws / (float)GuaranteedDrawInternal;
        }
        private CurrencyDefinition GetPlayerCurrency()
        {
            CurrencyDefinition gachaCurrency = GetSelectedGachaCurrencyCostInternal().Definition;
            return gachaCurrency;
        }
        private GachaCurrencyCost GetSelectedGachaCurrencyCostInternal()
        {
            GachaCurrencyCost cost = m_UseAltermatifCurrencyToDraw ? m_Definition.MainCurrencyToDraw : m_Definition.AlternatifCurrencyToDraw;
            return cost;
        }
        public GachaCurrencyCost GetSelectedGachaCurrencyCost()
        {
            GachaCurrencyCost cost = GetSelectedGachaCurrencyCostInternal();
            return cost;
        }
        public GachaCurrencyCost GetFinalCurrencyCost(int amount)
        {
            CurrencyDefinition defi = GetSelectedGachaCurrencyCostInternal().Definition;
            int finalCost = GetFinalCostInternal(amount);
            return new GachaCurrencyCost(defi, finalCost);
        }
        public int GetFinalSingleDrawCost()
        {
            return GetFinalCostInternal(1);
        }
        public int GetFinalMultiDrawCost()
        {
            return GetFinalCostInternal(MultiDrawInternal);
        }
        private int GetFinalCostInternal(int drawCount)
        {
            int cost = GetSelectedGachaCurrencyCostInternal().Amount * drawCount;
            bool used = m_SingleDrawDiscount.Used;
            float discount = m_SingleDrawDiscount.PriceRate;
            if (drawCount > 1)
            {
                used = m_MultipleDrawDiscount.Used;
                discount = m_MultipleDrawDiscount.PriceRate;
            }
            int finalCost = used ? Mathf.RoundToInt(cost * discount) : cost;
            return finalCost;
        }
        public void PerformingSingleDraw()
        {
            int finalCost = GetFinalCostInternal(1);
            m_SingleDrawDiscount.SetUsed(false);
            GameManager.Instance.StartCoroutine(PerformDrawCoroutine(1, finalCost));
        }

        public void PerformingMultiDraw()
        {
            int finalCost = GetFinalCostInternal(MultiDrawInternal);
            m_MultipleDrawDiscount.SetUsed(false);
            GameManager.Instance.StartCoroutine(PerformDrawCoroutine(MultiDrawInternal, finalCost));
        }

        private IEnumerator PerformDrawCoroutine(int drawCount, int cost)
        {
            if (PlayerCurrencyAmountInternal < cost)
            {
                m_OnDrawResultFail?.Invoke(GetPlayerCurrency());
                yield break;
            }

            Player.Instance.AddCurrencyAmount(GetPlayerCurrency(), -cost);
            List<GachaReward> results = new();
            string allRewards = "";
            for (int i = 0; i < drawCount; i++)
            {
                m_TotalDraws++;
                GachaReward result = CalculateDrawResult();
                results.Add(result);
                
            }
            m_OnDrawResultSuccess?.Invoke(results);
            foreach (GachaReward re in results)
            {
                allRewards += re.Definition.name;
                re.ApplyRewardToPlayer();
            }
            
            if (!m_SkipTimeline)
            {
                //timeline.Play();
                //yield return new WaitUntil(() => timeline.state != PlayState.Playing);
                yield return new WaitForSeconds(1f);
            }

            
            Debug.Log($"Gacha Reward {allRewards}");
        }

        private GachaReward CalculateDrawResult()
        {
            if (m_TotalDraws >= GuaranteedDrawInternal)
            {
                m_TotalDraws = 0; // Reset counter after main reward
                int random = Random.Range(0, MainRewardInternal.Count);

                return MainRewardInternal[random];
            }

            float roll = Random.value;
            float cumulative = 0f;

            foreach (var reward in GachaRewardsInternal)
            {
                cumulative += reward.DropRate;
                if (roll < cumulative)
                {
                    GachaReward result = reward;
                    
                    return result;
                }    
            }
            return GachaRewardsInternal[0]; // Default to first reward if no match
        }
    }
}
