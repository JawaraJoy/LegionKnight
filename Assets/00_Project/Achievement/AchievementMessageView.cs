using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class AchievementMessageView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_MessageText;

        public void ShowMessage(Achievement achievement)
        {
            ShowInternal();
            m_MessageText.text = achievement.UnclockedMessage;
        }
    }
}
