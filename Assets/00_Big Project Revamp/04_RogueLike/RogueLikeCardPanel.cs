using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class RogueLikeCardPanel : PanelView
    {
        [SerializeField]
        private RLCardView[] m_CardViews;

        private void Start()
        {
            foreach (var card in m_CardViews)
            {
                Button button = card.SelectButton;
                button.onClick.AddListener(HideInternal);
            }
        }
        private void ShowCards(CardConfig[] cardConfigs)
        {
            for (int i = 0; i < m_CardViews.Length; i++)
            {
                if (i < cardConfigs.Length)
                {
                    m_CardViews[i].ShowCard(cardConfigs[i]);
                }
            }
        }
        override protected void ShowInternal()
        {
            // Additional logic for showing the card panel can be added here
            
            base.ShowInternal();
            RefreshCardsInternal();
        }
        public void RefreshCards()
        {
            RefreshCardsInternal();
        }
        private void RefreshCardsInternal()
        {
            int drawAmount = m_CardViews.Length; // Assuming you want to draw as many cards as there are views
            CardConfig[] cardConfigs = RushGameManager.Instance.RogueLikeManager.Config.GetDifferenceCardRandom(drawAmount);
            ShowCards(cardConfigs);
        }
    }
}
