using UnityEngine;

namespace Rush
{
    public class RogueLikeManager : RogueLikeHandler
    {
        
    }
    // this is singleton
    // you can call RushGameManager.Instance.RoguelikeManager
    public partial class RushGameManager
    {
        [SerializeField]
        private RogueLikeManager m_RogueLikeManager;

        public RogueLikeManager RogueLikeManager => m_RogueLikeManager;
    }
}
