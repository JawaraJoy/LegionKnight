using UnityEngine;
using UnityEngine.Events;
using LegionKnight;

namespace Rush
{
    public class RogueLikeManagerAgent : MonoBehaviour, IReseter
    {
        [SerializeField]
        private RogueLikeForProgressType m_For = RogueLikeForProgressType.Player;
        [SerializeField]
        private int m_ExpMultiplyByPerfect = 1;
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
            if (m_For == RogueLikeForProgressType.Player)
            {
                Handler.OnForPlayerLevelUp.AddListener(OnLevelUpInvoke);
            }
            else
            {
                Handler.OnForBossLevelUp.AddListener(OnLevelUpInvoke);
            }

            Handler.OnCardCollected.AddListener(OnCardCollectedInvoke);
        }
        public void AddExperience(int perfectCombo)
        {
            int totalAmount = (perfectCombo + 1) * m_ExpMultiplyByPerfect;
            if (m_For == RogueLikeForProgressType.Player)
            {
                Handler.AddForPlayerExperience(totalAmount);
            }
            else
            {
                Handler.AddForBossExperience(totalAmount);
            }
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

        public void ResetProgression()
        {
            Handler.ResetProgression();
        }
    }
    public enum RogueLikeForProgressType
    {
        Player,
        Enemy,
    }
}
