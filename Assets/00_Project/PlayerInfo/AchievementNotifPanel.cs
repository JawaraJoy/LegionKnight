using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class AchievementNotifPanel : PanelView
    {
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private TextMeshProUGUI m_Desc;
        [SerializeField]
        private TextMeshProUGUI m_Note;

        public void ShowNotif(BadgeContent badge)
        {
            if (badge == null || badge.Definition == null)
            {
                return;
            }
            ShowInternal();
            int level = badge.CurrentUpgradeLevel;
            m_Icon.sprite = badge.Definition.Upgrade[level].Icon;

            string desc = $"Achievement [<color=yellow>{badge.Definition.Upgrade[level].Label}</color>]";
            m_Desc.text = desc;
            m_Note.text = "Go to Profile to claim the Reward!!";
            StartCoroutine(HideAfterDelay(5f));
        }

        private IEnumerator HideAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            HideInternal();
        }
    }
}
