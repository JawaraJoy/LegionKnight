using UnityEngine;

namespace Rush
{
    public interface ISkillContext : IInitialiazable, IHasModuleContext
    {
        Skill Skill { get; }
    }
}
