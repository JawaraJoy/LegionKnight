using LegionKnight;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class CardUnit
    {
        private bool m_IsOwned;
        private bool m_IsAdded;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private CardConfig m_CardConfig;

        public bool IsOwned => m_IsOwned = m_Amount > 0;
        public bool IsAdded => m_IsAdded;
        public int Amount => m_Amount;
        public CardConfig CardConfig => m_CardConfig;

        private string AmountKey => m_CardConfig.BaseInfo.Id + "amount";
        private string UsedKey => m_CardConfig.BaseInfo.Id + "used";

        public CardUnit(CardConfig cardConfig)
        {
            m_CardConfig = cardConfig;
            m_IsOwned = false;
            m_IsAdded = false;
        }

        public void AddAmount(int add)
        {
            m_Amount += add;
            m_IsOwned = m_Amount > 0;
        }

        public void SetIsAdded(bool set)
        {
            m_IsAdded = set;
        }

        public void Init()
        {

            m_IsOwned = m_Amount > 0;
        }
    }
}
