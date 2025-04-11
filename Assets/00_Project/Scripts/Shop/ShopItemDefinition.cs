using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Shop Item", menuName = "Legion Knight/Shop Item")]
    public partial class ShopItemDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_ItemName;
        [SerializeField]
        private string m_ContainerName;
        [SerializeField]
        private string m_TabName;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private Object m_ItemToBuy;
        [SerializeField]
        private Object m_ItemBonus;
        [SerializeField]
        private string m_BonusDescription;
        [SerializeField]
        private CurrencyDefinition m_CurrencyDefinition;
        [SerializeField]
        private int m_Price;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private int m_BonusAmount;

        [SerializeField]
        private string m_BuyButtonText;

        [SerializeField]
        private CurrencyDefinition m_SpendRewardDefinition;
        [SerializeField]
        private int m_SpendRewardAmount;

        public string ItemName => m_ItemName;
        public string ContainerName => m_ContainerName;
        public string TabName => m_TabName;
        public string BuyButtonText => m_BuyButtonText;
        public Sprite Icon => m_Icon;
        public int Price => m_Price;
        public CurrencyDefinition Currency => m_CurrencyDefinition;
        public int Amount => m_Amount;
        public int BonusAmount => m_BonusAmount;
        public int SpendRewardAmount => m_SpendRewardAmount;
        public Object ItemToBuy => m_ItemToBuy;
        public Object ItemBonus => m_ItemBonus;
        public string BonusDescription => m_BonusDescription;

        private UnityAction m_OnBought;
        public bool CanBuy()
        {
            return GetPlayerCurrencyAmount() >= m_Price;
        }
        private int GetPlayerCurrencyAmount()
        {
            return Player.Instance.GetCurrencyAmount(m_CurrencyDefinition);
        }

        public void TryBuy(UnityAction onBought)
        {
            if (CanBuy())
            {
                //Player.Instance.AddCurrencyAmount(m_CurrencyDefinition, -m_Price);
                //Player.Instance.AddCurrencyAmount(m_SpendRewardDefinition, m_SpendRewardAmount);
                //AddItemToPlayer(m_ItemToBuy);
                GameManager.Instance.OnCanBuyItemInvoke(this);
                m_OnBought += onBought;
            }
            else
            {
                m_OnBought -= onBought;
                GameManager.Instance.OnCantBuyItemInvoke(this);
            }
            GameManager.Instance.OnItemSelectedInvoke(this);
        }
        public void Buy()
        {
            Player.Instance.AddCurrencyAmount(m_CurrencyDefinition, -m_Price);
            Player.Instance.AddCurrencyAmount(m_SpendRewardDefinition, m_SpendRewardAmount);
            AddItemToPlayer(m_ItemToBuy);
            
            if (GameManager.Instance.GetShopItemControl(this).IsBonusAvaible)
            {
                AddBonusItemToPlayer(m_ItemBonus);
            }
            m_OnBought?.Invoke();
            GameManager.Instance.OnItemBoughtInvoke(this);
        }

        private void AddItemToPlayer(Object item)
        {
            if (item is CharacterDefinition itemDefinition)
            {
                Player.Instance.SetOwned(itemDefinition, true);
            }
            else if (item is CurrencyDefinition currencyDefinition)
            {
                Player.Instance.AddCurrencyAmount(currencyDefinition, m_Amount);
            }
            else
            {
                Debug.LogError($"Unsupported item type: {item.GetType()}");
            }
        }
        private void AddBonusItemToPlayer(Object item)
        {
            if (item is CharacterDefinition itemDefinition)
            {
                Player.Instance.SetOwned(itemDefinition, true);
            }
            else if (item is CurrencyDefinition currencyDefinition)
            {
                Player.Instance.AddCurrencyAmount(currencyDefinition, m_BonusAmount);
            }
            else
            {
                Debug.LogError($"Unsupported item type: {item.GetType()}");
            }
        }
    }
}
