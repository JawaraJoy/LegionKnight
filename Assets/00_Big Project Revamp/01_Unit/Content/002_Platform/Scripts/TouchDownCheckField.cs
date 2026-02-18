using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [System.Serializable]
    public class TouchDownCheckField
    {
        [SerializeField, MMReadOnly]
        private bool m_OnStayPerfect;
        [SerializeField, MMReadOnly]
        private int m_OnStayPerfectCount;
        [SerializeField]
        private UnityEvent<ISkillContext> m_OnNormalTouchDown;
        [SerializeField]
        private UnityEvent<ISkillContext> m_OnPerfectTouchDown;
        public bool IsStayPerfect => m_OnStayPerfect;
        public int StayPerfectCount => m_OnStayPerfectCount;
        private void SetIsStayPerfectInternal(bool value, ISkillContext context)
        {
            m_OnStayPerfect = value;
            if (value)
            {
                AddStayPerfectCountInternal(1);
                OnPerfectTouchDownInvoke(context);
            }
            else
            {
                SetStayPerfectCountInternal(0);
                OnNormalTouchDownInvoke(context);
            }
        }
        public void SetIsStayPerfect(bool value, ISkillContext context)
        {
            SetIsStayPerfectInternal(value, context);
        }
        private void AddStayPerfectCountInternal(int add)
        {
            m_OnStayPerfectCount += add;
        }
        private void SetStayPerfectCountInternal(int value)
        {
            m_OnStayPerfectCount = value;
        }
        private void OnNormalTouchDownInvoke(ISkillContext context)
        {
            m_OnNormalTouchDown?.Invoke(context);
        }
        private void OnPerfectTouchDownInvoke(ISkillContext context)
        {
            m_OnPerfectTouchDown?.Invoke(context);
        }
    }
}
