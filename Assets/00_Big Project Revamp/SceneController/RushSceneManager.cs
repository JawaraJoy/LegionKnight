using LegionKnight;
using UnityEngine;

namespace Rush
{
    public partial class RushSceneManager : SceneController
    {
        
    }
    public partial class RushGameManager
    {
        [SerializeField]
        private RushSceneManager m_SceneController;
        public RushSceneManager SceneContrller => m_SceneController;
    }
}
