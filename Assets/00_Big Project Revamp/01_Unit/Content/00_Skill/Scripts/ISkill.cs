using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public interface ISkill : IHasSkillContext, IHasProgress
    {
        SkillConfig SkillConfig { get; }
        UnityEvent<Unit> OnAbilityDelivered { get; }
        void ForceActivateAll();
        void Init(SkillConfig skillConfig, IModuleContext moduleContext);
    }
}
