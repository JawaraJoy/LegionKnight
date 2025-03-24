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
        private int m_TotalDraws = 0;
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
        private UnityEvent<List<string>> m_OnDrawResultSuccess = new();
        [SerializeField]
        private UnityEvent m_OnDrawResultFail = new();
        private int MultiDrawInternal => m_Definition.MultiDraw;
        private int GuaranteedDrawInternal => m_Definition.GuaranteedDraw;
        private List<GachaReward> MainRewardInternal => m_Definition.MainRewards;
        public int PlayerCurrencyInternal => Player.Instance.GetCurrencyAmount(GetPlayerCurrency());
        private List<GachaReward> GachaRewardsInternal => m_Definition.GachaRewards;
        private CurrencyDefinition GetPlayerCurrency()
        {
            CurrencyDefinition gachaCurrency = GetGachaCurrencyCost().Definition;
            return gachaCurrency;
        }
        private GachaCurrencyCost GetGachaCurrencyCost()
        {
            GachaCurrencyCost cost = m_UseAltermatifCurrencyToDraw ? m_Definition.MainCurrencyToDraw : m_Definition.AlternatifCurrencyToDraw;
            return cost;
        }
        public void PerformingSingleDraw()
        {
            int cost = GetGachaCurrencyCost().Amount;
            bool used = m_SingleDrawDiscount.Used;
            float discount = m_SingleDrawDiscount.Discount;
            int finalCost = used ? Mathf.RoundToInt(cost * discount) : cost;
            m_SingleDrawDiscount.SetUsed(false);
            GameManager.Instance.StartCoroutine(PerformDrawCoroutine(1, finalCost));
        }

        public void PerformingMultiDraw()
        {
            int cost = GetGachaCurrencyCost().Amount * MultiDrawInternal;
            bool used = m_MultipleDrawDiscount.Used;
            float discount = m_MultipleDrawDiscount.Discount;
            int finalCost = used ? Mathf.RoundToInt(cost * discount) : cost;
            m_MultipleDrawDiscount.SetUsed(false);
            GameManager.Instance.StartCoroutine(PerformDrawCoroutine(MultiDrawInternal, finalCost));
        }

        private IEnumerator PerformDrawCoroutine(int drawCount, int cost)
        {
            if (PlayerCurrencyInternal < cost)
            {
                m_OnDrawResultFail?.Invoke();
                yield break;
            }

            Player.Instance.AddCurrencyAmount(GetPlayerCurrency(), -cost);
            List<string> results = new List<string>();

            for (int i = 0; i < drawCount; i++)
            {
                m_TotalDraws++;
                results.Add(CalculateDrawResult());
            }

            if (!m_SkipTimeline)
            {
                //timeline.Play();
                //yield return new WaitUntil(() => timeline.state != PlayState.Playing);
                yield return new WaitForSeconds(1f);
            }

            m_OnDrawResultSuccess?.Invoke(results);
        }

        private string CalculateDrawResult()
        {

            if (m_TotalDraws >= GuaranteedDrawInternal)
            {
                m_TotalDraws = 0; // Reset counter after main reward
                int random = Random.Range(0, MainRewardInternal.Count);
                return MainRewardInternal[random].RewardName;
            }

            float roll = Random.value;
            float cumulative = 0f;

            foreach (var reward in GachaRewardsInternal)
            {
                cumulative += reward.DropRate;
                if (roll < cumulative)
                {
                    string result = $"{reward.RewardName}, ";
                    return result;
                }    
            }
            return GachaRewardsInternal[0].RewardName; // Default to first reward if no match
        }
    }
}
