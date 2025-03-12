using UnityEngine;

namespace LegionKnight
{
    public partial class BosSkill : PassiveSkill
    {
        
    }

    public partial class BosEnemy
    {
        [SerializeField]
        private BosSkill m_BosSkill;

        public void AddOneMana(int indexSkill)
        {
            m_BosSkill.AddOneMana(indexSkill);
        }
        public void ResetMana(int indexSkill)
        {
            m_BosSkill.ResetMana(indexSkill);
        }
    }
    public partial class LevelHandler
    {
        public void AddOneMana(int indexSkill)
        {
            m_SpawnedBosEnemy.AddOneMana(indexSkill);
        }
        public void ResetMana(int indexSkill)
        {
            m_SpawnedBosEnemy.ResetMana(indexSkill);
        }
    }
    public partial class GameManager
    {
        public void AddBosOneMana(int indexSkill)
        {
            m_LevelManager.AddOneMana(indexSkill);
        }
        public void ResetBosMana(int indexSkill)
        {
            m_LevelManager.ResetMana(indexSkill);
        }
    }
    public partial class LevelManagerAgent
    {
        public void AddBosOneMana(int indexSkill)
        {
            GameManager.Instance.AddBosOneMana(indexSkill);
        }
        public void ResetBosMana(int indexSkill)
        {
            GameManager.Instance.ResetBosMana(indexSkill);
        }
    }
}
