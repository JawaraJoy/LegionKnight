using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rush
{
    [CreateAssetMenu(fileName = "Scene Config", menuName = "Rush/Scene")]
    public class SceneConfig : ScriptableObject
    {
        [SerializeField]
        private SceneAsset m_SceneAsset;
        [SerializeField]
        private LoadSceneMode m_Mode;
        [SerializeField]
        private float m_HideLoadingPanelDelay;
        public SceneAsset SceneAsset => m_SceneAsset;
        public LoadSceneMode Mode => m_Mode;
        public float HideLoadingPanelDelay => m_HideLoadingPanelDelay;
    }
}
