using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class BosDirectDamage : MonoBehaviour
    {
        [SerializeField]
        private bool m_DealDamageOnInitialization = true;
        [SerializeField]
        private int m_Damage;
        [SerializeField]
        private int m_DamageGrowth;

        [SerializeField]
        private UnityEvent<int> m_OnDealDamage = new UnityEvent<int>();


        private int m_FinalDamage;

        private void Start()
        {
            
        }
        public void Init()
        {
            BosEnemy bos = GameManager.Instance.SpawnedBosenemy;
            int level = bos.GetBosLevel();
            m_FinalDamage = m_Damage + (m_DamageGrowth * level - 1);
            if (m_DealDamageOnInitialization)
            {
                DealDamage();
            }
        }
        private void DealDamage()
        {
            Player.Instance.TakeDamage(m_FinalDamage);
            m_OnDealDamage.Invoke(m_FinalDamage);
        }
    }
}
