using LegionKnight;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public interface IDamageable
    {
        int MaxHealth { get; }
        int Health { get; }
        int Barrier { get; }
        int RemainingReborn { get; }
        int Defense { get; }
        int Shield { get; }
        float DamageReductionRate { get; }
        int CurrentDamageTaken { get; }
        int TotalDamageTaken { get; }
        float CurrentHealthRate { get; }
        void TakeDamage(IAttacker attacker);
    }
}
