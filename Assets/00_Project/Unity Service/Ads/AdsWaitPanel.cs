using System.Collections;
using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public const string AdsWait = "AdsWaitPanel";
    }
    public class AdsWaitPanel : PanelView
    {
        public override string UniqueId => PanelId.AdsWait;

        [SerializeField]
        private TextMeshProUGUI m_WaitingText;
        public void ShowWaitMessage(string message)
        {
            ShowWaitMessageInternal(message);
        }
        private void ShowWaitMessageInternal(string message)
        {
            ShowInternal();
            SetWaitMessageInternal(message);
        }
        public void ShowWaitMessage(string message, float duration)
        {
            StartCoroutine(ShowingWaitMessageTempo(message, duration));
        }
        private IEnumerator ShowingWaitMessageTempo(string message, float duration)
        {
            ShowWaitMessageInternal(message);
            yield return new WaitForSeconds(duration);
            HideInternal();
        }
        public void SetWaitMessage(string message)
        {
            SetWaitMessageInternal(message);
        }
        private void SetWaitMessageInternal(string message)
        {
            m_WaitingText.text = message;
        }
    }
    public partial class GameManager
    {
        private AdsWaitPanel GetAdsWaitPanel()
        {
            return GetPanelInternal<AdsWaitPanel>();
        }
        public void ShowAdWaitMessage(string message)
        {
            GetAdsWaitPanel().ShowWaitMessage(message);
        }
        public void SetAdWaitMessage(string message)
        {
            GetAdsWaitPanel().SetWaitMessage(message);
        }
        public void ShowAdWaitMessage(string message, float duration)
        {
            GetAdsWaitPanel().ShowWaitMessage(message, duration);
        }
    }
}
