using Rush;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Product", menuName = "Legion Knight/IAP/Product", order = 1)]
    public partial class ProductDefinition : CollectibleConfig
    {
        [SerializeField]
        private int m_MultipleToOriginal;
        [SerializeField]
        private ProductItemConfig m_MainProduct;
        [SerializeField]
        private ProductItemConfig[] m_AdditionalProducts;
        [SerializeField]
        private ProductItemConfig m_BonusProduct;
        public decimal MultipleToOri => m_MultipleToOriginal;
        public ProductItemConfig MainProduct => m_MainProduct;
        public ProductItemConfig[] AdditionalProducts => m_AdditionalProducts;
        public ProductItemConfig BonusProduct => m_BonusProduct;

        public List<ProductItemConfig> GetAllProduct(bool hasBonus)
        {
            return GetAllProductsInternal(hasBonus);
        }

        
        public List<ProductItemConfig> GetAdditionalProduct()
        {
            return GetAdditionalProductInternal();
        }
        private List<ProductItemConfig> GetAdditionalProductInternal()
        {
            return new List<ProductItemConfig>(m_AdditionalProducts);
        }
        private List<ProductItemConfig> GetAllProductsInternal(bool hasBonus)
        {
            List<ProductItemConfig> allProducts = new()
            {
                m_MainProduct
            };
            allProducts.AddRange(m_AdditionalProducts);
            if (hasBonus && m_BonusProduct != null)
            {
                allProducts.Add(m_BonusProduct);
            }
            return allProducts;
        }

        public void AddAllProductToPlayer(bool hasBonus)
        {
            foreach (var product in GetAllProductsInternal(hasBonus))
            {
                product.AddProductToPlayer();
            }
        }
    }

    
}
