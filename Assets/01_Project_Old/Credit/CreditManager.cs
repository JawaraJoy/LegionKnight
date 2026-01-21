using UnityEngine;

namespace LegionKnight
{
    public class CreditManager : Credit
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private CreditManager m_CreditManager;
        public CreditManager CreditManager => m_CreditManager;
    }
}
