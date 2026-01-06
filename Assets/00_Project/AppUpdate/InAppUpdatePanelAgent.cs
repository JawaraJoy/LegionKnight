using Google.Play.AppUpdate;
using UnityEngine;

namespace LegionKnight
{
    public class InAppUpdatePanelAgent : MonoBehaviour
    {
        private InAppUpdatePanel m_Panel;
        private InAppUpdatePanel GetPanel()
        {
            if (m_Panel == null)
            {
                m_Panel = CanvasManager.Instance.GetPanel<InAppUpdatePanel>();
            }
            return m_Panel;
        }
        public void ShowCheckText()
        {
            GetPanel().Show();
            GetPanel().GetBinding<InAppUpdateCheckText>().Show();
        }
        public void HideCheckText()
        {
            GetPanel().Hide();
            GetPanel().GetBinding<InAppUpdateCheckText>().Hide();
        }
        public void ShowAvaiableInfo()
        {
            GetPanel().Show();
            GetPanel().GetBinding<InAppUpdateAvailable>().Show();
        }
        public void HideAvaiableInfo()
        {
            GetPanel().Hide();
            GetPanel().GetBinding<InAppUpdateAvailable>().Hide();
        }
        public void ShowFail()
        {
            GetPanel().Show();
            GetPanel().GetBinding<InAppDownloadFailed>().Show();
        }
        public void HideFail()
        {
            GetPanel().Hide();
            GetPanel().GetBinding<InAppDownloadFailed>().Hide();
        }
        public void Show()
        {
            GetPanel().Show();
        }
        public void Hide()
        {
            GetPanel().Hide();
        }
        public void SetAvailableInfo(AppUpdateInfo info)
        {
            GetPanel().GetBinding<InAppUpdateAvailable>().SetAvailableInfo(info);
        }
        public void SetCheckText(string text)
        {
            GetPanel().GetBinding<InAppUpdateCheckText>().SetCheckText(text);
        }
    }
}
