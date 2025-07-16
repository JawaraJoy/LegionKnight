using UnityEngine;

namespace LegionKnight
{
    public class BosBarGameplayAgent : MonoBehaviour
    {

        private GameplayPanel GetGameplayPanel()
        {
            return GameManager.Instance.GetPanel<GameplayPanel>();
        }
        public void SetCastingName(string castingName)
        {
            var gameplayPanel = GetGameplayPanel();
            if (gameplayPanel != null)
            {
                gameplayPanel.SetCastingName(castingName);
            }
        }
        public void SetCastingTime(float castingTime)
        {
            var gameplayPanel = GetGameplayPanel();
            if (gameplayPanel != null)
            {
                gameplayPanel.SetCastingTime(castingTime);
            }

        }
        public void HideCastingBar()
        {
            var gameplayPanel = GetGameplayPanel();
            if (gameplayPanel != null)
            {
                gameplayPanel.HideCastingBar();
            }
        }
    }
}
