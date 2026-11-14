using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    /// <summary>
    /// Satu slot di deck bar.
    /// - Kosong  : m_EmptyContent aktif, m_FilledContent nonaktif
    /// - Terisi  : m_FilledContent aktif, tampilkan icon card
    /// - Tap slot terisi → SelectStandbyCardConfig → CardDetailView terbuka
    /// </summary>
    public class CardSlotView : UIView
    {
        [SerializeField]
        private Sprite m_DefaultEmptySprite;
        [SerializeField] private Image m_CardIcon;
        [SerializeField]
        private Image m_CardFrame;
        [SerializeField] private Button m_SlotButton;

        private CardUnit m_OccupiedCard;
        [SerializeField] private GameObject m_EmptyMark;
        private PlatformAlbum m_Album;
        [SerializeField] private TMPro.TMP_Text m_SlotLabel;
        [SerializeField] private TMPro.TMP_Text m_Copies;
        private int m_Index;
        public void BindAlbum(PlatformAlbum album, int index)
        {
            m_Album = album;
            m_Index = index;
        }

        // ── Setup ─────────────────────────────────────────────────────────────
public void SetEmpty()
        {
            m_OccupiedCard = null;
            if (m_Copies != null) m_Copies.transform.parent.gameObject.SetActive(false);
            if (m_SlotLabel != null) m_SlotLabel.text = "Slot " + (m_Index + 1);
            m_CardIcon.sprite = m_DefaultEmptySprite;
            m_CardIcon.enabled = m_EmptyMark == null;
            if (m_EmptyMark != null) m_EmptyMark.SetActive(true);

            m_SlotButton.onClick.RemoveListener(OnSlotClicked);
            m_SlotButton.interactable = m_Album != null;
            if (m_Album != null) m_SlotButton.onClick.AddListener(OnSlotClicked);
            if (m_CardFrame != null) m_CardFrame.color = Color.white;
        }

        public void SetFilled(CardUnit cardUnit)
        {
            m_OccupiedCard = cardUnit;
            if (m_Copies != null)
            {
                m_Copies.text = $"×{cardUnit.Amount:N0}";
                m_Copies.transform.parent.gameObject.SetActive(true);
            }
            if (m_SlotLabel != null) m_SlotLabel.text = $"{cardUnit.CardConfig.BaseInfo.Name}  +{cardUnit.Upgrade}";
            if (m_EmptyMark != null) m_EmptyMark.SetActive(false);
            if (m_CardIcon != null) m_CardIcon.enabled = true;

            if (m_CardIcon != null)
                m_CardIcon.sprite = cardUnit.CardConfig.CollectibleField.Icon;
            if (m_CardFrame != null)
                m_CardFrame.color = cardUnit.CardConfig.CollectibleField.RarityConfig.Color;

            m_SlotButton.interactable = true;
            m_SlotButton.onClick.RemoveListener(OnSlotClicked);
            m_SlotButton.onClick.AddListener(OnSlotClicked);
        }

        // ── Tap slot terisi → buka detail untuk remove ────────────────────────
        private void OnSlotClicked()
        {
            if (m_OccupiedCard == null)
            {
                if (m_Album != null) m_Album.OpenSlot(m_Index, null);
                return;
            }

            // Set sebagai selected card lalu CardDetailView akan terbuka via event
            if (m_Album != null) m_Album.OpenSlot(m_Index, m_OccupiedCard);
        }
    

private void OnDestroy()
        {
            if (m_SlotButton != null)
            {
                m_SlotButton.onClick.RemoveListener(OnSlotClicked);
            }
        }
}
}

