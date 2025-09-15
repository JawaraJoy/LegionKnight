using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class ShopBannerField
    {
        [SerializeField]
        private string m_BannerName;
        [SerializeField]
        private Sprite m_BannerMainImage;
        public string BannerName => m_BannerName;
        public Sprite BannerMainImage => m_BannerMainImage;
    }
}
