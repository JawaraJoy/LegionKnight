using UnityEngine;

namespace Rush
{
    public interface IHealthMod : IHasMaxHealth
    {
        float HealthRate { get; }
        void SetHealth(int val);
        void SetMaxHealth(int val);
        void AddHealth(int val);
        void AddMaxHealth(int val, bool withCurrentHealth);
        void MultiplyHealth(int val);
        void MultiplyMaxHealth(int val, bool withCurrentHealth);
    }
    public interface IHasHealth
    {
        int Health { get; }
    }
    public interface IHasMaxHealth : IHasHealth
    {
        int MaxHealth { get; }
    }
}
