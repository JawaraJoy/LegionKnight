using UnityEngine;

namespace Rush
{
    public interface ISpeedable
    {
        float Speed { get; }
        void SetSpeed(float speed);
    }
}
