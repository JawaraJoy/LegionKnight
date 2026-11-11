using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class IAPBundleEntry
    {
        [SerializeField] private CollectibleConfig m_Collectible;
        [SerializeField] private int m_Amount = 1;

        public CollectibleConfig Collectible => m_Collectible;
        public int Amount => m_Amount;
    }
}