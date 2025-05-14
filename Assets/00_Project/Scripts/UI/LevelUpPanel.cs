using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelIds
    {
        public const string LevelUpPanel = "LevelUpPanel";
    }
    public partial class LevelUpPanel : PanelView
    {
        public override string UniqueId => PanelIds.LevelUpPanel;

        [SerializeField]
        private TextMeshProUGUI m_LevelText;

        public void SetLevelText(int level)
        {
            if (m_LevelText != null)
            {
                m_LevelText.text = $"{level}";
            }
        }
    }

    public partial class GameManager
    {
        private LevelUpPanel GetLevelUpPanel()
        {
            return GetPanelInternal<LevelUpPanel>();
        }

        public void ShowLevelUpPanel()
        {
            LevelUpPanel levelUpPanel = GetLevelUpPanel();
            if (levelUpPanel != null)
            {
                levelUpPanel.Show();
                levelUpPanel.SetLevelText(Player.Instance.GetPlayerLevel());
            }
        }
    }
}
