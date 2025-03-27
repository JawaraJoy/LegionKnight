using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public partial class BannerPromoView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_PromoText;
        public void SetPromoText(string set)
        {
            m_PromoText.text = set;
        }
    }

    public partial class BannerPanel
    {
        public void SetPromoText(string set)
        {
            GetBinding<BannerPromoView>().SetPromoText(set);
        }
    }
    public partial class GachaManagerAgent
    {
        
        public void SetPromoText(GachaBanner banner)
        {
            string text = banner.PromoText;
            GetBannerPanel().SetPromoText(text);
        }
    }
}
