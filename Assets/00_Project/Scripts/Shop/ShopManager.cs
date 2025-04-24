using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class ShopManager : ShopHandler
    {
        
    }
    public partial class GameManager
    {
        [SerializeField]
        private ShopManager m_ShopManager;
        public void InitShop()
        {
            m_ShopManager.Init();
        }
        private ShopContainer GetShopContainerInternal(string name)
        {
            return m_ShopManager.GetShopContainer(name);
        }
        public void SelectShop(string containerName)
        {
            m_ShopManager.SelectShop(containerName);
        }
        public void OnItemSelectedInvoke(ShopItemDefinition defi)
        {
            m_ShopManager.OnItemSelectedInvoke(defi);
        }
        public void OnCanBuyItemInvoke(ShopItemDefinition defi)
        {
            m_ShopManager.OnCanBuyItemInvoke(defi);
        }
        public void OnCantBuyItemInvoke(ShopItemDefinition defi)
        {
            m_ShopManager.OnCantBuyItemInvoke(defi);
        }
        public void OnItemBoughtInvoke(ShopItemDefinition defi)
        {
            m_ShopManager.OnItemBoughtInvoke(defi);
        }
        public void OnItemBuyInvoke(ShopItemDefinition defi)
        {
            m_ShopManager.OnItemBuyInvoke(defi);
        }
        private ShopItemControl GetShopItemControlInternal(ShopItemDefinition item)
        {
            ShopContainer container = GetShopContainerInternal(item.ContainerName);
            if (container == null)
            {
                Debug.LogError($"Shop container with name {item.ContainerName} not found");
                return null;
            }
            return container.GetShopItemControl(item);
        }
        public void CheckShopAvailableInternal(ShopItemDefinition defi)
        {
            GetShopItemControlInternal(defi).CheckAvailableInternal();
        }
        public ShopItemControl GetShopItemControl(ShopItemDefinition item)
        {
            return GetShopItemControlInternal(item);
        }
        public void SetItemAvaible(ShopItemDefinition item, bool available)
        {
            ShopItemControl control = GetShopItemControlInternal(item);
            if (control == null)
            {
                Debug.LogError($"ShopItemControl not found for {item.name} in {item.ContainerName} and tab {item.TabName}");
                return;
            }
            control.SetAvailable(available);
        }
        public void SetBonusAvaible(ShopItemDefinition item, bool available)
        {
            ShopItemControl control = GetShopItemControlInternal(item);
            if (control == null)
            {
                Debug.LogError($"ShopItemControl not found for {item.name} in {item.ContainerName} and tab {item.TabName}");
                return;
            }
            control.SetBonusAvaible(available);
        }
    }

    public partial class ShopManagerAgent
    {
        public void InitShop()
        {
            GameManager.Instance.InitShop();
        }
        private void SetItemAvailableInternal(ShopItemDefinition item, bool available)
        {
            GameManager.Instance.SetItemAvaible(item, available);
        }
        public void SetItemAvailable(ShopItemDefinition item, bool available)
        {
            SetItemAvailableInternal(item, available);
        }
        public void SetBonusAvaible(ShopItemDefinition item, bool available)
        {
            GameManager.Instance.SetBonusAvaible(item, available);
        }
        public void SetItemAvailableTrue(ShopItemDefinition item)
        {
            SetItemAvailableInternal(item, true);
        }
        public void SetItemAvailableFalse(ShopItemDefinition item)
        {
            SetItemAvailableInternal(item, false);
        }
        public void SelectShop(string containerName)
        {
            GameManager.Instance.SelectShop(containerName);
        }
        public void CheckAvailableInternal(ShopItemDefinition defi)
        {
            GameManager.Instance.CheckShopAvailableInternal(defi);
        }
    }

}
