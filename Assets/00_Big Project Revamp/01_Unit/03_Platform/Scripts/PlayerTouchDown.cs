using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class PlayerTouchDown : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private int calledCount;
        [SerializeField, MMReadOnly]
        private PlatformSkillField m_PlatformSkillField;
        public void ForceSkillActivate(bool isPerfect, ISkillContext context)
        {
            Unit unit = context.ModuleContext.Unit;
            if (unit.HasBind(out SkillController controller))
            {
                if (context.Skill is Platform2D platform)
                {
                    PlatformDirection direction = platform.Direction;
                    
                    switch (direction)
                    {
                        case PlatformDirection.Left:
                            m_PlatformSkillField = platform.PlatformConfig.LeftSkillField;
                            break;
                        case PlatformDirection.Right:
                            m_PlatformSkillField = platform.PlatformConfig.RightSkillField;
                            break;
                        default:
                            break;
                    }

                    SkillConfig[] skillConfigs;
                    int skillCount;
                    if (isPerfect)
                    {
                        skillCount = m_PlatformSkillField.OnPerfectTouchSkill.Length;
                        if (skillCount == 0) return;

                        skillConfigs = m_PlatformSkillField.OnPerfectTouchSkill;
                    }
                    else
                    {
                        skillCount = m_PlatformSkillField.OnNormalTouchSkill.Length;
                        if (skillCount == 0) return;
                        skillConfigs = m_PlatformSkillField.OnNormalTouchSkill;
                    }
                    foreach (var skill in skillConfigs)
                    {
                        controller.ForceActive(skill);
                    }
                }
            }
            calledCount++;
        }
    }
}
