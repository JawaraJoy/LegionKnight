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
        private StageConfig m_CurrenStage;
        [SerializeField]
        private Image m_NextLevelImage;
        [SerializeField]
        private TextMeshProUGUI m_CompleteText;

        protected override void ShowInternal()
        {
            
        }
        private IEnumerator DelayOpen(float delay, UnityAction action)
        {
            yield return new WaitForSeconds(delay);
            if (IsShowInternal) yield break;
            action?.Invoke();
        }
        public override void Hide()
        {
            base.Hide();
        }

        public void SetLevelDefinition(StageConfig stageConfig)
        {
            
        }

        public void StartNextLevel()
        {
            if (m_CurrenStage != null)
            {
                
            }
            else
            {
                Debug.LogError("No level definition set.");
            }
            HideInternal();
        }

        public void PlayAgain()
        {
            if (m_CurrenStage != null)
            {
                
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
        }
    }
}
