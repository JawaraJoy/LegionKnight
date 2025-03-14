using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerWeaponer : PassiveSkill
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerWeaponer m_Weaponer;

        public Transform SkillSpawnPost => m_Weaponer.transform;
        public void InitWeaponer(CharacterDefinition definition)
        {
            m_Weaponer.Init(definition.Weapons);
        }
        public void AddManaToAllWeapon(int add)
        {
            m_Weaponer.AddManaToAll(add);
        }
        public void ForceActivated(int indexSkill)
        {
            m_Weaponer.ForceActivated(indexSkill);
        }
        public void ForceActivated(string skillName)
        {
            m_Weaponer.ForceActivated(skillName);
        }
        public void ActiveWeaponProjectileAbility(string abilityName)
        {
            m_Weaponer.ActiveProjectileAbility(abilityName);
        }
        public void AddWeaponProjectileAbilities(ProjectileAbility ability)
        {
            m_Weaponer.AddProjectileAbilities(ability);
        }
        public void RemoveWeaponProjectileAbilities(ProjectileAbility ability)
        {
            m_Weaponer.RemoveProjectileAbilities(ability);
        }
        public void SetWeaponCanActive(bool set)
        {
            m_PassiveSkill.SetCanActive(set);
        }
    }
    public partial class PlayerAgent
    {
        public void ActiveWeaponProjectileAbility(string abilityName)
        {
            Player.Instance.ActiveWeaponProjectileAbility(abilityName);
        }
        public void SetWeaponCanActive(bool set)
        {
            Player.Instance.SetWeaponCanActive(set);
        }
    }
}
