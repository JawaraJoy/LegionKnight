using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class ShopManager : ShopHandler
    {
        
    }

    public partial class RushPlayer
    {
        [SerializeField] private ShopManager m_ShopManager;
        public ShopManager ShopManager => m_ShopManager;
    }
}