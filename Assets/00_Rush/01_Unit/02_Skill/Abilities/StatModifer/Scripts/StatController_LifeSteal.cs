using UnityEngine;

namespace Rush
{
    public partial class StatController
    {
        public float LifeStealRate
        {
            get
            {
                float total = 0f;
                foreach (var modifier in m_Modifiers)
                {
                    if (modifier == null || !modifier.IsActive ||
                        !(modifier.Config is ILifeStealModifier lifeSteal)) continue;
                    int skillLevel = modifier.Context.AbilityContext.SkillContext.Skill.Progression.Level;
                    total += lifeSteal.LifeStealPerStackAtLevel(skillLevel) *
                        Mathf.Clamp(modifier.StackCount, 0, modifier.Config.MaxStackCount);
                }
                return Mathf.Clamp01(total);
            }
        }
    }
}
