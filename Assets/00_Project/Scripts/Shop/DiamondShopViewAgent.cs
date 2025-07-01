using UnityEngine;

namespace LegionKnight
{
    public class DiamondShopViewAgent : MonoBehaviour
    {
        public void ShowDiamondShop(string showTab)
        {
            GameManager.Instance.ShowDiamondShop(showTab);
        }
    }
}
