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

        [SerializeField] private PlayerLevelUpView m_Presentation;
        private PlayerProgression m_LegacyProgression;

        private void Awake()
        {
            if (m_Presentation != null) return;
            m_LegacyProgression = Player.Instance.Progression;
            m_LegacyProgression.AddOnCurrentExpRateChange(SetExpSlider);
            m_LegacyProgression.AddOnLevelUp(SetLevelText);
            if (m_LevelUpMonitor != null) m_LevelUpMonitor.Init();
        }

        private void OnDestroy()
        {
            if (m_LegacyProgression == null) return;
            m_LegacyProgression.RemoveOnCurrentExpRateChange(SetExpSlider);
            m_LegacyProgression.RemoveOnLevelUp(SetLevelText);
        }
        protected override void ShowInternal()
        {
            var progression = Player.Instance.Progression;
            if (!progression.LevelUpTrigerred || MasterPanelUtility.IsShow) return;
            if (m_Presentation != null && IsShown && m_Presentation.isActiveAndEnabled) return;
            int previous = progression.PreviousLevel;
            base.ShowInternal();
            if (!IsShown) return;
            if (m_Presentation != null) m_Presentation.Present(progression, previous);
            else progression.SetLevelUpTriggered(false);
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
