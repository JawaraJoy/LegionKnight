using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerPlatformDeckAgent : MonoBehaviour
    {
        public StandbyPlatformDefinition GetUsedStanbyPlatform()
        {
            return Player.Instance.GetUsedStanbyPlatform();
        }
        public void SetIsOwned(StandbyPlatformDefinition platform, int add)
        {
            Player.Instance.AddPlatformAmount(platform, add);
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
