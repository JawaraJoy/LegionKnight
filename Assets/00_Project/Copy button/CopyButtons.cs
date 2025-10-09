using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class CopyButtons : UIView
    {
        [SerializeField] 
        private TextMeshProUGUI m_TextToCopy;
        [SerializeField] 
        private Button m_CopyButton;

        [SerializeField]
        private TextView m_TextView;

        private void Awake()
        {
            m_CopyButton.onClick.AddListener(() =>
            {
                CopyText();
            });
        }
        private void CopyText()
        {
            GUIUtility.systemCopyBuffer = m_TextToCopy.text;
            StartCoroutine(ShowCopiedMessage());
        }

        private IEnumerator ShowCopiedMessage()
        {
            Debug.Log("Copied!");
            m_TextView.Show();
            m_TextView.SetText("Content is Copied");
            yield return new WaitForSeconds(1f);
            m_TextView.Hide();
            // Hide "Copied!" message or reset UI
        }
    }
}
