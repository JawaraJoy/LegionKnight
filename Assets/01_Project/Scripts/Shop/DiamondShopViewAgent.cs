using UnityEngine;

namespace LegionKnight
{
    public class DiamondShopViewAgent : MonoBehaviour
    {
        public void ShowDiamondShop(string showTab)
        {
            CanvasManager.Instance.ShowDiamondShop(showTab);
        }
    }
}
