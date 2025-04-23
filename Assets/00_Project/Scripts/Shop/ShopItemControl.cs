using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class ShopItemControl
    {
        [SerializeField]
        private bool m_IsAvailable;
        [SerializeField]
        private bool m_IsBonusAvaible;
        [SerializeField]
        private ShopItemDefinition m_ShopItem;
        public ShopItemDefinition ShopItem => m_ShopItem;
        public bool IsAvailable => m_IsAvailable;
        public bool IsBonusAvaible => m_IsBonusAvaible;
        public Sprite Icon => m_ShopItem.Icon;
        public int Price => m_ShopItem.Price;
        public CurrencyDefinition Currency => m_ShopItem.Currency;
        public int Amount => m_ShopItem.Amount;
        public int BonusAmount => m_ShopItem.BonusAmount;
        public int ShopingPointReward => m_ShopItem.SpendRewardAmount;
        public Object ItemToBuy => m_ShopItem.ItemToBuy;
        public Object ItemBonus => m_ShopItem.ItemBonus;

        private string IdInternal => m_ShopItem.Id;

        public void InitInternal()
        {
            UnityService.Instance.LoadData(IdInternal + "a");
            UnityService.Instance.LoadData(IdInternal + "b");
            if (UnityService.Instance.HasData(IdInternal + "a"))
            {
                m_IsAvailable = UnityService.Instance.GetData<bool>(IdInternal + "a");
            }
            if (UnityService.Instance.HasData(IdInternal + "b"))
            {
                m_IsBonusAvaible = UnityService.Instance.GetData<bool>(IdInternal + "b");
            }
        }
        public void SetAvailable(bool available)
        {
            m_IsAvailable = available;
            UnityService.Instance.SaveData(IdInternal + "a", m_IsAvailable);
        }
        public void SetShopItem(ShopItemDefinition shopItem)
        {
            m_ShopItem = shopItem;
        }
        public void SetBonusAvaible(bool available)
        {
            m_IsBonusAvaible = available;
            UnityService.Instance.SaveData(IdInternal + "b", m_IsBonusAvaible);
        }
    }
}
