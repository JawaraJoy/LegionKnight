using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Spin Wheel", menuName = "Legion Knight/SpinWheel/SpinWheel")]
    public class SpinWheelDefinition : ScriptableObject
    {
        [SerializeField]
        private int m_FreeDrawAmount;
        [SerializeField]
        private TimerDefinition m_FreeDrawResetTime;
        [SerializeField]
        private int m_MinimalSpinStep = 40;
        [SerializeField]
        private int m_MinAdditionalSpinStep = 16;
        [SerializeField]
        private int m_MaxAdditionalSpinStep = 40;
        [SerializeField]
        private float m_StartStepDelay = 0.1f;
        [SerializeField]
        private float m_MidDelayGrowthStep = 0.05f;
        [SerializeField]
        private float m_EndDelayGrowthEachStep = 0.05f;
        [SerializeField]
        private SpinRewardDefinition[] m_Rewards;

        public int FreeDrawAmount => m_FreeDrawAmount;
        public TimerDefinition FreeDrawResetTime => m_FreeDrawResetTime;
        public int MiniSpinStep => m_MinimalSpinStep;
        public int MinAdditionalSpinStep => m_MinAdditionalSpinStep;
        public int MaxAdditionalSpinStep => m_MaxAdditionalSpinStep;
        public float StartStepDelay => m_StartStepDelay;
        public float MidDelayGrowthStep => m_MidDelayGrowthStep;
        public float EndDelayGrowthStep => m_EndDelayGrowthEachStep;
        public SpinRewardDefinition[] Rewards => m_Rewards;

        private SpinRewardDefinition GetSpinReward(string id)
        {
            foreach (var r in m_Rewards)
            {
                if (r.Id == id)
                {
                    return r;
                }
            }
            return null;
        }

        public bool HasSpinReward(string id, out SpinRewardDefinition defi)
        {
            bool has = GetSpinReward(id) != null;
            if (has)
            {
                defi = GetSpinReward(id);
            }
            else
            {
                defi = null;
            }
            return has;
        }

        public bool IsTimeToReset(UnityAction<int> onReset)
        {
            bool isTimeToReset = m_FreeDrawResetTime.IsTimeToReset();
            if (isTimeToReset)
            {
                onReset.Invoke(m_FreeDrawAmount);
            }
            return isTimeToReset;
        }
    }
}
