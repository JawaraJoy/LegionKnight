using UnityEngine;

namespace LegionKnight
{
    public class BosSkillContainerAgent : MonoBehaviour
    {
        private ProjectileAbility m_Ability;
        private GameplayPanel GetGameplayPanel()
        {
            return CanvasManager.Instance.GetPanel<GameplayPanel>();
        }
        private ProjectileAbility GetProjectileAbility()
        {
            if (m_Ability == null)
            {
                return m_Ability;
            }
            BosSkill bossSkill = GameManager.Instance.GetSpawnedBosEnemy().BosSkill;
            if (bossSkill == null)
            {
                Debug.LogWarning("BosSkill is null.");
                return null;
            }
            else
            {
                if (bossSkill.TryGetComponent(out ProjectileAbility ability))
                {
                    m_Ability = ability;
                }
                else
                {
                    Debug.LogWarning("ProjectileAbility component not found on BosSkill.");
                    return null;
                }
            }
            return m_Ability;
        }
        public void ActivateAbility(string abilityName)
        {
            ProjectileAbility ability = GetProjectileAbility();
            if (ability != null)
            {
                ability.TriggerAbility();
            }
            else
            {
                Debug.LogWarning("Cannot activate ability. ProjectileAbility is null.");
            }
        }
        private BosSkillContainer GetBosSkillContainer()
        {
            BosSkillContainer skillContainer = GetGameplayPanel().GetBosSkillContainer();
            return skillContainer;
        }

        public void InitSkillContainer()
        {
            GetBosSkillContainer().Init();
        }
        public void InitBos(BosDefinition definition)
        {
            GetBosSkillContainer().Init(definition);
        }
    }

    public partial class GameplayPanel
    {
        public BosSkillContainer GetBosSkillContainer()
        {
            return GetBinding<BosSkillContainer>();
        }
    }
}
