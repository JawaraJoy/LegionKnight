using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Product", menuName = "Legion Knight/IAP/Product", order = 1)]
    public partial class ProductDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private string m_Description;
        [SerializeField]
        private ProductItem m_MainProduct;
        [SerializeField]
        private ProductItem[] m_AdditionalProducts;
        [SerializeField]
        private ProductItem m_BonusProduct;

        public string Id => m_Id;
        public string Name => m_Name;
        public string Description => m_Description;
        public ProductItem MainProduct => m_MainProduct;
        public ProductItem[] AdditionalProducts => m_AdditionalProducts;
        public ProductItem BonusProduct => m_BonusProduct;

        public List<ProductItem> GetAllProduct(bool hasBonus)
        {
            return GetAllProductsInternal(hasBonus);
        }

        private List<ProductItem> GetAllProductsInternal(bool hasBonus)
        {
            List<ProductItem> allProducts = new()
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

    [System.Serializable]
    public partial class ProductItem
    {
        [SerializeField]
        ScriptableObject m_Object;
        [SerializeField]
        private int m_Amount;

        public ScriptableObject Object => m_Object;
        public int Amount => m_Amount;

        public void AddProductToPlayer()
        {
            if (m_Object is CurrencyDefinition currency)
            {
                Player.Instance.AddCurrencyAmount(currency, m_Amount);
            }
            else if (m_Object is CharacterDefinition character)
            {
                Player.Instance.SetOwned(character, true);
            }
            else if (m_Object is StandbyPlatformDefinition plat)
            {
                Player.Instance.AddPlatformAmount(plat, m_Amount);
            }
        }
    }
}
