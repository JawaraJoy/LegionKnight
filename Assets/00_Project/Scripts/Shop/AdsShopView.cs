using UnityEngine;

namespace LegionKnight
{
    public partial class AdsShopView : ShopView
    {
        
    }

    public partial class ShopPanel
    {
        private AdsShopView GetAdsShopView()
        {
            return GetBinding<AdsShopView>();
        }
    }
}
