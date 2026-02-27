using UnityEngine;

namespace Rush
{
    public interface ITargetable : IUnitExtension
    {
        void Notify(AbilityContext context);
        void SetTargeted(bool targeted);
        bool IsTargeted { get; }
        bool IsAlive {  get; }
        Transform TargetTransform { get; }
    }
}
