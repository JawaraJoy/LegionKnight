using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public partial class SceneHandler : MonoBehaviour
    {
        [SerializeField]
        private List<SceneField> m_Scenes = new();

        private SceneField GetSceneField(string sceneName)
        {
            SceneField match = m_Scenes.Find(x => x.SceneName == sceneName);
            return match;
        }
        public void LoadScene(string sceneName)
        {
            LoadSceneInternal(sceneName);
        }
        protected void LoadSceneInternal(string sceneName)
        {
            GetSceneField(sceneName).LoadScene();
        }
        public void UnLoadScene(string sceneName)
        {
            GetSceneField(sceneName).UnloadScene();
        }
    }
}
