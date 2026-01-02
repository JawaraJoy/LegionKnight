using UnityEngine;

namespace Rush
{
    public interface IDistanceable 
    {
        float MaxDistance { get; }
        void SetMaxDistance(float distance);
    }
}
