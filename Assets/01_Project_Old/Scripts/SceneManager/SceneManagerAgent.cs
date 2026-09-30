using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class SceneManagerAgent : MonoBehaviour
    {
        
        public void LoadSceneConfig(SceneConfig sceneAsset)
        {
            GameSetting.Instance.SceneSetting.LoadSceneConfig(sceneAsset);
        }
        public void UnLoadSceneConfig(SceneConfig sceneAsset)
        {
            GameSetting.Instance.SceneSetting.UnLoadSceneConfig(sceneAsset);
        }
    }
}
