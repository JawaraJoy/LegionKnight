#if UNITY_EDITOR
using UnityEditor;
#endif
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rush
{
    [CreateAssetMenu(fileName = "Scene Config", menuName = "Rush/Scene")]
    public class SceneConfig : ScriptableObject
    {
#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField]
        [Tooltip("Scene asset for editor selection only")]
        private SceneAsset m_SceneAsset;
#endif
        [SerializeField, MMReadOnly]
        private string m_SceneName;
        [SerializeField]
        private LoadSceneMode m_Mode;
        [SerializeField]
        private float m_HideLoadingPanelDelay;
        public string SceneName => m_SceneName;
        public LoadSceneMode Mode => m_Mode;
        public float HideLoadingPanelDelay => m_HideLoadingPanelDelay;
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_SceneAsset != null)
            {
                m_SceneName = m_SceneAsset.name;
            }
        }
#endif
    }
}
