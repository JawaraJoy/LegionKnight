using Rush;
using UnityEditor;
using UnityEngine;

namespace LegionKnight
{
    public class SceneManagerAgent : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            GameManager.Instance.SceneController.LoadScene(sceneName);
        }
        public void UnLoadScene(string sceneName)
        {
            GameManager.Instance.SceneController.UnLoadScene(sceneName);
        }
        public void LoadSceneConfig(SceneConfig sceneAsset)
        {
            GameManager.Instance.SceneController.LoadSceneConfig(sceneAsset);
        }
        public void UnLoadSceneConfig(SceneConfig sceneAsset)
        {
            GameManager.Instance.SceneController.UnLoadSceneConfig(sceneAsset);
        }
    }
}
