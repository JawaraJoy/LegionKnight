using UnityEngine;

namespace Rush
{
    public class GameStateManager : GameStateHandler
    {
        
    }
    public partial class RushGameManager
    {
        [SerializeField]
        private GameStateManager m_GameStateManager;
        public GameStateManager GameStateManager => m_GameStateManager;
    }
}
