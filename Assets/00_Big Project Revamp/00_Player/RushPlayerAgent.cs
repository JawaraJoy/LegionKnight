using UnityEngine;

namespace Rush
{
    public class RushPlayerAgent : MonoBehaviour
    {
        public void Init(HeroUnitConfig lastUsedHero)
        {
            RushPlayer.Instance.Init(lastUsedHero);
        }
    }
}
