using UnityEngine;

namespace Rush
{
    public abstract class SummonShape : Configuration
    {
        public abstract void SpawnUnits(SummonAbilityConfig config, Transform spawnSpot); 
    }
}
