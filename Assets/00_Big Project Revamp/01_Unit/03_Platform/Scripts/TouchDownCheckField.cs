using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [System.Serializable]
    public class TouchDownCheckField
    {
        [SerializeField, MMReadOnly]
        private bool m_IsStayPerfect;
        [SerializeField, MMReadOnly]
        private int m_StayPerfectCount;
        [SerializeField]
        private UnityEvent<bool,ISkillContext> m_OnTouchDown;
        [SerializeField]
        private UnityEvent<ISkillContext> m_OnNormalTouchDown;
        [SerializeField]
        private UnityEvent<ISkillContext> m_OnPerfectTouchDown;
        [SerializeField]
        private UnityEvent<int> m_OnStayPerfectCountChange;
        public bool IsStayPerfect => m_IsStayPerfect;
        public int StayPerfectCount => m_StayPerfectCount;
        private void SetIsStayPerfectInternal(bool value, ISkillContext context)
        {
            m_IsStayPerfect = value;
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
            m_OnTouchDown.Invoke(value, context);
        }
        public void SetIsStayPerfect(bool value, ISkillContext context)
        {
            SetIsStayPerfectInternal(value, context);
        }
        private void AddStayPerfectCountInternal(int add)
        {
            m_StayPerfectCount += add;
        }
        private void SetStayPerfectCountInternal(int value)
        {
            m_StayPerfectCount = value;
        }
        private void OnNormalTouchDownInvoke(ISkillContext context)
        {
            m_OnNormalTouchDown?.Invoke(context);
            
        }
        private void OnPerfectTouchDownInvoke(ISkillContext context)
        {
            m_OnPerfectTouchDown?.Invoke(context);
            m_OnStayPerfectCountChange?.Invoke(m_StayPerfectCount);
        }

        
    }
}
