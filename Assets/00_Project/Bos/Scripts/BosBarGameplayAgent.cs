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
        public void SetBosName(BosDefinition defi)
        {
            var gameplayPanel = GetGameplayPanel();
            if (gameplayPanel != null)
            {
                gameplayPanel.SetBosName(defi);
            }
        }
        public void SetHealth(float rate)
        {
            var gameplayPanel = GetGameplayPanel();
            if (gameplayPanel != null)
            {
                gameplayPanel.SetHealth(rate);
            }
        }
        public void ShowHealthBar()
        {
            var gameplayPanel = GetGameplayPanel();
            if (gameplayPanel != null)
            {
                gameplayPanel.ShowHealthBar();
            }
        }

        public void HideHealthBar()
        {
            var gameplayPanel = GetGameplayPanel();
            if (gameplayPanel != null)
            {
                gameplayPanel.HideHealthBar();
            }
        }
    }
}
