using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Targetable : Bindable
    {
        [SerializeField, MMReadOnly]
        private bool m_IsAlive = true;
        [SerializeField, MMReadOnly]
        private bool m_IsTargeted = false;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnNotified;
        public bool IsTargeted => m_IsTargeted;
        public bool IsAlive => m_IsAlive;
        public void SetTargeted(bool targeted)
        {
            m_IsTargeted = targeted;
        }
        public void SetAlive(bool alive)
        {
            m_IsAlive = alive;
        }
        public void Notify(AbilityContext context)
        {
            m_OnNotified?.Invoke(context);
        }
        /// <summary>
        /// Rotates spawn point to face target in 2D (XY plane, Z-axis rotation only).
        /// </summary>
        public static void LookAtFirstTarget2D(Transform subject, Targetable targetable)
        {
            if (targetable == null)
                return;

            Vector2 direction = targetable.transform.position - subject.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            subject.rotation = Quaternion.Euler(0f, 0f, angle);
        }

    }
}
