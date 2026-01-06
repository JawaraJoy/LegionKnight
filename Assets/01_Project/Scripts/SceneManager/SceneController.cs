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
        public void LoadScene(string sceneName)
        {
            m_SceneController.LoadScene(sceneName);
        }
        public void UnLoadScene(string sceneName)
        {
            m_SceneController.UnLoadScene(sceneName);
        }
    }
}
