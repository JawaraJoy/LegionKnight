using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace LegionKnight
{
    [System.Serializable]
    public partial class SellProduct
    {
        [SerializeField]
        private ProductDefinition m_Definition;
        [SerializeField]
        private bool m_IsAvailable;
        [SerializeField]
        private bool m_IsBonusAvailable;

        private string m_Message;

        public string Message => m_Message;
        public ProductDefinition Definition => m_Definition;
        public bool Available => m_IsAvailable;
        public bool IsBonusAvailable => m_IsBonusAvailable;
        public void Init()
        {
            if (UnityService.Instance.HasData(m_Definition.Id + "a"))
            {
                m_IsAvailable = UnityService.Instance.GetData<bool>(m_Definition.Id);
            }
            else
            {
                UnityService.Instance.SaveData(m_Definition.Id, m_IsAvailable);
            }
            if (UnityService.Instance.HasData(m_Definition.Id + "b"))
            {
                m_IsBonusAvailable = UnityService.Instance.GetData<bool>(m_Definition.Id + "b");
            }
            else
            {
                UnityService.Instance.SaveData(m_Definition.Id + "b", m_IsBonusAvailable);
            }
        }
        public void SetMessage(string message)
        {
            m_Message = message;
        }
        public void AddProductToPlayer()
        {
            if (!m_IsAvailable) return;
            m_Definition.AddAllProductToPlayer(m_IsBonusAvailable);
        }
        public void SetIsAvailable(bool isAvailable)
        {
            m_IsAvailable = isAvailable;
            UnityService.Instance.SaveData(m_Definition.Id + "a", m_IsAvailable);
        }
        public void SetIsBonusAvailable(bool isBonusAvailable)
        {
            m_IsBonusAvailable = isBonusAvailable;
            UnityService.Instance.SaveData(m_Definition.Id + "b", m_IsBonusAvailable);
        }
        public List<ProductItem> GetAllProductItems(bool hasBonus)
        {
            return m_Definition.GetAllProduct(hasBonus);
        }
    }
    public partial class InAppPurchase : MonoBehaviour
    {
        [SerializeField]
        private SellProduct[] m_SellProducts;

        [SerializeField]
        private UnityEvent<SellProduct> m_OnPurchaseComplete = new();
        [SerializeField]
        private UnityEvent<SellProduct> m_OnPurchaseFailed = new();

        public void Init()
        {
            foreach (var product in m_SellProducts)
            {
                product.Init();
            }
        }
        public void OnPurchasedCompleteAddListerner(UnityAction<SellProduct> callback)
        {
            m_OnPurchaseComplete.AddListener(callback);
        }
        public void OnPurchasedFailedAddListerner(UnityAction<SellProduct> callback)
        {
            m_OnPurchaseFailed.AddListener(callback);
        }
        public void OnPurchasedCompleteRemoveListerner(UnityAction<SellProduct> callback)
        {
            m_OnPurchaseComplete.RemoveListener(callback);
        }
        public void OnPurchasedFailedRemoveListerner(UnityAction<SellProduct> callback)
        {
            m_OnPurchaseFailed.RemoveListener(callback);
        }
        private SellProduct GetSellProduct(string id)
        {
            foreach (var product in m_SellProducts)
            {
                if (product.Definition.Id == id)
                {
                    return product;
                }
            }
            return null;
        }
        public void OnPurchaseCompleted(Product product)
        {
            string id = product.definition.id;
            SellProduct sellProduct = GetSellProduct(id);
            sellProduct?.AddProductToPlayer();
            m_OnPurchaseComplete?.Invoke(sellProduct);
            sellProduct.SetIsBonusAvailable(false);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription reason)
        {
            Debug.Log($"Purchase failed for product {product.definition.id}: {reason.reason} : {reason.message}");
            SellProduct sellProduct = GetSellProduct(product.definition.id);
            sellProduct?.SetMessage(reason.message);
            m_OnPurchaseFailed?.Invoke(sellProduct);
        }
        public void SetIsAvailable(string id, bool isAvailable)
        {
            SellProduct sellProduct = GetSellProduct(id);
            sellProduct?.SetIsAvailable(isAvailable);
        }
        public void SetIsBonusAvailable(string id, bool isBonusAvailable)
        {
            SellProduct sellProduct = GetSellProduct(id);
            sellProduct?.SetIsAvailable(isBonusAvailable);
        }
        public bool IsProductAvailable(string id)
        {
            SellProduct sellProduct = GetSellProduct(id);
            return sellProduct != null && sellProduct.Available;
        }
        public bool IsBonusAvailable(string id)
        {
            SellProduct sellProduct = GetSellProduct(id);
            return sellProduct != null && sellProduct.IsBonusAvailable;
        }
    }
}
