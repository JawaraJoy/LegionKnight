using UnityEngine;

namespace LegionKnight
{
    public class BossDeckManager : BossDeck
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private BossDeckManager m_BossDeckManager;

        public BossUnit GetRandomDefeatedBoss()
        {
            return m_BossDeckManager.GetRandomDefeatedBoss();
        }

        public void InitializeBossDeck()
        {
            m_BossDeckManager.Initialize();

        }
        public void DefeatedBoss(BosDefinition defi)
        {
            m_BossDeckManager.DefeatedBoss(defi);
        }
    }
}
