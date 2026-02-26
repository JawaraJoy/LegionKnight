using UnityEngine;

namespace Rush
{
    public class StageManagerAgent : MonoBehaviour
    {
        public void PlayStage()
        {
            RushGameManager.Instance.StageManager.PlayStage();
        }
    }
}
