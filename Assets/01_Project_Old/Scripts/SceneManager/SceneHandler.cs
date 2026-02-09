using Rush;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
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
            SceneField match = m_Scenes.Find(x => x.SceneConfig.SceneName == sceneName);
            return match;
        }
        [Obsolete("Use LoadSceneConfig in the future")]
        public void LoadScene(string sceneName)
        {
            LoadSceneInternal(sceneName);
            OnStartLoadScene();
        }
        public void LoadSceneConfig(SceneConfig sceneConfig)
        {
            LoadSceneInternal(sceneConfig.SceneName);
            OnStartLoadScene();
        }
        protected void LoadSceneInternal(string sceneName)
        {
            GetSceneField(sceneName).LoadScene();
        }
        [Obsolete("Use UnLoadSceneConfig in the future")]
        public void UnLoadScene(string sceneName)
        {
            GetSceneField(sceneName).UnloadScene();
        }
        public void UnLoadSceneConfig(SceneConfig sceneConfig)
        {
            GetSceneField(sceneConfig.SceneName).UnloadScene();
        }
        private void OnStartLoadScene()
        {
            m_OnStartLoadScene?.Invoke();
        }
    }
}
