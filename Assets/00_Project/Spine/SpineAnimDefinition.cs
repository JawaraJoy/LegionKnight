using Spine.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Spine Anim", menuName = "Legion Knight/Spine/Spine Anim", order = 1)]
    public class SpineAnimDefinition : ScriptableObject
    {
        [SerializeField]
        private int m_AnimTrack;
        [SerializeField]
        private string m_AnimName;
        [SerializeField]
        private bool m_Loop;

        [SerializeField]
        private SpineAnimDefinition m_NextAnim;
        [SerializeField]
        private float m_NextAnimDelay = 0f;

        public int AnimTrack => m_AnimTrack;
        public string AnimName => m_AnimName;
        public bool Loop => m_Loop;
        public SpineAnimDefinition NextAnim => m_NextAnim;
        public void Play(SkeletonAnimation skeletonAnimation, UnityAction callback = null)
        {
            if (skeletonAnimation == null) return;
            var aa = skeletonAnimation.state.SetAnimation(m_AnimTrack, m_AnimName, m_Loop);
            float animationTime = aa.AnimationTime;
            float animationDuration = aa.Animation.Duration;
            aa.Complete += (trackEntry) =>
            {
                callback?.Invoke();
                if (m_NextAnim != null)
                {
                    skeletonAnimation.StartCoroutine(PlayNext(skeletonAnimation, m_NextAnimDelay));
                }
            };
        }

        private IEnumerator PlayNext(SkeletonAnimation anim, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (m_NextAnim != null)
            {
                m_NextAnim.Play(anim);
            }
        }
    }
}
