using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class GachaItemNoteView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_ItemNameAndAmountText;
        [SerializeField]
        private TextMeshProUGUI m_RateText;
        [SerializeField]
        private TextMeshProUGUI m_NotesText;

        [SerializeField, MMReadOnly]
        private GachaRewardConfig m_Item;

        public GachaRewardConfig Item => m_Item;

        public void Init(GachaRewardConfig item)
        {
            m_Item = item;
            string itemName = "Item";
            ScriptableObject defi = m_Item.GachaItemConfig;
            if (defi is IDescriptable des)
            {
                itemName = des.Label;
            }
            string rate = item.Weight * 100 + "%";
            string note = item.GachaItemConfig is IDescriptable desc ? desc.Description : "";
            string amount = item.Amount.ToString();

            m_ItemNameAndAmountText.text = $"{itemName} x{amount}";
            m_RateText.text = rate;
            m_NotesText.text = note;
            
            ShowInternal();
            //string itemName = item.
        }
    }
}
