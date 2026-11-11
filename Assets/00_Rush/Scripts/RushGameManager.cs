using UnityEngine;

namespace Rush
{
    public partial class RushGameManager : Singleton<RushGameManager>
    {
        [SerializeField]
        private GameConfig m_GameConfig;
        public GameConfig GameConfig => m_GameConfig;
    }
}
