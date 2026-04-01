using Rush;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    
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
        private IEnumerator DelayOpen(float delay, UnityAction action)
        {
            yield return new WaitForSeconds(delay);
            if (IsShowInternal) yield break;
            action?.Invoke();
        }

        public void SetStageConfig(StageConfig stageConfig)
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
    }
}
