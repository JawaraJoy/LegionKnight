using Spine;
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

        [SerializeField]
        private string m_EventName;

        private UnityEvent<SpineAnimDefinition> m_ActionOnEvent;

        public int AnimTrack => m_AnimTrack;
        public string AnimName => m_AnimName;
        public bool Loop => m_Loop;
        public string EventName => m_EventName;
        public SpineAnimDefinition NextAnim => m_NextAnim;
        public void Play(SkeletonAnimation skeletonAnimation, UnityAction callback = null)
        {
            if (skeletonAnimation == null) return;
            Spine.Animation animData = skeletonAnimation.skeletonDataAsset.GetAnimationStateData().SkeletonData.FindAnimation(m_AnimName);
            if (animData == null) return;
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

        public void PlayUI(SkeletonGraphic skeletonAnimation, UnityAction onComplete = null)
        {
            if (skeletonAnimation == null) return;
            Spine.Animation animData = skeletonAnimation.skeletonDataAsset.GetAnimationStateData().SkeletonData.FindAnimation(m_AnimName);
            if (animData == null) return;
            var aa = skeletonAnimation.AnimationState.SetAnimation(m_AnimTrack, m_AnimName, m_Loop);
            float animationTime = aa.AnimationTime;
            float animationDuration = aa.Animation.Duration;
            aa.Complete += (trackEntry) =>
            {
                onComplete?.Invoke();
                if (m_NextAnim != null)
                {
                    skeletonAnimation.StartCoroutine(PlayNextUI(skeletonAnimation, m_NextAnimDelay));
                }
            };
        }
        private IEnumerator PlayNextUI(SkeletonGraphic anim, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (m_NextAnim != null)
            {
                m_NextAnim.PlayUI(anim);
            }
        }

        public void PauseUI(SkeletonGraphic anim)
        {
            anim.timeScale = 0f;
        }
        public void ResumeUI(SkeletonGraphic anim)
        {
            anim.timeScale = 1f;
        }

        public void AddEventCallBack(SkeletonGraphic anim, UnityAction<SpineAnimDefinition> onEventTriggered)
        {
            m_ActionOnEvent.RemoveAllListeners();
            anim.AnimationState.Event += HandleSpineEvent;
            m_ActionOnEvent.AddListener(onEventTriggered);
        }

        private void HandleSpineEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == m_EventName)
            {
                m_ActionOnEvent?.Invoke(this);
            }
        }
    }
}
