using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Stat Influencer", menuName = "Rush/Combat/Ability/Stat Influencer", order = 1)]
    public class StatInfluencerConfig : AbilityConfig
    {
        [SerializeField]
        private StatInfluencer m_StatInfluencerPrefab;
        [SerializeField]
        private ModifierType m_ModifierType = ModifierType.Buff;
        [SerializeField]
        private float m_Duration = 5f;
        [SerializeField]
        private float m_DurationGrowthByLevel = 0f;
        [SerializeField]
        private int m_MaxStackCount = 1;
        [SerializeField]
        private int m_UpdatePerStackCount = 1;
        [SerializeField]
        private bool m_ResetDurationOnStackUpdate = true;
        [SerializeField]
        private HowStackUpdate m_HowStackUpdate = HowStackUpdate.Addictive;
        [SerializeField]
        private HowStatRemoved m_HowToRemove = HowStatRemoved.RemoveOnDurationEnd;
        public ModifierType ModifierType => m_ModifierType;
        public StatInfluencer StatInfluencerPrefab => m_StatInfluencerPrefab;
        public float Duration => m_Duration;
        public float DurationGrowthByLevel => m_DurationGrowthByLevel;
        public float FinalDurationByLevel(int level)
        {
            return m_Duration + m_DurationGrowthByLevel * level;
        }
        public int MaxStackCount => m_MaxStackCount;
        public int GetStartingStack()
        {
            int startingStack = 0;
            switch (m_HowStackUpdate)
            {
                case HowStackUpdate.Addictive:
                    startingStack = 1;
                    break;
                case HowStackUpdate.Subtractive:
                    startingStack = m_MaxStackCount;
                    break;
            }
            return startingStack;
        }
        public int UpdatePerStackCount => m_UpdatePerStackCount;
        public bool ResetDurationOnStackUpdate => m_ResetDurationOnStackUpdate;
        public HowStackUpdate HowStackUpdate => m_HowStackUpdate;
        public HowStatRemoved HowToRemove => m_HowToRemove;
    }
    public enum HowStatRemoved
    {
        RemoveOnDurationEnd,
        RemoveOnStackZero,
        RemoveOnStackExceedMax,
        RemoveStackOnDurationEnd, // ✅ NEW
    }
    public enum HowStackUpdate
    {
        Addictive = 0,
        Subtractive = 1,
    }
    public enum ModifierType
    {
        Buff = 0,
        Debuff = 1,
    }
}