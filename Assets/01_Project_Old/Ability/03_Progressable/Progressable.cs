using UnityEngine;

namespace LegionKnight
{
    public class Progressable : MonoBehaviour
    {

    }

    public interface IProgressable
    {
        int GetLevel();
    }

    public static class SProgressable
    {
        public static int GetLevel(GameObject spawnedBy)
        {
            int level = 1;
            if (spawnedBy != null)
            {
                Player player = spawnedBy.GetComponentInParent<Player>();
                BosEnemy bosEnemy = spawnedBy.GetComponentInParent<BosEnemy>();
                IEnemy enemy = spawnedBy.GetComponentInParent<IEnemy>();
                if (player != null)
                {
                    level = player.UsedCharacter.GetUnitLevel();
                }
                if (bosEnemy != null)
                {
                    level = bosEnemy.GetBosLevel();
                }
                if (enemy != null)
                {
                    level = enemy.DynamicLevel;
                }
            }
            return level;
        }
    }
}
