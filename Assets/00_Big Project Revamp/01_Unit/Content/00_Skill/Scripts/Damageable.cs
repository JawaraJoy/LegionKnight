using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Damageable : Bindable, IDamageable
    {
        [SerializeField]
        private DamageableField m_DamageableField;
        public DamageableField DamageableField =>  m_DamageableField;

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (TryGetComponent(out Targetable targetable))
            {
                m_DamageableField.OnTriggerEnter2D(collision, targetable);
            }
        }
    }
}
