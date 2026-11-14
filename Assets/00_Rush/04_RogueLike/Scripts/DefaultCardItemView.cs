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

        [SerializeField] private TMPro.TextMeshProUGUI m_Caption;
        [SerializeField] private TMPro.TMP_Text m_Copies;
        private CardUnit m_CardUnit;

        [SerializeField] private TMPro.TMP_Text m_Role;
        private static readonly string[] s_Roles = { "Basic", "Passive", "Ultimate", "Platform" };

        public void Setup(CardUnit cardUnit, int index)
        {
            m_CardUnit = cardUnit;
            if (m_Copies != null) m_Copies.text = $"×{cardUnit.Amount:N0}";
            if (m_Role != null) m_Role.text = index < s_Roles.Length ? s_Roles[index] : string.Empty;
            if (m_Caption != null) m_Caption.text = $"{cardUnit.CardConfig.BaseInfo.Name}  +{cardUnit.Upgrade}";

            if (m_CardIcon != null)
                m_CardIcon.sprite = cardUnit.CardConfig.CollectibleField.Icon;

            if (m_RarityColor != null)
                m_RarityColor.color = cardUnit.CardConfig.CollectibleField.RarityConfig.Color;

            m_SelectButton.onClick.RemoveListener(OnClicked);
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
    

private void OnDestroy()
        {
            if (m_SelectButton != null)
            {
                m_SelectButton.onClick.RemoveListener(OnClicked);
            }
        }
}
}

