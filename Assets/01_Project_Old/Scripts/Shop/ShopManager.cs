using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class ShopManager : ShopHandler
    {
        
    }
    public partial class GameManager
    {
        [SerializeField]
        private ShopManager m_ShopManager;
        public ShopManager ShopManager => m_ShopManager;
    }

    public partial class ShopManagerAgent
    {
        public void InitShop()
        {
            GameManager.Instance.ShopManager.Init();
        }
        public void SelectShop(string containerName)
        {
            GameManager.Instance.ShopManager.SelectShop(containerName);
        }
    }

}
