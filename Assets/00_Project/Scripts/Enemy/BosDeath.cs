using UnityEngine;

namespace LegionKnight
{
    public partial class BosDeath : MonoBehaviour
    {
        public void EraseBosDamageables()
        {
            EraseBosProjectileInternal();
            if (!GameManager.Instance.IsInfiniteLevel)
            {
                GameManager.Instance.SetLevelOver(true);
                WinPanel winPanel = GameManager.Instance.GetPanel<WinPanel>();
                winPanel.Show();
                winPanel.SetLevelDefinition(GameManager.Instance.LevelDefinition);
                GameManager.Instance.SetLevelUnlocked(GameManager.Instance.LevelDefinition.NextLevel, true);
                GameManager.Instance.SetLevelCompleted(GameManager.Instance.LevelDefinition, true);
                
            }
        }
        public void EraseBossProjectile()
        {
            EraseBosProjectileInternal();
        }
        private void EraseBosProjectileInternal()
        {
            BosDamageable[] damageables = FindObjectsByType<BosDamageable>(FindObjectsSortMode.None);
            // Loop through each GameObject and destroy it
            foreach (BosDamageable damageable in damageables)
            {
                Destroy(damageable.gameObject);
            }

        }
    }
}
