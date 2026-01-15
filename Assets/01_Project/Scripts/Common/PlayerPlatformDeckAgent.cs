using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class PlayerPlatformDeckAgent : MonoBehaviour
    {
        public PlatformConfig GetUsedStanbyPlatform()
        {
            return Player.Instance.GetUsedStanbyPlatform();
        }
        public void SetIsOwned(PlatformConfig platform, int add)
        {
            Player.Instance.AddPlatformAmount(platform, add);
        }
        public void SetUsedStandbyPlatform()
        {
            Player.Instance.SetUsedStanbyPlatform();
        }
        public void SelectStandbyPlatform(PlatformConfig platform)
        {
            Player.Instance.SelectStandbyPlatform(platform);
        }
    }
}
