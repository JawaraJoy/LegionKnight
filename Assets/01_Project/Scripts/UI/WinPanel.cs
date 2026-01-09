using Rush;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
        private SceneConfig m_HomeScene;
        [SerializeField]
        private LevelDefinition m_CurrenLevel;
        [SerializeField]
        private Image m_NextLevelImage;
        [SerializeField]
        private TextMeshProUGUI m_CompleteText;

        [SerializeField]
        private UnityEvent<CharacterReward> m_OnSetLevelDefinition = new();
        protected override void ShowInternal()
        {
            if (!GameManager.Instance.IsInfiniteLevel)
            {
                GameManager.Instance.SetLevelOver(true);
                SetLevelDefinition(GameManager.Instance.LevelDefinition);
                GameManager.Instance.SetLevelUnlocked(GameManager.Instance.LevelDefinition.NextLevel, true);
                GameManager.Instance.SetLevelCompleted(GameManager.Instance.LevelDefinition, true);
                UnityAction open = new (base.ShowInternal);
                StartCoroutine(DelayOpen(3f, open));
                Player.Instance.SetPause(true);
            }
        }
        private IEnumerator DelayOpen(float delay, UnityAction action)
        {
            yield return new WaitForSeconds(delay);
            if (m_IsShow) yield break;
            action?.Invoke();
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

            bool isComplete = GameManager.Instance.IsLevelCompleted(m_CurrenLevel);
            CharacterReward reward = isComplete ? m_CurrenLevel.RepeatReward : m_CurrenLevel.FirstReward;
            m_OnSetLevelDefinition.Invoke(reward);
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
            GameManager.Instance.SceneController.LoadSceneConfig(m_HomeScene);
            HideInternal();
            GameManager.Instance.StoreLevelScore();
        }
    }
}
