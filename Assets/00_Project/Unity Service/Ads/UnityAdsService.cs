using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

namespace LegionKnight
{
    public class UnityAdsService : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<UnityAction> m_OnShow = new();
        public void ShowRewardedAd(UnityAction onCompleted)
        {
            m_OnShow.Invoke(onCompleted);
        }
    }
}
