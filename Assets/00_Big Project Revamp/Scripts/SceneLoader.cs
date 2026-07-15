using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rush
{
    public class SceneLoader : MonoBehaviour
    {
        public void RetScene()
        {
            RushGameManager.Instance.StageManager.ResetProgression();
            RushPlayer.Instance.ResetProgression();
        }
    }
}
