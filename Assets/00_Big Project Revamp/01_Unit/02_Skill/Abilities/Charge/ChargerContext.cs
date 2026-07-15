
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class ChargerContext 
    {
        private Charger m_Charger;
        private Skill[] m_Skills;
        public Charger Charger => m_Charger;
        public Skill[] Skills => m_Skills;
        public ChargerContext(Charger healer, Skill[] skilss)
        {
            m_Charger = healer;
            m_Skills = skilss;
        }
    }
}
