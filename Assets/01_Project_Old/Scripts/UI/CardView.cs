using Rush;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class CardView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_CardNameText;
        [SerializeField]
        private TextMeshProUGUI m_CardDescriptionText;
        [SerializeField]
        private Image m_CardBigIcon;

        // Fired when player selects a card (preview only)
        [SerializeField]
        private UnityEvent<CardConfig> m_OnCardConfigSelected = new();
        // Fired when used card list changes
        [SerializeField]
        private UnityEvent<List<CardConfig>> m_OnUsedCardConfigListChanged = new();

        private void Start() => InitInternal();

        private void InitInternal()
        {
            var usedList = Player.Instance.PlayerCardDeck.GetUsedCardConfig();
            if (usedList != null && usedList.Count > 0)
            {
                var first = usedList[0];
                m_CardBigIcon.sprite = first.CollectibleField.SplashImage;
                m_CardNameText.text = first.BaseInfo.Name;
                m_CardDescriptionText.text = first.BaseInfo.Description;
                OnCardConfigSelectedInvoke(first);
            }
            OnUsedCardConfigListChangedInvoke(usedList);
        }

        // Called when player selects a card (preview only)
        public void SetCardConfigSelected(CardConfig cardConfig)
        {
            m_CardBigIcon.sprite = cardConfig.CollectibleField.SplashImage;
            m_CardNameText.text = cardConfig.BaseInfo.Name;
            m_CardDescriptionText.text = cardConfig.BaseInfo.Description;
            OnCardConfigSelectedInvoke(cardConfig);
        }

        // Called when used card list is updated
        public void SetUsedCardConfigList(List<CardConfig> usedList)
        {
            OnUsedCardConfigListChangedInvoke(usedList);
        }

        private void OnCardConfigSelectedInvoke(CardConfig cardConfig)
            => m_OnCardConfigSelected?.Invoke(cardConfig);

        private void OnUsedCardConfigListChangedInvoke(List<CardConfig> usedList)
            => m_OnUsedCardConfigListChanged?.Invoke(usedList);
    }

    public partial class HeroPanel
    {
        private CardView GetCardView() => GetBinding<CardView>();

        public void SetCardConfigSelected(CardConfig cardConfig)
            => GetCardView().SetCardConfigSelected(cardConfig);

        public void SetUsedCardConfigList(List<CardConfig> usedList)
            => GetCardView().SetUsedCardConfigList(usedList);
    }

    public partial class CanvasManager
    {
        public void SetCardConfigSelected(CardConfig cardConfig)
            => GetPanelInternal<HeroPanel>().SetCardConfigSelected(cardConfig);

        public void SetUsedCardConfigList(List<CardConfig> usedList)
            => GetPanelInternal<HeroPanel>().SetUsedCardConfigList(usedList);
    }
}