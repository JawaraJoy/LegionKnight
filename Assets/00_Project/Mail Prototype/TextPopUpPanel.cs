using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    public class TextPopUpPanel : PanelView
    {
        [SerializeField]
        private float m_AutoHideDelay = 0f;
        public void ShowText(string text)
        {
            GetBinding<TextView>().SetText(text);
            ShowInternal();
            StartCoroutine(HideDelay());
        }

        private IEnumerator HideDelay()
        {
            yield return new WaitForSeconds(m_AutoHideDelay);
            HideInternal();
        }
    }
}
