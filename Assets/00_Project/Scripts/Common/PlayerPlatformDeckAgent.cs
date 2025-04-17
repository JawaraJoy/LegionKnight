using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerPlatformDeckAgent : MonoBehaviour
    {
        public StanbyPlatform GetUsedStanbyPlatform()
        {
            return Player.Instance.GetUsedStanbyPlatform();
        }
        public void SetIsOwned(StanbyPlatform platform, bool isOwned)
        {
            Player.Instance.SetIsOwned(platform, isOwned);
        }
        public void SetUsedStandbyPlatform(StanbyPlatform platform)
        {
            Player.Instance.SetUsedStandbyPlatform(platform);
        }
    }
}
