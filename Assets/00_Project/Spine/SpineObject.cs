using NaughtyAttributes;
using Spine;
using Spine.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class SpineObject : View
    {
        private ScriptableObject m_Defi;
        [SerializeField]
        private SkeletonAnimation m_SkeletonAnimation;

        private bool m_Initialized = false;

        public bool Initialized => m_Initialized;
        public ScriptableObject Defi => m_Defi;

        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnSetAnim = new ();
        [SerializeField]
        private UnityEvent<SpineObject> m_OnAnimationDone = new ();

        [SerializeField]
        private SpineEvent[] m_SpineEvents;

        private SpineEvent GetSpineEvent(SpineAnimDefinition defi)
        {
            foreach (var spineEvent in m_SpineEvents)
            {
                if (spineEvent.Definition == defi)
                {
                    return spineEvent;
                }
            }
            return null;
        }
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
        public void InitCharSpine(ScriptableObject defi)
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
            m_SkeletonAnimation.state.AddAnimation(0, "Idle", true, 0f);
        }
        public void PlayDeath()
        {
            if (!m_Initialized) return;
            m_SkeletonAnimation.state.SetAnimation(0, "Fall", false);
            //m_SkeletonAnimation.state.AddAnimation(0, "Idle", true, 0f);
        }
        public void FlipX(bool left)
        {
            if (!m_Initialized) return;
            m_SkeletonAnimation.initialFlipX = left;
        }
        public void PlayAnimationOnce(string key)
        {
            if (!m_Initialized) return;
            m_SkeletonAnimation.state.SetAnimation(0, key, false);
        }
        public void SetAnim(SpineAnimDefinition anim)
        {
            //if (!m_Initialized) return;
            if (anim == null) return;
            anim.Play(m_SkeletonAnimation, () => OnAnimationDone(anim));
            m_OnSetAnim.Invoke(anim);
            Debug.Log($"Animation set for {anim.AnimName}");
            var spineEvent = GetSpineEvent(anim);
            spineEvent?.OnStart.Invoke();
        }

        private void OnAnimationDone(SpineAnimDefinition anim)
        {
            m_OnAnimationDone.Invoke(this);
            Debug.Log($"Animation done for {anim.AnimName}");
            var spineEvent = GetSpineEvent(anim);
            spineEvent?.OnEnd.Invoke();
        }
    }
}
