using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class ProjectileAbility : MonoBehaviour
    {
        [SerializeField]
        private string m_AbilityName;
        [SerializeField]
        private SkillOwner m_AbilityOwner = SkillOwner.Player;
        [SerializeField]
        private UnityEvent m_OnAbilityTriggered = new();
        public string AbilityName => m_AbilityName;
        private void Start()
        {
            AddAbilitiesInternal();
        }
        private void OnDestroy()
        {
            RemoveAbilitiesInternal();
        }

        private void AddAbilitiesInternal()
        {
            switch (m_AbilityOwner)
            {
                case SkillOwner.Player:
                    Player.Instance.AddWeaponProjectileAbilities(this);
                    Player.Instance.AddSkillProjectileAbilities(this);
                    break;
                case SkillOwner.Boss:
                    GameManager.Instance.AddBosProjectileAbilities(this);
                    break;
            }
        }
        private void RemoveAbilitiesInternal()
        {
            switch (m_AbilityOwner)
            {
                case SkillOwner.Player:
                    Player.Instance.RemoveWeaponProjectileAbilities(this);
                    Player.Instance.RemoveSkillProjectileAbilities(this);
                    break;
                case SkillOwner.Boss:
                    GameManager.Instance.RemoveBosProjectileAbilities(this);
                    break;
            }
        }
        public void TriggerAbility()
        {
            m_OnAbilityTriggered?.Invoke();
        }
    }
}
