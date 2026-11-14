using LegionKnight;
using TMPro;
using UnityEngine;

namespace Rush
{
    public sealed class AlbumCardCaption : MonoBehaviour
    {
        [SerializeField] private CardSelectView m_Card;
        [SerializeField] private TMP_Text m_Copies;
        [SerializeField] private TextMeshProUGUI m_Caption;

        [SerializeField] private GameObject m_SelectedTick;

        public void SetSelected(CardConfig card)
        {
            if (m_SelectedTick != null) m_SelectedTick.SetActive(card != null && m_Card.CardConfig == card);
        }

        public void Display(CardUnit unit)
        {
            m_Caption.text = unit.CardConfig.BaseInfo.Name;
            m_Copies.text = $"×{unit.Amount:N0}";
        }
    }
}
