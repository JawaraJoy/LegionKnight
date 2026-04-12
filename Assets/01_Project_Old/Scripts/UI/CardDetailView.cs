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

        private CardUnit m_SelectedCard;

        private void Awake()
        {
            Player.Instance.PlayerCardDeck.OnSelectedCard.AddListener(SetCardConfigSelected);
        }
        // Called when player selects a card (preview only)
        public void SetCardConfigSelected(CardUnit cardunit)
        {
            m_CardBigIcon.sprite = cardunit.CardConfig.CollectibleField.SplashImage;
            m_CardNameText.text = cardunit.CardConfig.BaseInfo.Name;
            m_CardDescriptionText.text = cardunit.CardConfig.BaseInfo.Description;
        }
    }
}