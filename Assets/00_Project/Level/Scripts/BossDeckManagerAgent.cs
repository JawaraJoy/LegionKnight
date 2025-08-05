using UnityEngine;

namespace LegionKnight
{
    public class BossDeckManagerAgent : MonoBehaviour
    {
        public BossUnit GetRandomDefeatedBoss()
        {
            return GameManager.Instance.GetRandomDefeatedBoss();
        }

        public void Initialize()
        {
            GameManager.Instance.InitializeBossDeck();

        }
        public void DefeatedBoss(BosDefinition defi)
        {
            GameManager.Instance.DefeatedBoss(defi);
        }
    }
}
