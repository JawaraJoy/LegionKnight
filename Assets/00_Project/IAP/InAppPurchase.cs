using UnityEngine;
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

        public ProductDefinition Definition => m_Definition;
        public bool Available => m_IsAvailable;
        public bool IsBonusAvailable => m_IsBonusAvailable;
        public void AddProductToPlayer()
        {
            if (!m_IsAvailable) return;
            m_Definition.AddAllProductToPlayer(m_IsBonusAvailable);
        }
        public void SetIsAvailable(bool isAvailable)
        {
            m_IsAvailable = isAvailable;
        }
        public void SetIsBonusAvailable(bool isBonusAvailable)
        {
            m_IsBonusAvailable = isBonusAvailable;
        }
    }
    public partial class InAppPurchase : MonoBehaviour
    {
        [SerializeField]
        private SellProduct[] m_SellProducts;

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
            if (sellProduct != null)
            {
                sellProduct.AddProductToPlayer();
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription reason)
        {
            Debug.Log($"Purchase failed for product {product.definition.id}: {reason.reason} : {reason.message}");
        }

        public void SetIsAvailable(string id, bool isAvailable)
        {
            SellProduct sellProduct = GetSellProduct(id);
            if (sellProduct != null)
            {
                sellProduct.SetIsAvailable(isAvailable);
            }
        }
        public void SetIsBonusAvailable(string id, bool isBonusAvailable)
        {
            SellProduct sellProduct = GetSellProduct(id);
            if (sellProduct != null)
            {
                sellProduct.SetIsAvailable(isBonusAvailable);
            }
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
