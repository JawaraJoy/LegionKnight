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
        public void LoadSceneAsset(SceneAsset sceneAsset)
        {
            GameManager.Instance.SceneController.LoadSceneAsset(sceneAsset);
        }
        public void UnLoadSceneAsset(SceneAsset sceneAsset)
        {
            GameManager.Instance.SceneController.UnLoadSceneAsset(sceneAsset);
        }
    }
}
