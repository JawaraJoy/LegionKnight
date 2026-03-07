using UnityEngine;

namespace Rush
{
    public class EnemyWaveEnemiesSkillAgent : MonoBehaviour
    {
        [SerializeField]
        private SkillCategoryConfig m_SkillCategoryToManipulate;

        private EnemyWaveHandler m_EnemyWaveHandler;

        private EnemyWaveHandler EnemyWaveHandler
        {
            get
            {
                if (m_EnemyWaveHandler == null)
                {
                    m_EnemyWaveHandler = RushGameManager.Instance.StageManager.EnemyWaveHandler;
                }
                return m_EnemyWaveHandler;
            }
        }
        public void AddCharge(int amount)
        {
            foreach (Unit spawner in EnemyWaveHandler.GetActiveEnemies())
            {
                if (spawner.HasBind(out SkillController skillController))
                {
                    CategorySkillController ultimates = skillController.GetCategoryController(m_SkillCategoryToManipulate);
                    if (ultimates != null)
                    {
                        ultimates.AddCharge(amount);
                    }
                }
            }
        }
    }
}
