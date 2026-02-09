using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class ChargerContext 
    {
        [SerializeField, MMReadOnly]
        private Charger m_Charger;
        [SerializeField, MMReadOnly]
        private SkillActivator[] m_Skills;
        public Charger Charger => m_Charger;
        public SkillActivator[] Skills => m_Skills;
        public ChargerContext(Charger healer, SkillActivator[] skilss)
        {
            m_Charger = healer;
            m_Skills = skilss;
        }
    }
}
