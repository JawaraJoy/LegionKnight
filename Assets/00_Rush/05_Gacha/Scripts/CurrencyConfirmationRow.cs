using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class CurrencyConfirmationRow : MonoBehaviour
    {
        [System.Serializable]
        private sealed class ItemField
        {
            [SerializeField] private Image m_Frame;
            [SerializeField] private Image m_Icon;
            [SerializeField] private TMP_Text m_Name;
            [SerializeField] private TMP_Text m_Amount;
            [SerializeField] private GameObject m_Divider;

            public void Bind(string name, Sprite icon, RarityConfig rarity, string amount, bool divider)
            {
                m_Frame.color = rarity != null ? rarity.Color : Color.white;
                m_Icon.sprite = icon;
                m_Icon.enabled = icon != null;
                m_Name.text = name;
                m_Amount.text = amount;
                m_Divider.SetActive(divider);
            }
        }

        [SerializeField] private ItemField m_Item;
        internal void Bind(string name, Sprite icon, RarityConfig rarity, string amount, bool divider) =>
            m_Item.Bind(name, icon, rarity, amount, divider);
    }
}
