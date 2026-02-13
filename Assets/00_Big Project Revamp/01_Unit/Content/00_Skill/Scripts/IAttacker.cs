using UnityEngine;

namespace Rush
{
    public interface IAttacker 
    {
        int Damage { get; }
        bool IsTrueDamage { get; }
        bool IsFatalDamage { get; }
        float DamageBasedTargetMaxHP { get; }
    }
}
