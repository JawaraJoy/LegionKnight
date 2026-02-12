using UnityEngine;

namespace Rush
{
    public interface IAttacker 
    {
        int GetFinalDamage(Damageable damageable);
        bool FatalDamage { get; }
        bool TrueDamage { get; }
    }
}
