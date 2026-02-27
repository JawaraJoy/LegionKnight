using UnityEngine;

namespace Rush
{
    public class RogueLikeManager : RogueLikeHandler
    {
        
    }
    public partial class RushGameManager
    {
        [SerializeField]
        private RogueLikeManager m_RogueLikeManager;

        public RogueLikeManager RogueLikeManager => m_RogueLikeManager;
    }
}
