using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Heal : MonoBehaviour
    {
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
            m_OnHealApplied.Invoke(amount);
        }
    }
}
