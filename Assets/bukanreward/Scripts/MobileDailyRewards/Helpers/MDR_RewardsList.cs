using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppsDaddyO.Rewards
{
    // 3 lists are created for the first day, the middle of the week and last day to have different rarity rewards available    
    public class MDR_RewardsList : MonoBehaviour
    {
        public List<MDR_RewardsItem> firstDayOrAllRewards; // first day rewards list
        public List<MDR_RewardsItem> midWeekRewards;   // middle of the week rewards list
        public List<MDR_RewardsItem> lastDayRewards;   // last day rewards list
    }
}
