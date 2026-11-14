using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class PlayerLevelRewardRow : MonoBehaviour
    {
        [SerializeField] private Image m_Frame, m_Icon, m_Mark;
        [SerializeField] private TMP_Text m_ItemLabel, m_Rarity;

        public void Display(LootField reward)
        {
            var item = reward.ItemLoot;
            var art = item.CollectibleField;
            var rarity = art?.RarityConfig;
            m_Frame.color = m_Mark.color = rarity != null ? rarity.Color : Color.white;
            m_Icon.sprite = art?.Icon;
            m_Icon.enabled = m_Icon.sprite != null;
            m_Mark.enabled = rarity != null;
            m_Rarity.text = rarity != null ? rarity.BaseInfo.Name : string.Empty;
            string amount = reward.Amount.ToString("N0", CultureInfo.InvariantCulture);
            string name = item.BaseInfo.Name;
            m_ItemLabel.text = name == "Diamond" && reward.Amount != 1 ? amount + " Diamonds" : name + " ×" + amount;
        }
    }
}
