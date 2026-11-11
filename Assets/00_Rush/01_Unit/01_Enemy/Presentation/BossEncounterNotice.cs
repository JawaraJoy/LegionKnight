using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class BossEncounterNotice : MonoBehaviour
    {
        [System.Serializable]
        private class NoticeField
        {
            [SerializeField] private Image m_Icon;
            [SerializeField] private Image m_Ribbon;
            [SerializeField] private TMP_Text m_Title;
            [SerializeField] private TMP_Text m_Detail;
            public Image Icon => m_Icon;
            public Image Ribbon => m_Ribbon;
            public TMP_Text Title => m_Title;
            public TMP_Text Detail => m_Detail;
        }
        [SerializeField] private NoticeField m_View;
        private float m_ExpiresAt;
        public void Present(string title, string detail, Sprite icon, Color tint, float seconds)
        {
            m_ExpiresAt = Time.unscaledTime + seconds;
            m_View.Title.text = title;
            m_View.Detail.text = detail;
            m_View.Icon.sprite = icon;
            m_View.Icon.enabled = icon != null;
            m_View.Ribbon.color = tint;
            gameObject.SetActive(true);
        }
        private void Update()
        {
            if (Time.unscaledTime >= m_ExpiresAt) gameObject.SetActive(false);
        }
    }
}
