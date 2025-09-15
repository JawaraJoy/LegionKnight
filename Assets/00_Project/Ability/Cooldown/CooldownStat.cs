using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class CooldownStat
    {
        [SerializeField]
        private float m_CooldownTime = 5f;
        [SerializeField]
        private float m_CooldownUpgrade = 0f;

        public float CooldownTime => m_CooldownTime;
        public float CooldownUpgrade => m_CooldownUpgrade;

        public void SetCooldownTime(float cooldownTime)
        {
            m_CooldownTime = cooldownTime;
        }
        public void SetCooldownUpgrade(float cooldownUpgrade)
        {
            m_CooldownUpgrade = cooldownUpgrade;
        }
        public CooldownStat(float cooldownTime, float cooldownUpgrade)
        {
            m_CooldownTime = cooldownTime;
            m_CooldownUpgrade = cooldownUpgrade;
        }
    }

    public partial class AbilityDefinition
    {
        [SerializeField]
        private CooldownStat m_CooldownStat;

        public CooldownStat CooldownStat => m_CooldownStat;

        public float CooldownTime => m_CooldownStat.CooldownTime;
        public float CooldownUpgrade => m_CooldownStat.CooldownUpgrade;

        public void SetCooldownTime(float cooldownTime)
        {
            m_CooldownStat.SetCooldownTime(cooldownTime);
        }
        public void SetCooldownUpgrade(float cooldownUpgrade)
        {
            m_CooldownStat.SetCooldownUpgrade(cooldownUpgrade);
        }
    }
}
