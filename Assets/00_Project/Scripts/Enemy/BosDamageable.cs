using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class BosDamageable : Damageable
    {
        protected override void OnDeathInvoke()
        {
            base.OnDeathInvoke();
            //GameManager.Instance.BosDeathTriggered();
        }
        public void Heal(int amount)
        {
            AddCurrentHealthInternal(amount);
        }
    }
    public partial class BosEnemy
    {
        [SerializeField]
        private BosDamageable m_Damageable;
        public BosDamageable Damageable => m_Damageable;
        public void InitDamageable(int spawnCount) // level
        {
            Stat bosStat = m_BosDefinition.FinalStat(spawnCount);
            SetBosLevelInternal(m_BosDefinition.StartLevel + spawnCount);
            int atk = bosStat.Attack;
            int def = bosStat.Defense;
            int health = bosStat.Health;

            m_Damageable.SetHealth(health);
            m_Damageable.SetDamage(atk);
            m_Damageable.SetDefend(def);

        }
        public void Heal(int amount)
        {
            m_Damageable.Heal(amount);
        }
    }
    public partial class LevelHandler
    {
        [SerializeField]
        private UnityEvent m_OnBosDeath = new();

        public void BosDeathTriggered()
        {
            OnBosDeathInvoke();
        }
        private void OnBosDeathInvoke()
        {
            m_OnBosDeath?.Invoke();
        }
    }
    public partial class GameManager
    {
        public void BosDeathTriggered()
        {
            m_LevelManager.BosDeathTriggered();
        }
    }

    public partial class LevelManagerAgent
    {
        public void BosDeathTriggered()
        {
            GameManager.Instance.BosDeathTriggered();
        }
    }
}
