using UnityEngine;

namespace Rush
{
    public interface ILifetimeable
    {
        float Lifetime { get; }
        void SetLifetime(float lifetime);
    }
}
