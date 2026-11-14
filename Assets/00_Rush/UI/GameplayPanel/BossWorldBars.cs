using UnityEngine;

namespace Rush
{
    // Leave combat bindings, shield/barrier effects and other world indicators intact.
    public sealed class BossWorldBars : MonoBehaviour
    {
        [SerializeField] private CanvasGroup[] m_DuplicateBars;
        [SerializeField] private CategorySkillController m_Ultimates;
        public Skill QueuedUltimate => m_Ultimates != null ? m_Ultimates.CurrentSkill : null;

        public void SetHudVisible(bool visible)
        {
            foreach (var bar in m_DuplicateBars) if (bar != null) bar.alpha = visible ? 0f : 1f;
        }
        private void OnDisable() => SetHudVisible(false);
    }
}
