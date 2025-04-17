using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerPlatformDeckAgent : MonoBehaviour
    {
        public StandbyPlatformDefinition GetUsedStanbyPlatform()
        {
            return Player.Instance.GetUsedStanbyPlatform();
        }
        public void SetIsOwned(StandbyPlatformDefinition platform, bool isOwned)
        {
            Player.Instance.SetIsPlatformOwned(platform, isOwned);
        }
        public void SetUsedStandbyPlatform()
        {
            Player.Instance.SetUsedStanbyPlatform();
        }
        public void SelectStandbyPlatform(StandbyPlatformDefinition platform)
        {
            Player.Instance.SelectStandbyPlatform(platform);
        }
    }
}
