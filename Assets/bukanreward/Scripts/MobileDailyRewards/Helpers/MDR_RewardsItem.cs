using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AppsDaddyO.Rewards
{
    // A reward class for the creation of the rewards
    [System.Serializable]
    public class MDR_RewardsItem
    {
        public string rewardName;
        public Sprite rewardImage;
        public int rewardAmount;

        public MDR_RewardsItem(string rewardName, Sprite rewardImage, int rewardAmount)
        {
            this.rewardName = rewardName;
            this.rewardImage = rewardImage;
            this.rewardAmount = rewardAmount;
        }
    }
}