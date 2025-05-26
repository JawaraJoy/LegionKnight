using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class AchievementMessageGroupView : UIView
    {
        [SerializeField]
        private AchievementMessageView[] m_MessageViews;
        public void ShowMessages()
        {
            List<Achievement> achievements = Player.Instance.GetAchievementHolds();
            if (achievements.Count <= 0)
            {
                HideInternal();
                return;
            }
            ShowInternal();
            foreach (var textView in m_MessageViews)
            {
                textView.Hide();
            }
            for (int i = 0; i < m_MessageViews.Length; i++)
            {
                if (i < achievements.Count)
                {
                    m_MessageViews[i].ShowMessage(achievements[i]);
                }
            }
        }
        public void ApplyMessage()
        {
            Player.Instance.ApplyAchievementGroup();
        }
    }
}
