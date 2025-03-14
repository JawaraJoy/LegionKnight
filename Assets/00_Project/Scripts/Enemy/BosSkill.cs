using UnityEngine;

namespace LegionKnight
{
    public partial class BosSkill : PassiveSkill // The Boss Component
    {
        
    }

    public partial class BosEnemy // The Core Object
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
        public void AddManaToAll(int add)
        {
            m_BosSkill.AddManaToAll(add);
        }
    }
    public partial class LevelHandler // the Boss Spawn Handler
    {
        public void AddOneMana(int indexSkill)
        {
            m_SpawnedBosEnemy.AddOneMana(indexSkill);
        }
        public void ResetMana(int indexSkill)
        {
            m_SpawnedBosEnemy.ResetMana(indexSkill);
        }
        public void AddManaToAllBosSkill(int add)
        {
            m_SpawnedBosEnemy.AddManaToAll(add);
        }
    }
    public partial class GameManager // The Game Manager who handle Level
    {
        public void AddBosOneMana(int indexSkill)
        {
            m_LevelManager.AddOneMana(indexSkill);
        }
        public void ResetBosMana(int indexSkill)
        {
            m_LevelManager.ResetMana(indexSkill);
        }
        public void AddManaToAllBosSkill(int add)
        {
            m_LevelManager.AddManaToAllBosSkill(add);
        }
    }
    public partial class LevelManagerAgent // The Component Accessor to GameManager
    {
        public void AddBosOneMana(int indexSkill)
        {
            GameManager.Instance.AddBosOneMana(indexSkill);
        }
        public void ResetBosMana(int indexSkill)
        {
            GameManager.Instance.ResetBosMana(indexSkill);
        }
        public void AddManaToAllBosSkill(int add)
        {
            GameManager.Instance.AddManaToAllBosSkill(add);
        }
    }
}
