using System.Collections.Generic;
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
        public Transform SkillSpawnPost => m_BosSkill.transform;
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
        public void ActiveProjectileAbility(string abilityName)
        {
            m_BosSkill.ActiveProjectileAbility(abilityName);
        }
        public void AddProjectileAbilities(ProjectileAbility ability)
        {
            m_BosSkill.AddProjectileAbilities(ability);
        }
        public void RemoveProjectileAbilities(ProjectileAbility ability)
        {
            m_BosSkill.RemoveProjectileAbilities(ability);
        }
    }
    public partial class LevelHandler // the Boss Spawn Handler
    {
        public void AddOneMana(int indexSkill)
        {
            if (m_SpawnedBosEnemy == null) return;
            m_SpawnedBosEnemy.AddOneMana(indexSkill);
        }
        public void ResetMana(int indexSkill)
        {
            if (m_SpawnedBosEnemy == null) return;
            m_SpawnedBosEnemy.ResetMana(indexSkill);
        }
        public void AddManaToAllBosSkill(int add)
        {
            if (m_SpawnedBosEnemy == null) return;
            m_SpawnedBosEnemy.AddManaToAll(add);
        }
        public void ActiveBosProjectileAbility(string abilityName)
        {
            if (m_SpawnedBosEnemy == null) return;
            m_SpawnedBosEnemy.ActiveProjectileAbility(abilityName);
        }
        public void AddBosProjectileAbilities(ProjectileAbility ability)
        {
            if (m_SpawnedBosEnemy == null) return;
            m_SpawnedBosEnemy.AddProjectileAbilities(ability);
        }
        public void RemoveBosProjectileAbilities(ProjectileAbility ability)
        {
            if (m_SpawnedBosEnemy == null) return;
            m_SpawnedBosEnemy.RemoveProjectileAbilities(ability);
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
        public void ActiveBosProjectileAbility(string abilityName)
        {
            m_LevelManager.ActiveBosProjectileAbility(abilityName);
        }
        public void AddBosProjectileAbilities(ProjectileAbility ability)
        {
            m_LevelManager.AddBosProjectileAbilities(ability);
        }
        public void RemoveBosProjectileAbilities(ProjectileAbility ability)
        {
            m_LevelManager.RemoveBosProjectileAbilities(ability);
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
        public void ActiveBosProjectileAbility(string abilityName)
        {
            GameManager.Instance.ActiveBosProjectileAbility(abilityName);
        }
    }
}
