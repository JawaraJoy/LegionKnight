using UnityEngine;

namespace LegionKnight
{
    public class PadManager : FlyCollecHandler
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private PadManager m_PadManager;
        public PadManager PadManager => m_PadManager;
    }
}
