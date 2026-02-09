using UnityEngine;

namespace LegionKnight
{
    public partial class SceneController : SceneHandler
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private SceneController m_SceneController;
        public SceneController SceneController => m_SceneController;
    }
}
