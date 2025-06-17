using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Heal : MonoBehaviour
    {
        [SerializeField]
        private SkillOwner m_SkillOwner = SkillOwner.Player; // Owner of the skill, e.g., Player or Enemy
        [SerializeField]
        private UnityEvent<int> m_OnHealApplied = new(); // Event to notify when healing is applied
        public void ApplyHeal(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning("Heal amount must be greater than zero.");
                return;
            }
            ApplyHealInternal(amount);
        }
        private void ApplyHealInternal(int amount)
        {
            switch (m_SkillOwner)
            {
                case SkillOwner.Player:
                    Player.Instance.Heal(amount);
                    break;
                case SkillOwner.Boss:
                    BosEnemy bosEnem = GameManager.Instance.SpawnedBosenemy;
                    bosEnem.Heal(amount);
                    break;
                default:
                    Debug.LogWarning("Unknown skill owner type.");
                    break;
            }
        }
    }
}
