using Rush;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class CardDetailView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_CardNameText;
        [SerializeField]
        private TextMeshProUGUI m_CardDescriptionText;
        [SerializeField]
        private Image m_CardBigIcon;
        [SerializeField]
        private Button m_AddOrRemoveToDeckButton;
        [SerializeField]
        private TextMeshProUGUI m_AddOrRemoveText;

        private CardUnit m_SelectedCard;

        private void Awake()
        {
            Player.Instance.PlayerCardDeck.OnSelectedCard.AddListener(SetCardConfigSelected);
            m_AddOrRemoveToDeckButton.onClick.AddListener(AddOrRemoveButton);
        }
        // Called when player selects a card (preview only)
        private void SetCardConfigSelected(CardUnit cardunit)
        {
            ShowInternal();
            m_SelectedCard = cardunit;
            m_CardBigIcon.sprite = cardunit.CardConfig.CollectibleField.SplashImage;
            m_CardNameText.text = cardunit.CardConfig.BaseInfo.Name;
            m_CardDescriptionText.text = cardunit.CardConfig.BaseInfo.Description;

            bool isAdded = m_SelectedCard.IsAdded;
            if (isAdded)
            {
                m_AddOrRemoveText.text = "Remove";
            }
            else
            {
                m_AddOrRemoveText.text = "Add";
            }
        }
        private void AddOrRemoveButton()
        {
            bool isAdded = m_SelectedCard.IsAdded;
            if (isAdded)
            {
                RemoveCard();
            }
            else
            {
                AddCard();
            }
        }
        private void AddCard()
        {
            Player.Instance.PlayerCardDeck.SetUsedCardConfig();
        }
        private void RemoveCard()
        {
            Player.Instance.PlayerCardDeck.RemoveUsedCardConfig(m_SelectedCard.CardConfig);
        }
    }
}