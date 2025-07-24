using NaughtyAttributes;
using Spine.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class SpineObject : View
    {
        private CharacterDefinition m_Defi;
        [SerializeField]
        private SkeletonAnimation m_SkeletonAnimation;

        private bool m_Initialized = false;

        public bool Initialized => m_Initialized;
        public CharacterDefinition Defi => m_Defi;
        public SkeletonAnimation SkeletonAnimation
        {
            get
            {
                if (m_SkeletonAnimation == null)
                {
                    m_SkeletonAnimation = GetComponent<SkeletonAnimation>();
                }
                return m_SkeletonAnimation;
            }
        }

        [ContextMenu("Initialize Spine")]
        public void InitSpine(CharacterDefinition defi)
        {
            m_SkeletonAnimation.Initialize(true);
            m_Defi = defi;
            m_Initialized = m_Defi != null && m_SkeletonAnimation != null;
        }

        public void PlayJump()
        {
            if (!m_Initialized) return;
            m_SkeletonAnimation.state.SetAnimation(0, "Jump", false);
        }
        public void PlayIdle()
        {
            if (!m_Initialized) return;
            m_SkeletonAnimation.state.SetAnimation(0, "Idle", true);
        }
        public void PlayAttack()
        {
            if (!m_Initialized) return;
            m_SkeletonAnimation.state.SetAnimation(0, "Attack", false);
        }
        public void PlayDeath()
        {
            if (!m_Initialized) return;
            m_SkeletonAnimation.state.SetAnimation(0, "Death", false);
        }
    }
}
