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
        private List<GachaReward> GachaRewardsInternal => m_Definition.GachaRewards;
        public BannerDefinition Definition => m_Definition;
        public int TotalDraws => m_TotalDraws;
        public bool SkipTimeline => m_SkipTimeline;
        public Sprite VisualBanner => m_Definition.VisualBanner;
        public Sprite SmallVisualBanner => m_Definition.SmallVisualBanner;

        public DrawDiscount SingleDrawDiscount => m_SingleDrawDiscount;
        public DrawDiscount MultipleDrawDiscount => m_MultipleDrawDiscount;

        public void Init()
        {
            if (UnityService.Instance.HasData(m_Definition.Id + "totaldraws"))
            {
                m_TotalDraws = UnityService.Instance.GetData<int>(m_Definition.Id + "totaldraws");
            }
            else
            {
                m_TotalDraws = 0;
            }
            if (UnityService.Instance.HasData(m_Definition.Id + "sfDUsed"))
            {
                m_SingleDrawDiscount.SetFirstDrawUsed(UnityService.Instance.GetData<bool>(m_Definition.Id + "sfDUsed"));
            }

            if (UnityService.Instance.HasData(m_Definition.Id + "mfDUsed"))
            {
                m_MultipleDrawDiscount.SetFirstDrawUsed(UnityService.Instance.GetData<bool>(m_Definition.Id + "mfDUsed"));
            }
        }
        public float GetDrawCountRate()
        {
            return (float)m_TotalDraws / (float)GuaranteedDrawInternal;
        }
        private GachaCurrencyCost GetSelectedGachaCurrencyCostInternal(int drawAmount)
        {
            int mainCost = m_Definition.MainCurrencyToDraw.Amount * drawAmount;
            int playerCurrencyAmount = Player.Instance.GetCurrencyAmount(m_Definition.MainCurrencyToDraw.Definition);
            bool alternatif = playerCurrencyAmount < mainCost;
            GachaCurrencyCost cost = alternatif ?
                m_Definition.AlternatifCurrencyToDraw :
                m_Definition.MainCurrencyToDraw;
            GachaCurrencyCost final = new GachaCurrencyCost(cost.Definition, cost.Amount * drawAmount);
            return final;
        }
        public GachaCurrencyCost GetSelectedGachaCurrencyCost(int drawAmount)
        {
            GachaCurrencyCost cost = GetSelectedGachaCurrencyCostInternal(drawAmount);
            return cost;
        }
        public GachaCurrencyCost GetFinalCurrencyCost(int drawAmount)
        {
            CurrencyDefinition defi = GetSelectedGachaCurrencyCostInternal(drawAmount).Definition;
            int finalCost = GetFinalCostInternal(drawAmount);
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
            int cost = GetSelectedGachaCurrencyCostInternal(drawCount).Amount;
            bool used = m_SingleDrawDiscount.Used;
            bool firstDrawUse = m_SingleDrawDiscount.FirstDrawUse;
            float discount = m_SingleDrawDiscount.PriceRate;
            float firstDrawDiscount = m_SingleDrawDiscount.PriceRateFirstDraw;
            if (drawCount > 1)
            {
                used = m_MultipleDrawDiscount.Used;
                firstDrawUse = m_MultipleDrawDiscount.FirstDrawUse;
                discount = m_MultipleDrawDiscount.PriceRate;
                firstDrawDiscount = m_MultipleDrawDiscount.PriceRateFirstDraw;

            }
            if (used && firstDrawUse)
            {
                cost = Mathf.CeilToInt(cost * discount * firstDrawDiscount);
            }
            else if (used)
            {
                cost = Mathf.CeilToInt(cost * discount);
            }
            else if (firstDrawUse)
            {
                cost = Mathf.CeilToInt(cost * firstDrawDiscount);
            }
            return cost;
        }
        public void PerformingSingleDraw()
        {
            int finalCost = GetFinalCostInternal(1);
            m_SingleDrawDiscount.SetFirstDrawUsed(true);
            GameManager.Instance.StartCoroutine(PerformDrawCoroutine(1, finalCost));
            UnityService.Instance.SaveData(m_Definition.Id + "sfDUsed", m_SingleDrawDiscount.FirstDrawUse);
        }

        public void PerformingMultiDraw()
        {
            int finalCost = GetFinalCostInternal(MultiDrawInternal);
            m_MultipleDrawDiscount.SetFirstDrawUsed(true);
            GameManager.Instance.StartCoroutine(PerformDrawCoroutine(MultiDrawInternal, finalCost));
            UnityService.Instance.SaveData(m_Definition.Id + "mfDUsed", m_MultipleDrawDiscount.FirstDrawUse);
        }

        private IEnumerator PerformDrawCoroutine(int drawCount, int cost)
        {
            CurrencyDefinition defi = GetSelectedGachaCurrencyCostInternal(drawCount).Definition;
            int playerCurrencyAmount = Player.Instance.GetCurrencyAmount(defi);
            if (playerCurrencyAmount < cost)
            {
                m_OnDrawResultFail?.Invoke(defi);
                yield break;
            }

            Player.Instance.AddCurrencyAmount(defi, -cost);
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
                //re.ApplyRewardToPlayer();
            }
            
            if (!m_SkipTimeline)
            {
                //timeline.Play();
                //yield return new WaitUntil(() => timeline.state != PlayState.Playing);
                yield return new WaitForSeconds(1f);
            }

            UnityService.Instance.SaveData(m_Definition.Id + "totaldraws", m_TotalDraws);
            Debug.Log($"Gacha Reward {allRewards}");
        }

        private GachaReward CalculateDrawResult()
        {
            if (m_TotalDraws >= GuaranteedDrawInternal)
            {
                m_TotalDraws = 0;
                int random = Random.Range(0, MainRewardInternal.Count);
                return MainRewardInternal[random];
            }

            // Normalize drop rates
            float totalDropRate = 0f;
            foreach (var reward in GachaRewardsInternal)
                totalDropRate += reward.DropRate;

            float roll = Random.value * totalDropRate;
            float cumulative = 0f;

            foreach (var reward in GachaRewardsInternal)
            {
                cumulative += reward.DropRate;
                if (roll < cumulative)
                    return reward;
            }
            // Fallback (should not happen if rates are set up correctly)
            return GachaRewardsInternal[^1];
        }
    }
}
