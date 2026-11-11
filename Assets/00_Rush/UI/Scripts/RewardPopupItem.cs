using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class RewardPopupItem : MonoBehaviour
    {
        [Serializable]
        private sealed class CardField
        {
            [SerializeField] private Image m_Icon;
            [SerializeField] private TMP_Text m_Name;
            [SerializeField] private TMP_Text m_Amount;
            [SerializeField] private Image m_TileFrame;
            [SerializeField] private Image m_IconFrame;

            public void SetReward(LootField reward)
            {
                var item = reward.ItemLoot;
                var appearance = item.CollectibleField;
                m_Icon.sprite = appearance?.Icon;
                m_Icon.enabled = m_Icon.sprite != null;
                m_Name.text = item.BaseInfo.Name;
                m_Amount.text = c_AmountPrefix + reward.Amount.ToString();
                Color frameColor = appearance?.RarityConfig != null ? appearance.RarityConfig.Color : Color.white;
                m_TileFrame.color = frameColor;
                m_IconFrame.color = frameColor;
            }
        }

        private const string c_AmountPrefix = "\u00D7";
        [SerializeField] private CardField m_Card = new();

        public void SetReward(LootField reward)
        {
            m_Card.SetReward(reward);
        }
    }
}
