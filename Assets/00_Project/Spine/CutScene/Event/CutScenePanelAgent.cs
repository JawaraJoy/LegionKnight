using UnityEngine;

namespace LegionKnight
{
    public class CutScenePanelAgent : MonoBehaviour
    {
        [SerializeField]
        private SpineAnimDefinition m_Definition;
        private CutScenePanel GetPanel()
        {
            return GameManager.Instance.GetPanel<CutScenePanel>();
        }

        public void PlayUI()
        {
            GetPanel().Show();
            SpineUI spineUI = GetPanel().GetBinding<SpineUI>();
            spineUI.Play(m_Definition);
        }

        public void Pause()
        {
            SpineUI spineUI = GetPanel().GetBinding<SpineUI>();
            spineUI.PauseUI(m_Definition);
        }
        public void Resume()
        {
            SpineUI spineUI = GetPanel().GetBinding<SpineUI>();
            spineUI.ResumeUI(m_Definition);
        }
    }
}
