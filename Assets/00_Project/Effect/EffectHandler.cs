using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class EffectHandler : MonoBehaviour
    {
        [SerializeField]
        private List<EffectAnimator> m_EffectAnimators = new();
        public void AddEffectAnimator(EffectAnimator animator)
        {
            if (animator != null && !m_EffectAnimators.Contains(animator))
            {
                m_EffectAnimators.Add(animator);
            }
            else
            {
                Debug.LogWarning("EffectAnimator is null or already added.");
            }
        }
        public void RemoveEffectAnimator(EffectAnimator animator)
        {
            if (animator != null && m_EffectAnimators.Contains(animator))
            {
                m_EffectAnimators.Remove(animator);
            }
            else
            {
                Debug.LogWarning("EffectAnimator is null or not found in the list.");
            }
        }
        private EffectAnimator GetEffect(EffectDefinition defi)
        {
            foreach (var effectAnimator in m_EffectAnimators)
            {
                if (effectAnimator.Definition == defi)
                {
                    return effectAnimator;
                }
            }
            Debug.LogWarning($"EffectDefinition {defi.name} not found in EffectAnimators.");
            return null;
        }
        public void PlayEffect(EffectDefinition defi, string animationName)
        {
            var effectAnimator = GetEffect(defi);
            if (effectAnimator != null)
            {
                effectAnimator.PlayAnimation(animationName);
            }
        }
        public void PauseEffect(EffectDefinition defi)
        {
            var effectAnimator = GetEffect(defi);
            if (effectAnimator != null)
            {
                effectAnimator.PauseAnimation();
            }
        }
        public void ResumeEffect(EffectDefinition defi)
        {
            var effectAnimator = GetEffect(defi);
            if (effectAnimator != null)
            {
                effectAnimator.ResumeAnimation();
            }
        }
        public void SpeedUpEffect(EffectDefinition defi)
        {
            var effectAnimator = GetEffect(defi);
            if (effectAnimator != null)
            {
                effectAnimator.SpeedUpAnimation();
            }
        }
        public void ResetSpeed(EffectDefinition defi)
        {
            var effectAnimator = GetEffect(defi);
            if (effectAnimator != null)
            {
                effectAnimator.ResetSpeed();
            }
        }
    }
}
