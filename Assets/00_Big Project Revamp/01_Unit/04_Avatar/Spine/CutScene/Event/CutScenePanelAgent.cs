using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class CutScenePanelAgent : MonoBehaviour
    {
        [SerializeField]
        private AnimationClipConfig m_Config;
        private CutScenePanel GetPanel()
        {
            return CanvasManager.Instance.GetPanel<CutScenePanel>();
        }

        public void PlayUI()
        {
            GetPanel().Show();
            AvatarSpineUI spineUI = GetPanel().GetBinding<AvatarSpineUI>();
            spineUI.PlayClip(m_Config);
        }

        public void Pause()
        {
            AvatarSpineUI spineUI = GetPanel().GetBinding<AvatarSpineUI>();
            spineUI.Pause();
        }
        public void Resume()
        {
            AvatarSpineUI spineUI = GetPanel().GetBinding<AvatarSpineUI>();
            spineUI.Resume();
        }
    }
}
