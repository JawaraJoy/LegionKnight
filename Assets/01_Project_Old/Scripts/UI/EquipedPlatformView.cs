using MoreMountains.Tools;
using Rush;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

namespace LegionKnight
{
    public class EquippedCardView : UIView
    {
        [SerializeField]
        private Image m_CardEquipedIcon;
        [SerializeField]
        private UnityEvent<CardUnit> m_OnCardEquiped;
        [SerializeField, MMReadOnly]
        private CardConfig m_SelectedCardConfig;

        public void SetCardConfigSelected(CardConfig cardConfig)
        {
            m_SelectedCardConfig = cardConfig;
        }

        public void Equip()
        {
            if (m_SelectedCardConfig == null) return;

            CardUnit unit = Player.Instance.PlayerCardDeck.GetCardOwned(m_SelectedCardConfig);
            Player.Instance.PlayerCardDeck.SetIsEquiped(m_SelectedCardConfig, true);
            m_OnCardEquiped.Invoke(unit);
            m_CardEquipedIcon.sprite = m_SelectedCardConfig.CollectibleField.Icon;
        }

        public void Init()
        {
            List<CardConfig> usedList = Player.Instance.PlayerCardDeck.GetUsedCardConfig();

            if (usedList == null || usedList.Count == 0)
            {
                HideInternal();
                return;
            }

            // Display icon from the first owned card in the used list
            bool anyOwned = false;
            foreach (var cardConfig in usedList)
            {
                if (Player.Instance.PlayerCardDeck.IsCardOwned(cardConfig))
                {
                    ShowInternal();
                    m_CardEquipedIcon.sprite = cardConfig.CollectibleField.Icon;
                    anyOwned = true;
                    break;
                }
            }

            if (!anyOwned)
            {
                HideInternal();
            }
        }
    }
}