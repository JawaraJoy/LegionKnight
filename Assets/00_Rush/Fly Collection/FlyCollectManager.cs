using UnityEngine;
using LegionKnight;
namespace Rush
{
    public class FlyCollectManager : FlyCollecHandler
    {
        
    }
    public partial class RushGameManager
    {
        [SerializeField]
        private FlyCollectManager m_FlyCollectManager;
        public FlyCollectManager FlyCollectManager => m_FlyCollectManager;
    }
}
