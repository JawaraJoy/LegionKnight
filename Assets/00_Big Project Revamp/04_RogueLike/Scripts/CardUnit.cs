using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class CardUnit
    {
        [SerializeField, MMReadOnly]
        private bool m_IsOwned;
        [SerializeField, MMReadOnly]
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
            UnityService.Instance.SaveData(AmountKey, m_Amount);
            m_IsOwned = m_Amount > 0;
        }

        public void SetIsAdded(bool set)
        {
            m_IsAdded = set;
            UnityService.Instance.SaveData(UsedKey, m_IsAdded);
        }

        public void Init()
        {
            if (UnityService.Instance.HasData(AmountKey))
                m_Amount = UnityService.Instance.GetData<int>(AmountKey);

            if (UnityService.Instance.HasData(UsedKey))
                m_IsAdded = UnityService.Instance.GetData<bool>(UsedKey);

            m_IsOwned = m_Amount > 0;
        }
    }
}
