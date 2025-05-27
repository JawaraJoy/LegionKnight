using UnityEngine;

namespace LegionKnight
{
    public class EffectManager : EffectHandler
    {
        
    }
    public partial class GameManager
    {
        [SerializeField]
        private EffectManager m_EffectManager;

        public void AddEffectAnimator(EffectAnimator animator)
        {
            if (m_EffectManager != null)
            {
                m_EffectManager.AddEffectAnimator(animator);
            }
            else
            {
                Debug.LogWarning("EffectManager is not assigned.");
            }
        }
        public void RemoveEffectAnimator(EffectAnimator animator)
        {
            if (m_EffectManager != null)
            {
                m_EffectManager.RemoveEffectAnimator(animator);
            }
            else
            {
                Debug.LogWarning("EffectManager is not assigned.");
            }
        }
        public void PlayEffect(EffectDefinition defi, string animationName)
        {
            if (m_EffectManager != null)
            {
                m_EffectManager.PlayEffect(defi, animationName);
            }
            else
            {
                Debug.LogWarning("EffectManager is not assigned.");
            }
        }
        public void PauseEffect(EffectDefinition defi)
        {
            if (m_EffectManager != null)
            {
                m_EffectManager.PauseEffect(defi);
            }
            else
            {
                Debug.LogWarning("EffectManager is not assigned.");
            }
        }
        public void ResumeEffect(EffectDefinition defi)
        {
            if (m_EffectManager != null)
            {
                m_EffectManager.ResumeEffect(defi);
            }
            else
            {
                Debug.LogWarning("EffectManager is not assigned.");
            }
        }
        public void SpeedUpEffect(EffectDefinition defi)
        {
            if (m_EffectManager != null)
            {
                m_EffectManager.SpeedUpEffect(defi);
            }
            else
            {
                Debug.LogWarning("EffectManager is not assigned.");
            }
        }
        public void ResetSpeed(EffectDefinition defi)
        {
            if (m_EffectManager != null)
            {
                m_EffectManager.ResetSpeed(defi);
            }
            else
            {
                Debug.LogWarning("EffectManager is not assigned.");
            }
        }

    }
}
