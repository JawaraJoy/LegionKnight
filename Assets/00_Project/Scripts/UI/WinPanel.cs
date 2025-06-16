using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public const string WinPanel = "WinPanel";
    }
    public partial class WinPanel : PanelView
    {
        [SerializeField]
        private string m_HomeSceneName = "ReworkHome";
        [SerializeField]
        private LevelDefinition m_CurrenLevel;
        [SerializeField]
        private Image m_NextLevelImage;
        [SerializeField]
        private TextMeshProUGUI m_CompleteText;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            Player.Instance.SetPause(true);
        }
        public override void Hide()
        {
            base.Hide();
            Player.Instance.SetPause(false);
        }

        public void SetLevelDefinition(LevelDefinition defi)
        {
            m_CurrenLevel = defi;
            m_NextLevelImage.sprite = m_CurrenLevel.NextLevel.LevelImage;

            if (m_CurrenLevel == m_CurrenLevel.NextLevel)
            {
                if (GameManager.Instance.IsLevelUnlocked(m_CurrenLevel.NextLevel))
                {
                    m_CompleteText.text = "The Next Level already Uncloked";
                }
                m_CompleteText.text = "Every Level Is Cleared";
            }
            else
            {
                m_CompleteText.text = "New Level is Unlocked";
            }
        }

        public void StartNextLevel()
        {
            if (m_CurrenLevel != null)
            {
                m_CurrenLevel.NextLevel.StartLevel();
                GameManager.Instance.StoreLevelScore();
            }
            else
            {
                Debug.LogError("No level definition set.");
            }
            HideInternal();
        }

        public void PlayAgain()
        {
            if (m_CurrenLevel != null)
            {
                m_CurrenLevel.StartLevel();
                GameManager.Instance.StoreLevelScore();
            }
            else
            {
                Debug.LogError("No level definition set.");
            }
            HideInternal();
        }
        public void BackHome()
        {
            GameManager.Instance.LoadScene(m_HomeSceneName);
            HideInternal();
            GameManager.Instance.StoreLevelScore();
        }
    }
}
