using UnityEngine;

namespace LegionKnight
{
    public class EffectAnimator : MonoBehaviour
    {
        [SerializeField]
        private EffectDefinition m_Definition;
        [SerializeField]
        private Animator m_Animator;
        [SerializeField]
        private float m_DefaultSpeed = 1.0f;
        [SerializeField]
        private bool m_AutoPlay = false;

        [SerializeField]
        private float m_SpeedUp = 2.0f;

        public EffectDefinition Definition => m_Definition;
        public float DefaultSpeed => m_DefaultSpeed;
        public float SpeedUp => m_SpeedUp;


        private void Start()
        {
            // pause on start
            if (!m_AutoPlay)
            {
                PauseAnimationInternal();
            }
        }

        private void OnEnable()
        {
            GameManager.Instance.AddEffectAnimator(this);
        }
        private void OnDisable()
        {
            GameManager.Instance.RemoveEffectAnimator(this);
        }

        public void PlayAnimation(string animationName)
        {
            if (m_Animator != null)
            {
                m_Animator.speed = m_DefaultSpeed;
                m_Animator.Play(animationName);
            }
            else
            {
                Debug.LogWarning("Animator is not assigned.");
            }
        }
        public void PauseAnimationInternal()
        {
            if (m_Animator != null)
            {
                m_Animator.speed = 0;
            }
            else
            {
                Debug.LogWarning("Animator is not assigned.");
            }
        }
        public void PauseAnimation()
        {
            PauseAnimationInternal();
        }
        public void ResumeAnimation()
        {
            if (m_Animator != null)
            {
                m_Animator.speed = m_DefaultSpeed;
            }
            else
            {
                Debug.LogWarning("Animator is not assigned.");
            }
        }
        public void SpeedUpAnimation()
        {
            if (m_Animator != null)
            {
                m_Animator.speed = m_SpeedUp;
            }
            else
            {
                Debug.LogWarning("Animator is not assigned.");
            }
        }
        public void ResetSpeed()
        {
            if (m_Animator != null)
            {
                m_Animator.speed = m_DefaultSpeed;
            }
            else
            {
                Debug.LogWarning("Animator is not assigned.");
            }
        }
    }
}
