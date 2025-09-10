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
        private SpinRewardDefinition[] m_Rewards;

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
