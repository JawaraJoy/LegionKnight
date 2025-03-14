using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerPassiveSkill : PassiveSkill
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerPassiveSkill m_PassiveSkill;
        public void InitPassive(CharacterDefinition definition)
        {
            m_PassiveSkill.Init(definition.Passives);
        }
        public void AddManaToAll(int add)
        {
            m_PassiveSkill.AddManaToAll(add);
        }
        public void ActiveSkillProjectileAbility(string abilityName)
        {
            m_PassiveSkill.ActiveProjectileAbility(abilityName);
        }
        public void AddSkillProjectileAbilities(ProjectileAbility ability)
        {
            m_PassiveSkill.AddProjectileAbilities(ability);
        }
        public void RemoveSkillProjectileAbilities(ProjectileAbility ability)
        {
            m_PassiveSkill.RemoveProjectileAbilities(ability);
        }
        public void SetPassiveCanActive(bool set)
        {
            m_PassiveSkill.SetCanActive(set);
        }
    }
    public partial class PlayerAgent
    {
        public void ActiveSkillProjectileAbility(string abilityName)
        {
            Player.Instance.ActiveSkillProjectileAbility(abilityName);
        }
        public void SetPassiveCanActive(bool set)
        {
            Player.Instance.SetPassiveCanActive(set);
        }
    }
}
