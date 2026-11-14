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
        public enum Moment { None, Encounter, Defeated }

        [SerializeField] private NoticeField m_View;
        [SerializeField] private AudioSource m_EncounterSound;
        [SerializeField] private AudioSource m_DefeatedSound;
        private float m_ExpiresAt;
        private Object m_Subject;

        public void Present(string title, string detail, Sprite icon, Color tint, float seconds,
            Moment moment = Moment.None, Object subject = null)
        {
            // A different boss can replace the current banner before its timeout.
            if (m_Subject != subject)
            {
                m_Subject = subject;
                SetSoundActiveInternal(m_EncounterSound, false);
                SetSoundActiveInternal(m_DefeatedSound, false);
            }
            SetSoundActiveInternal(m_EncounterSound, moment == Moment.Encounter);
            SetSoundActiveInternal(m_DefeatedSound, moment == Moment.Defeated);
            m_ExpiresAt = Time.unscaledTime + seconds;
            m_View.Title.text = title;
            m_View.Detail.text = detail;
            m_View.Icon.sprite = icon;
            m_View.Icon.enabled = icon != null;
            m_View.Ribbon.color = tint;
            gameObject.SetActive(true);
        }
        private void SetSoundActiveInternal(AudioSource source, bool active)
        {
            if (source != null && source.gameObject.activeSelf != active)
                source.gameObject.SetActive(active);
        }

        private void Update()
        {
            if (Time.unscaledTime >= m_ExpiresAt) gameObject.SetActive(false);
        }
    }
}
