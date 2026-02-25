using UnityEngine;
using UnityEngine.Events;
using Rush;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Shop Item", menuName = "Legion Knight/Shop Item")]
    public partial class ShopItemDefinition : CollectibleConfig
    {
        private string m_ContainerName;
        [SerializeField]
        private string m_TabName;
        [SerializeField]
        private bool m_WatchAdOveride;
        [SerializeField]
        private CollectibleConfig m_ItemToBuy;
        [SerializeField]
        private CollectibleConfig m_ItemBonus;
        [SerializeField]
        private string m_BonusDescription;
        [SerializeField]
        private ItemConfig m_ItemCost;
        [SerializeField]
        private int m_Price;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private int m_BonusAmount;

        [SerializeField]
        private string m_BuyButtonText;

        [SerializeField]
        private ItemConfig m_ItemRewardOnSpending;
        [SerializeField]
        private int m_SpendRewardAmount;

        public string ContainerName => m_ContainerName;
        public string TabName => m_TabName;
        public string BuyButtonText => m_BuyButtonText;
        public int Price => m_Price;
        public ItemConfig ItemCost => m_ItemCost;
        public int Amount => m_Amount;
        public int BonusAmount => m_BonusAmount;
        public int SpendRewardAmount => m_SpendRewardAmount;
        public CollectibleConfig ItemToBuy => m_ItemToBuy;
        public CollectibleConfig ItemBonus => m_ItemBonus;
        public string BonusDescription => m_BonusDescription;

        private UnityAction m_OnBought;
        public bool CanBuy()
        {
            return GetPlayerCurrencyAmount() >= m_Price;
        }
        private int GetPlayerCurrencyAmount()
        {
            return Player.Instance.CurrencyControl.GetCurrencyAmount(m_ItemCost);
        }

        public void TryBuy(UnityAction onBought)
        {
            if (CanBuy())
            {
                GameManager.Instance.ShopManager.OnCanBuyItemInvoke(this);
                m_OnBought += onBought;
            }
            else
            {
                m_OnBought -= onBought;
                GameManager.Instance.ShopManager.OnCantBuyItemInvoke(this);
            }
            GameManager.Instance.ShopManager.OnItemSelectedInvoke(this);
        }

        public void Buy()
        {
            //GameManager.Instance.OnItemBuyInvoke(this);
            //GameManager.Instance.SetBonusAvaible(this, false);
            
            if (m_WatchAdOveride)
            {
                Watch();
            }
            else
            {
                Player.Instance.CurrencyControl.AddCurrencyAmount(m_ItemCost, -m_Price);
                AddItemToPlayer(m_ItemToBuy);
            }
        }
        private void Watch()
        {
            //UnityService.Instance.LoadRewardedAd();
            UnityService.Instance.ShowRewardedAd(() => AddItemToPlayer(m_ItemToBuy));
        }

        private void AddItemToPlayer(CollectibleConfig item)
        {
            GameManager.Instance.ShopManager.OnItemBuyInvoke(this);
            if (item is HeroUnitConfig heroconfig)
            {
                if (Player.Instance.HeroDeck.GetHeroUnit(heroconfig).Owned)
                {
                    //GameManager.Instance.AddStarConvertCount(itemDefinition.ShardConvert.Amount);
                    Currency shard = new(heroconfig.ItemDuplicateConverter.ItemConfig, heroconfig.ItemDuplicateConverter.Amount);
                    Player.Instance.CurrencyControl.AddCurrencyAmount(shard.ItemConfig, shard.Amount);
                }
                else
                {
                    Player.Instance.HeroDeck.SetOwned(heroconfig, true);
                }
            }
            if (item is ItemConfig currencyDefinition)
            {
                Player.Instance.CurrencyControl.AddCurrencyAmount(currencyDefinition, m_Amount);
            }
            if (item is PlatformConfig platformConfig)
            {
                Player.Instance.PlatformDeck.AddPlatformAmount(platformConfig, m_Amount);
            }
            else
            {
                Debug.LogError($"Unsupported item type: {item.GetType()}");
            }

            if (GameManager.Instance.ShopManager.GetShopContainer(m_ContainerName).GetShopItemControl(this).IsBonusAvaible && m_ItemBonus != null)
            {
                AddBonusItemToPlayer(m_ItemBonus);
            }
            m_OnBought?.Invoke();
            Player.Instance.CurrencyControl.AddCurrencyAmount(m_ItemRewardOnSpending, m_SpendRewardAmount);
            GameManager.Instance.ShopManager.OnItemBoughtInvoke(this);
        }
        private void AddBonusItemToPlayer(CollectibleConfig item)
        {
            if (item is HeroUnitConfig heroConfig)
            {
                Player.Instance.HeroDeck.SetOwned(heroConfig, true);
            }
            if (item is ItemConfig itemConfig)
            {
                Player.Instance.CurrencyControl.AddCurrencyAmount(itemConfig, m_BonusAmount);
            }
            if (item is PlatformConfig platformConfig)
            {
                Player.Instance.PlatformDeck.AddPlatformAmount(platformConfig, m_Amount);
            }
            else
            {
                Debug.LogError($"Unsupported item type: {item.GetType()}");
            }
        }
    }
}
