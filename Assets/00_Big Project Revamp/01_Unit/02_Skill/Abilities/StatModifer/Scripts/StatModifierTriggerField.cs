using UnityEngine;

namespace Rush
{
    public class StatModifierTriggerField
    {
        [SerializeField]
        private TriggerPurpose m_Purpose;
        [SerializeField]
        private StatModifierConfig m_Config;

        public void Trigger(StatController statcontroller)
        {
            switch (m_Purpose)
            {
                case TriggerPurpose.Activate:
                    statcontroller.AddModifier(null, statcontroller);
                    break;
                case TriggerPurpose.UpdateStack:
                    statcontroller.UpdateStack();
                    break;
            }
        }
    }
    public enum TriggerPurpose
    {
        Activate,
        UpdateStack,
    }
}
