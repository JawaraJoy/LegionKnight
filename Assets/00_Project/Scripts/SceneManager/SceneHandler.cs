using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class SceneHandler : MonoBehaviour
    {
        [SerializeField]
        private List<SceneField> m_Scenes = new();
        [SerializeField]
        private UnityEvent m_OnStartLoadScene = new();
        private SceneField GetSceneField(string sceneName)
        {
            SceneField match = m_Scenes.Find(x => x.SceneName == sceneName);
            return match;
        }
        public void LoadScene(string sceneName)
        {
            LoadSceneInternal(sceneName);
            OnStartLoadScene();
        }
        protected void LoadSceneInternal(string sceneName)
        {
            GetSceneField(sceneName).LoadScene();
        }
        public void UnLoadScene(string sceneName)
        {
            GetSceneField(sceneName).UnloadScene();
        }
        private void OnStartLoadScene()
        {
            m_OnStartLoadScene?.Invoke();
        }
    }
}
