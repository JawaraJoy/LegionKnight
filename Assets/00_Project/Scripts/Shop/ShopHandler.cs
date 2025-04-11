using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class ShopHandler : MonoBehaviour
    {
        [SerializeField]
        private List<ShopContainer> m_ShopContainers = new();

        [SerializeField]
        private UnityEvent<ShopContainer> m_OnShopSelected = new();
        [SerializeField]
        private UnityEvent<ShopItemDefinition> m_OnItemSelected = new();
        [SerializeField]
        private UnityEvent<ShopItemDefinition> m_OnCanBuyItem = new();
        [SerializeField]
        private UnityEvent<ShopItemDefinition> m_OnCantBuyItem = new();
        [SerializeField]
        private UnityEvent<ShopItemDefinition> m_OnItemBought = new();

        public void SelectShop(string containerName)
        {
            ShopContainer container = GetShopContainerInternal(containerName);
            container?.Init();
            OnShopSelected(container);
        }
        private void OnShopSelected(ShopContainer container)
        {
            m_OnShopSelected?.Invoke(container);
        }
        public void OnItemSelectedInvoke(ShopItemDefinition defi)
        {
            m_OnItemSelected?.Invoke(defi);
        }
        public void OnCanBuyItemInvoke(ShopItemDefinition defi)
        {
            m_OnCanBuyItem?.Invoke(defi);
        }
        public void OnCantBuyItemInvoke(ShopItemDefinition defi)
        {
            m_OnCantBuyItem?.Invoke(defi);
        }
        public void OnItemBoughtInvoke(ShopItemDefinition defi)
        {
            m_OnItemBought?.Invoke(defi);
        }
        private ShopContainer GetShopContainerInternal(string name)
        {
            foreach (var container in m_ShopContainers)
            {
                if (container.ShopName == name)
                {
                    return container;
                }
            }
            Debug.LogError($"Shop container with name {name} not found");
            return null;
        }
        public ShopContainer GetShopContainer(string name)
        {
            return GetShopContainerInternal(name);
        }
    }
}
