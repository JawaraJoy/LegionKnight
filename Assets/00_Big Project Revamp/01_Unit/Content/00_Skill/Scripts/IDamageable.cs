

namespace Rush
{
    public interface IDamageable : IHealthMod, IDefenseMod
    {
        int Shield { get; }
        int Barrier { get; }
        int CurrentDamageTaken { get; }
        float DamageReductionRate { get; }
        int TotalDamageTaken { get; }
        int RemainingReborn {  get; }
        bool IsImmortal { get; }
        void TakeDamage(IAttacker attacker);
        void Heal(IHealer healer);
    }
}
