using UnityEngine;

namespace Rush
{
    public class StageManagerAgent : MonoBehaviour
    {
        public void PlayStage()
        {
            RushGameManager.Instance.StageManager.PlayStage();
        }
        public void Resume()
        {
            RushGameManager.Instance.StageManager.Resume();
        }
        public void Pause()
        {
            RushGameManager.Instance.StageManager.Pause();
        }
    }
}
