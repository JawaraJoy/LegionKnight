using UnityEngine;

namespace Rush
{
    public class StageManagerAgent : MonoBehaviour, IReseter
    {
        public void PlayStage()
        {
            RushGameManager.Instance.StageManager.PlayStage();
        }
        public void Resume()
        {
            //RushGameManager.Instance.StageManager.Resume();
        }
        public void Pause()
        {
            //RushGameManager.Instance.StageManager.Pause();
        }
        public void SelectStage(StageConfig stage)
        {
            RushGameManager.Instance.StageManager.SelectStage(stage);
        }
        public void SetBackground(VerticalBackgroundConfig backgroundConfig)
        {
            RushGameManager.Instance.StageManager.SetBackground(backgroundConfig);
        }

        public void ResetProgression()
        {
            RushGameManager.Instance.StageManager.ResetProgression();
        }
    }
}
