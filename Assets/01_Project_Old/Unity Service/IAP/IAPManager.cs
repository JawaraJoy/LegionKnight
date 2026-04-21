using LegionKnight;
using Rush;
using UnityEngine;

namespace Rush
{
    public class IAPManager : InAppPurchase
    {
        
    }
}

namespace LegionKnight
{
    public partial class UnityService
    {
        [SerializeField]
        private IAPManager m_IAPManager;
        public IAPManager IAPManager => m_IAPManager;
    }
}
