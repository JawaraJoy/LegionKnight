using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class SpineUI : MonoBehaviour
    {
        [SerializeField]
        private SkeletonGraphic m_SkeletonGraphic;

        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnPlay;
        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnCompleted;
        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnPause;
        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnResume;
        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnEventTriggered;
        public void Play(SpineAnimDefinition anim)
        {
            anim.PlayUI(m_SkeletonGraphic, ()=> OnCompleteInvoke(anim));
            OnPlayInvoke(anim);
            if (anim.EventName == string.Empty) return;
            AddEventCallBack(anim);
        }
        public void PauseUI(SpineAnimDefinition anim)
        {
            anim.PauseUI(m_SkeletonGraphic);
            m_OnPause?.Invoke(anim);
        }
        public void ResumeUI(SpineAnimDefinition anim)
        {
            anim.ResumeUI(m_SkeletonGraphic);
            m_OnResume?.Invoke(anim);
        }

        private void AddEventCallBack(SpineAnimDefinition anim)
        {
            anim.AddEventCallBack(m_SkeletonGraphic, OnEventTriggered);
        }

        private void OnEventTriggered(SpineAnimDefinition anim)
        {
            m_OnEventTriggered?.Invoke(anim);
        }
        private void OnPlayInvoke(SpineAnimDefinition anim)
        {
            m_OnPlay?.Invoke(anim);
        }
        private void OnCompleteInvoke(SpineAnimDefinition anim)
        {
            m_OnCompleted?.Invoke(anim);
        }
    }
}
