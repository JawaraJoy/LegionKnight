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
        private UnityEvent<ISkillContext> m_OnTouchDown;
        [SerializeField]
        private UnityEvent<ISkillContext> m_OnNormalTouchDown;
        [SerializeField]
        private UnityEvent<ISkillContext> m_OnPerfectTouchDown;
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
            ForceSkillActivate(context, m_IsStayPerfect);
            m_OnTouchDown.Invoke(context);
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
        }

        private void ForceSkillActivate(ISkillContext context, bool isPerfect)
        {
            Unit unit = context.ModuleContext.Unit;
            if (unit.HasBind(out SkillController controller))
            {
                if (context.Skill is Platform2D platform)
                {
                    PlatformDirection direction = platform.Direction;
                    PlatformSkillField platformSkillField = new (null, null);
                    switch (direction)
                    {
                        case PlatformDirection.Left:
                            
                            platformSkillField = platform.PlatformConfig.LeftSkillField;
                            break;
                        case PlatformDirection.Right:
                            platformSkillField = platform.PlatformConfig.RightSkillField;
                            break;
                        default:
                            break;
                    }

                    SkillCategoryConfig skillCategory;
                    int skillCount;
                    if (isPerfect)
                    {
                        skillCount = platformSkillField.OnPerfectTouchSkill.Length;
                        if (skillCount == 0) return;

                        skillCategory = platformSkillField.OnPerfectTouchSkill[0].Category;
                    }
                    else
                    {
                        skillCount = platformSkillField.OnNormalTouchSkill.Length;
                        if (skillCount == 0) return;
                        skillCategory = platformSkillField.OnNormalTouchSkill[0].Category;
                    }
                    controller.ForceActiveByCategory(skillCategory);
                }
            }
        }
    }
}
