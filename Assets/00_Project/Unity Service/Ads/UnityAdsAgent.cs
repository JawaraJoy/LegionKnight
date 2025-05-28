using UnityEngine;

namespace LegionKnight
{
    public partial class UnityAdsAgent : MonoBehaviour
    {
        public void LoadRewardedAd()
        {
            UnityService.Instance.LoadRewardedAd();
        }
    }
}
