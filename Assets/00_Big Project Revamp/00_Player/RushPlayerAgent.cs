using UnityEngine;

namespace Rush
{
    public class RushPlayerAgent : MonoBehaviour
    {
        public void Init(HeroUnitConfig lastUsedHero)
        {
            RushPlayer.Instance.Init(lastUsedHero);
        }
        public void SetPosition(Vector2 set)
        {
            RushPlayer.Instance.SetPosition(set);
        }
        public void ResetProgression()
        {
            RushPlayer.Instance.ResetProgression();
        }
    }
}
