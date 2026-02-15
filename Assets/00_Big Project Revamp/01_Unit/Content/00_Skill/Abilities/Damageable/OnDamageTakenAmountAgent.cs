using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class OnDamageTakenAmountAgent : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<int> m_OnDamageTaken;

        public void OnDamageTakenInvoke(BattleContext context)
        {
            if (context.Damageable == null) return;
            m_OnDamageTaken?.Invoke(context.Damageable.DamageableField.CurrentDamageTaken);
        }
    }
}
