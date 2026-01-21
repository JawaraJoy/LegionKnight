using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class BosDotGrowth : MonoBehaviour
    {
        [SerializeField]
        private DotStat m_BaseStat;
 
        private int m_BosLevel = 0;

        public void ApplyDamageOvertime()
        {
            BosEnemy bosEnemy = GameManager.Instance.SpawnedBosenemy;
            if (bosEnemy != null)
            {
                m_BosLevel = bosEnemy.GetBosLevel();
                int atk = m_BaseStat.GetFinalDamagePerTick(m_BosLevel);
                float dur = m_BaseStat.GetFinalDuration(m_BosLevel);


                Player.Instance.ApplyPlayerDamageOverTime(atk, dur);
            }
        }
    }
}
