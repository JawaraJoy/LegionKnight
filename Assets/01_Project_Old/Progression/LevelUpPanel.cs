using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class LevelUpPanel : PanelView
    {

        [SerializeField]
        private TextMeshProUGUI m_LevelText;
        [SerializeField]
        private Slider m_ExpSlider;
        [SerializeField]
        private LevelUpMonitor m_LevelUpMonitor;

        private void Awake()
        {
            Player.Instance.AddOnCurrentExpRateChange(SetExpSlider);
            Player.Instance.AddOnLevelUp(SetLevelText);
            m_LevelUpMonitor.Init();
        }
        protected override void ShowInternal()
        {
            bool isLevelUp = Player.Instance.Progression.LevelUpTrigerred;
            if (isLevelUp)
            {
                base.ShowInternal();
            }
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

    public partial class CanvasManager
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
