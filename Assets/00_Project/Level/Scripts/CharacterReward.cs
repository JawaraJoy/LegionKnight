using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public enum RewardState
    {
        First,
        Repeat
    }
    [CreateAssetMenu(fileName = "CharacterReward", menuName = "Legion Knight/CharacterReward", order = 1)]
    public class CharacterReward : ScriptableObject
    {
        [SerializeField]
        private RewardState m_RewardState;
        [SerializeField]
        private RewardObject[] m_Rewards;

        public RewardState RewardState => m_RewardState;
        public RewardObject[] Rewards => m_Rewards;

        public void ClaimReward()
        {
            foreach (var reward in m_Rewards)
            {
                reward.ClaimReward();
            }
        }
    }

    [System.Serializable]
    public class RewardObject
    {
        [SerializeField]
        private ScriptableObject m_Defi;
        [SerializeField]
        private int m_Amount;

        public ScriptableObject Defi => m_Defi;
        public int Amount => m_Amount;

        public void ClaimReward()
        {
            if (m_Defi is CurrencyDefinition currency)
            {
                Player.Instance.AddCurrencyAmount(currency, m_Amount);
            }
            if (m_Defi is CharacterDefinition c)
            {
                Player.Instance.SetOwned(c, true);
            }
            if (m_Defi is StandbyPlatformDefinition p)
            {
                Player.Instance.AddPlatformAmount(p, m_Amount);
            }
        }
    }
}
