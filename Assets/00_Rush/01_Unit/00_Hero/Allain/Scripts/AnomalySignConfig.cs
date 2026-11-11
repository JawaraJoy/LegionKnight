using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(menuName = "Rush/Combat/Allain/Anomaly Sign")]
    public class AnomalySignConfig : StatusEffectConfig
    {
        [SerializeField] private AnomalyPulseConfig m_Collapse;
        public AnomalyPulseConfig Collapse => m_Collapse;
        public override void OnEffectStarted(StatusEffectContext context) { }
        public override void OnStackAdded(StatusEffectContext context) => ApplyStackInternal(context);
        public override void OnStackRemoved(StatusEffectContext context) { }
        public override void OnEffectEnded(StatusEffectContext context) { }

        private void ApplyStackInternal(StatusEffectContext context)
        {
            if (context.CurrentStack < MaxStackCount || m_Collapse == null || context.Infected == null) return;
            var pulse = Instantiate(m_Collapse.DeliverPrefab, context.Infected.transform.position, Quaternion.identity);
            pulse.Init(m_Collapse, context.AbilityContext.SkillContext);
            pulse.Activate();
        }
    }
}
