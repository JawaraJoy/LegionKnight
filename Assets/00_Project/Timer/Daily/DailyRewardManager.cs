using UnityEngine;

namespace LegionKnight
{
    public class DailyRewardManager : DailyReward
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private DailyRewardManager m_DailyRewardManager;
        public DailyRewardManager DailyRewardManager => m_DailyRewardManager;
    }
}
