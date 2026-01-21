using UnityEngine;

namespace LegionKnight
{
    public class PlatformModifierAgentTwo : MonoBehaviour
    {
        public void SetPlatformSpeed(float speedRate)
        {
            GameManager.Instance.SetSpeedPlatformRate(speedRate);
        }
    }
}
