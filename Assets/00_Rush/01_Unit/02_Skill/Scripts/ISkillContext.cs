using UnityEngine;

namespace Rush
{
    public interface ISkillContext : IInitialiazable, IHasModuleContext
    {
        ISkill Skill { get; }
    }
}
