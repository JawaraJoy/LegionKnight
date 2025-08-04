using UnityEngine;

namespace LegionKnight
{
    public class PlatformModifierAgent : MonoBehaviour
    {
        public void SetPlatformSpeed(float speedRate)
        {
            GameManager.Instance.SetSpeedPlatformRate(speedRate);
        }
    }
}
