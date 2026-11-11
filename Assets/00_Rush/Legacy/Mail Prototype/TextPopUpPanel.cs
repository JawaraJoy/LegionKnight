using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    public class TextPopUpPanel : PanelView
    {
        [SerializeField]
        private float m_AutoHideDelay = 0f;
        private Coroutine m_HideRoutine;

        public void ShowText(string text)
        {
            ShowTextInternal(text);
        }
        private void ShowTextInternal(string text)
        {
            if (m_HideRoutine != null) StopCoroutine(m_HideRoutine);
            GetBinding<TextView>().SetText(text);
            ShowInternal();
            m_HideRoutine = StartCoroutine(HideDelay());
        }

        private IEnumerator HideDelay()
        {
            yield return new WaitForSecondsRealtime(m_AutoHideDelay);
            m_HideRoutine = null;
            HideInternal();
        }
    }
}
