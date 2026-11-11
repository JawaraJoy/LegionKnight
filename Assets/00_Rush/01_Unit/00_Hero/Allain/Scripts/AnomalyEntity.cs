using UnityEngine;

namespace Rush
{
    public class AnomalyEntity : MonoBehaviour, IUnitExtension
    {
        [SerializeField] private AnimationClipConfig m_IdleClip;
        [SerializeField] private AnimationClipConfig m_DeathClip;
        private AnomalyEntityField m_Setup;
        private ISkillContext m_Owner;
        private Damageable m_Damageable;
        private ModuleContext m_ModuleContext;
        private bool m_Exploded;
        public IModuleContext ModuleContext => m_ModuleContext;
        public void Configure(AnomalyEntityField setup, ISkillContext owner) => ConfigureInternal(setup, owner);
        private void ConfigureInternal(AnomalyEntityField setup, ISkillContext owner)
        { m_Setup = setup; m_Owner = owner; }
        public void Init(Unit unit) => InitInternal(unit);
        private void InitInternal(Unit unit)
        {
            m_ModuleContext = new ModuleContext(unit, gameObject);
            m_Exploded = false;
            if (!unit.HasBind(out m_Damageable)) return;
            m_Damageable.AddBarrier(m_Setup.Barriers - m_Damageable.Barrier, false);
            m_Damageable.OnDeath.AddListener(OnDeathInternal);
            if (unit.HasBind(out AvatarSpine avatar) && m_IdleClip != null)
                avatar.PlayClipInterrupt(m_IdleClip);
            foreach (var collider in GetComponents<Collider2D>()) collider.enabled = true;
            foreach (var collider in GetComponents<Collider>()) collider.enabled = true;
        }
        private void OnDeathInternal(IAbilityContext killingContext)
        {
            if (m_Exploded) return;
            m_Exploded = true;
            var config = m_Setup.DeathExplosion;
            if (config != null && m_Owner?.ModuleContext.Unit != null)
            {
                var pulse = Instantiate(config.DeliverPrefab, transform.position, Quaternion.identity);
                pulse.Init(config, m_Owner);
                pulse.Activate();
            }
            foreach (var collider in GetComponents<Collider2D>()) collider.enabled = false;
            foreach (var collider in GetComponents<Collider>()) collider.enabled = false;
            if (m_ModuleContext.Unit.HasBind(out AvatarSpine avatar) && m_DeathClip != null)
            {
                avatar.PlayClipInterrupt(m_DeathClip);
                var animation = avatar.SkeletonAnimation.Skeleton.Data.FindAnimation(m_DeathClip.BaseInfo.Name);
                Destroy(gameObject, animation == null ? 0f : animation.Duration / Mathf.Max(Mathf.Epsilon, m_DeathClip.SpeedRate));
            }
            else Destroy(gameObject);
        }
        private void OnDisable()
        {
            if (m_Damageable != null) m_Damageable.OnDeath.RemoveListener(OnDeathInternal);
        }
    }
}
