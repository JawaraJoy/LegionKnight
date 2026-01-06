using UnityEngine;

namespace LegionKnight
{
    public class SceneManagerAgent : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            GameManager.Instance.LoadScene(sceneName);
        }
        public void UnLoadScene(string sceneName)
        {
            GameManager.Instance.UnLoadScene(sceneName);
        }
    }
}
