using UnityEngine;

namespace Rush
{
    public interface IAbility
    {
        bool Initialized { get; }
        void Init(AbilityContext context);
    }
}
