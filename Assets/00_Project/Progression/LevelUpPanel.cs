using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField]
        private Slider m_ExpSlider;

        private void Awake()
        {
            Player.Instance.AddOnCurrentExpRateChange(SetExpSlider);
            Player.Instance.AddOnLevelUp(SetLevelText);
        }

        public void SetLevelText(int level)
        {
            if (m_LevelText != null)
            {
                m_LevelText.text = $"{level}";
            }
        }
        public void SetExpSlider(float value)
        {
            if (m_ExpSlider != null)
            {
                m_ExpSlider.value = value;
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
            Debug.Log("Show Level Up Panel");
            LevelUpPanel levelUpPanel = GetLevelUpPanel();
            if (levelUpPanel != null)
            {
                levelUpPanel.Show();
                levelUpPanel.SetLevelText(Player.Instance.GetPlayerLevel());
                levelUpPanel.SetExpSlider(Player.Instance.GetPlayerLevelProgressionRate());
            }
        }
    }
}
