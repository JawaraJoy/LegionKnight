using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class SpinWheelResultPanel : CollectibleResultPanel
    {
        [Serializable]
        private sealed class CardField
        {
            [SerializeField] private GameObject m_Group;
            [SerializeField] private Image m_Icon;
            public void SetVisible(bool visible) => m_Group.SetActive(visible);
            [SerializeField] private Image m_Frame;
            [SerializeField] private Image m_RarityDot;
            [SerializeField] private TMP_Text m_Rarity;
            [SerializeField] private TMP_Text m_Name;
            [SerializeField] private TMP_Text m_Amount;

            public void Bind(CollectibleResultEntry entry)
            {
                m_Group.SetActive(true);
                var collectible = entry.Collectible;
                var info = collectible.CollectibleField;
                var rarity = info.RarityConfig;
                m_Icon.sprite = info.Icon;
                m_Icon.enabled = info.Icon != null;
                m_Frame.color = rarity != null ? rarity.Color : Color.white;
                m_RarityDot.color = m_Frame.color;
                m_RarityDot.enabled = rarity != null;
                m_Rarity.text = rarity != null ? rarity.BaseInfo.Name.ToUpperInvariant() : string.Empty;
                m_Name.text = collectible.BaseInfo.Name;
                m_Amount.text = $"×{entry.Amount}";
            }
        }

        [SerializeField] private CardField m_Card;
        protected override void ShowInternal()
        {
            m_Card.SetVisible(false);
            base.ShowInternal();
        }

        protected override void SpawnEntryInternal(CollectibleResultEntry entry) => m_Card.Bind(entry);
    }
}
