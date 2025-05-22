using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class UnlockedRewardView : UIView
    {
        [SerializeField]
        private UnlockedTextView[] m_UnlockedTextViews;

        public void ShowUnlockedReward(List<string> unlockedRewards)
        {
            ShowInternal();
            foreach (var textView in m_UnlockedTextViews)
            {
                textView.Hide();
            }
            for (int i = 0; i < m_UnlockedTextViews.Length; i++)
            {
                if (i < unlockedRewards.Count)
                {
                    m_UnlockedTextViews[i].ShowText(unlockedRewards[i]);
                }
            }
        }
    }
}
