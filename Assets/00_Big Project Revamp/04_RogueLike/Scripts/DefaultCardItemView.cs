using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    /// <summary>
    /// Satu item card di DefaultCardDeckView.
    /// Read-only — hanya tampil icon + rarity color.
    /// Tap → buka CardDetailView dalam mode ReadOnly.
    /// </summary>
    public class DefaultCardItemView : UIView
    {
        [SerializeField] private Image m_CardIcon;
        [SerializeField] private Image m_RarityColor;
        [SerializeField] private Button m_SelectButton;

        private CardUnit m_CardUnit;

        public void Setup(CardUnit cardUnit)
        {
            m_CardUnit = cardUnit;

            if (m_CardIcon != null)
                m_CardIcon.sprite = cardUnit.CardConfig.CollectibleField.Icon;

            if (m_RarityColor != null)
                m_RarityColor.color = cardUnit.CardConfig.CollectibleField.RarityConfig.Color;

            m_SelectButton.onClick.RemoveAllListeners();
            m_SelectButton.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            if (m_CardUnit == null) return;

            // Buka CardDetailView dalam mode read-only
            CanvasManager.Instance
                .GetPanel<PreparationPanel>()
                .CardTabView
                .CardDetailView
                .ShowReadOnly(m_CardUnit);
        }
    }
}