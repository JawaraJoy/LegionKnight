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
        public void ShowCards(CardConfig[] cardConfigs)
        {
            for (int i = 0; i < m_CardViews.Length; i++)
            {
                if (i < cardConfigs.Length)
                {
                    m_CardViews[i].Initialize(cardConfigs[i]);
                }
            }
            ShowInternal();
        }
    }
}
