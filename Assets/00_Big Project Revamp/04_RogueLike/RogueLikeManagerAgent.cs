using UnityEngine;
using UnityEngine.Events;
using LegionKnight;

namespace Rush
{
    public class RogueLikeManagerAgent : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<int> m_OnLevelUp;
        [SerializeField]
        private UnityEvent<CardConfig> m_OnCardCollected;

        private RogueLikeManager m_Handler;
        private RogueLikeCardPanel m_CardPanel;
        private RogueLikeManager Handler
        {
            get
            {
                if (m_Handler == null)
                {
                    m_Handler = RushGameManager.Instance.RogueLikeManager;
                }
                return m_Handler;
            }
        }
        private RogueLikeCardPanel CardPanel
        {
            get
            {
                if (m_CardPanel == null)
                {
                    m_CardPanel = CanvasManager.Instance.GetPanel<RogueLikeCardPanel>();
                }
                return m_CardPanel;
            }
        }
        private void Start()
        {
            Handler.OnLevelUp.AddListener(OnLevelUpInvoke);
            Handler.OnCardCollected.AddListener(OnCardCollectedInvoke);

            
        }
        public void AddExperience(int amount)
        {
            Handler.AddExperience(amount);
        }
        public void ResetProgress()
        {
            Handler.ResetProgress();
        }
        private void OnLevelUpInvoke(int level)
        {
            m_OnLevelUp.Invoke(level);
        }
        private void OnCardCollectedInvoke(CardConfig card)
        {
            m_OnCardCollected.Invoke(card);
        }
        public void ShowCardPanel()
        {
            CardPanel.Show();
        }
    }
}
