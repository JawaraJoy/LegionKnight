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

        // ── Setup ─────────────────────────────────────────────────────────────
        public void SetEmpty()
        {
            m_OccupiedCard = null;
            m_CardIcon.sprite = m_DefaultEmptySprite;

            m_SlotButton.onClick.RemoveAllListeners();
            m_SlotButton.interactable = false;
        }

        public void SetFilled(CardUnit cardUnit)
        {
            m_OccupiedCard = cardUnit;

            if (m_CardIcon != null)
                m_CardIcon.sprite = cardUnit.CardConfig.CollectibleField.Icon;
            if (m_CardFrame != null)
                m_CardFrame.color = cardUnit.CardConfig.CollectibleField.RarityConfig.Color;

            m_SlotButton.interactable = true;
            m_SlotButton.onClick.RemoveAllListeners();
            m_SlotButton.onClick.AddListener(OnSlotClicked);
        }

        // ── Tap slot terisi → buka detail untuk remove ────────────────────────
        private void OnSlotClicked()
        {
            if (m_OccupiedCard == null) return;

            // Set sebagai selected card lalu CardDetailView akan terbuka via event
            Player.Instance.PlayerCardDeck.SelectStandbyCardConfig(m_OccupiedCard.CardConfig);
        }
    }
}