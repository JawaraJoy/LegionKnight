using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public partial class BosDeath : MonoBehaviour
    {
        public void EraseBosDamageables()
        {
            EraseBosProjectileInternal();
            StartCoroutine(OpenWinPanelDelay(4f));
            /*if (!GameManager.Instance.IsInfiniteLevel)
            {
                GameManager.Instance.SetLevelOver(true);
                WinPanel winPanel = GameManager.Instance.GetPanel<WinPanel>();
                winPanel.Show();
                winPanel.SetLevelDefinition(GameManager.Instance.LevelDefinition);
                GameManager.Instance.SetLevelUnlocked(GameManager.Instance.LevelDefinition.NextLevel, true);
                GameManager.Instance.SetLevelCompleted(GameManager.Instance.LevelDefinition, true);
                
            }*/
        }

        public void OpenWinPanel(float delay)
        {
            StartCoroutine(OpenWinPanelDelay(delay));
        }

        private IEnumerator OpenWinPanelDelay(float delay)
        {
            if (!GameManager.Instance.IsInfiniteLevel)
            {
                GameManager.Instance.SetLevelOver(true);
                yield return new WaitForSeconds(delay);
                WinPanel winPanel = GameManager.Instance.GetPanel<WinPanel>();
                winPanel.Show();
                Debug.Log("Bos is DEath Show Win Panel");
                /*winPanel.SetLevelDefinition(GameManager.Instance.LevelDefinition);
                GameManager.Instance.SetLevelUnlocked(GameManager.Instance.LevelDefinition.NextLevel, true);
                GameManager.Instance.SetLevelCompleted(GameManager.Instance.LevelDefinition, true);*/
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
                Addressables.ReleaseInstance(damageable.gameObject);
                //Destroy(damageable.gameObject);
            }
            ProjectileDamage[] projectiles = FindObjectsByType<ProjectileDamage>(FindObjectsSortMode.None);
            // Loop through each GameObject and destroy it
            foreach (ProjectileDamage projectile in projectiles)
            {
                Addressables.ReleaseInstance(projectile.gameObject);
                //Destroy(projectile.gameObject);
            }
        }
    }
}
