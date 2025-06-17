using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class LevelRewardView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_TitleText;
        [SerializeField]
        private LevelRewardItemView[] m_RewardItems;

        public void Init(CharacterReward reward)
        {
            m_TitleText.text = reward.RewardState == RewardState.First ? "First Clear" : "Rewards";
            RewardObject[] rewards = reward.Rewards;

            for (int i = 0; i < m_RewardItems.Length; i++)
            {
                if (i < rewards.Length)
                {
                    m_RewardItems[i].Show();
                    m_RewardItems[i].Init(rewards[i]);
                }
                else
                {
                    m_RewardItems[i].Hide();
                }
            }
        }
    }
}
