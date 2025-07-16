using UnityEngine;

namespace LegionKnight
{
    public class InteruptViewAgent : MonoBehaviour
    {
        private GameplayPanel GetGameplayPanel()
        {
            GameplayPanel gameplayPanel = GameManager.Instance.GetPanel<GameplayPanel>();
            if (gameplayPanel == null)
            {
                Debug.LogError("GameplayPanel not found in GameManager.");
            }
            return gameplayPanel;
        }

        public void SetInteruptText(int current, int max)
        {
            GetGameplayPanel().SetInteruptText(current, max);
        }
        public void ShowInteruptView()
        {
            GetGameplayPanel().ShowInteruptView();
        }
        public void HideInteruptView()
        {
            GetGameplayPanel().HideInteruptView();
        }
    }
}
