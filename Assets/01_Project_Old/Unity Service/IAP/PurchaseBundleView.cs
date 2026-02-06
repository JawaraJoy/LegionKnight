using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Purchasing;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class PurchaseBundleView : UIView
    {
        [SerializeField]
        private ProductDefinition m_ProductDefinition;
        [SerializeField]
        private AssetReferenceGameObject m_ItemViewPrefab;

        [SerializeField]
        private TextMeshProUGUI m_LabelText;
        [SerializeField]
        private TextMeshProUGUI m_PriceText;
        [SerializeField]
        private TextMeshProUGUI m_OriginalPriceText;
        [SerializeField]
        private TextMeshProUGUI m_DiscountText;
        [SerializeField]
        private TextMeshProUGUI m_DescriptionText;
        [SerializeField]
        private ItemView m_MainItemView;
        [SerializeField]
        private ItemView m_BonusItemView;
        [SerializeField]
        private GameObject m_NotAvailableView;
        [SerializeField]
        private Transform m_ItemViewSpawn;
        [SerializeField]
        private Button m_PurchaseButton;

        private readonly List<ItemView> m_SpawnedAdditionalItemViews = new();

        private void OnEnable()
        {
            InitInternal(m_ProductDefinition);
        }
        private void OnDisable()
        {
            ClearItemView();
        }

        public void OnProductFecth(Product product)
        {
            decimal originalPrice = product.metadata.localizedPrice * m_ProductDefinition.MultipleToOri;
            decimal discountedPrice = product.metadata.localizedPrice;
            string samplePriceString = product.metadata.localizedPriceString;

            //bool isDiscounted = discountedPrice < originalPrice;

            string currencyCode = product.metadata.isoCurrencyCode;

            int discountPercent = Mathf.RoundToInt((float)((originalPrice - discountedPrice) / originalPrice * 100m));

            // Discounted price (use store localized string!)
            m_PriceText.text = product.metadata.localizedPriceString;

            // Original price (strikethrough)
            m_OriginalPriceText.text = $"<s>{FormatLikeStore(originalPrice, currencyCode)}</s>";

            m_DiscountText.text = $"-{discountPercent}%";

            
        }

        string FormatLikeStore(decimal value, string currencyCode)
        {
            var culture = CultureInfo
                .GetCultures(CultureTypes.SpecificCultures)
                .FirstOrDefault(c =>
                {
                    try
                    {
                        return new RegionInfo(c.LCID).ISOCurrencySymbol == currencyCode;
                    }
                    catch { return false; }
                });

            if (culture == null)
                return value.ToString("F0");

            var nfi = (NumberFormatInfo)culture.NumberFormat.Clone();

            // 🔥 HARD OVERRIDE for store-like formatting
            nfi.CurrencyDecimalDigits = IsZeroDecimalCurrency(currencyCode) ? 0 : 2;

            return value.ToString("C", nfi);
        }


        int GetDecimalCount(string priceString)
        {
            if (priceString.Contains(".") || priceString.Contains(","))
            {
                char separator = priceString.Contains(".") ? '.' : ',';
                return priceString.Split(separator).Last().Length;
            }
            return 0;
        }

        bool IsZeroDecimalCurrency(string iso)
        {
            switch (iso)
            {
                case "IDR":
                case "JPY":
                case "KRW":
                case "VND":
                    return true;
                default:
                    return false;
            }
        }


        public void Init()
        {
            InitInternal(m_ProductDefinition);
        }

        private void InitInternal(ProductDefinition defi)
        {
            m_ProductDefinition = defi;
            
            ClearItemView();
            m_LabelText.text = m_ProductDefinition.Label;
            if (m_DescriptionText != null)
            {
                m_DescriptionText.text = m_ProductDefinition.Description;
            }    
            if (GetAdditionalProductInternal().Count > 0)
            {
                SpawnAddionalItemView();
            }
            m_MainItemView.Init(GetMainProductInternal());
            m_BonusItemView.Init(GetBonusProductInternal());

            bool available = UnityService.Instance.IsProductAvailable(m_ProductDefinition.Id);
            bool hasBonus = UnityService.Instance.IsBonusAvailable(m_ProductDefinition.Id);

            if (hasBonus)
            {
                m_BonusItemView.Show();
            }
            else
            {
                m_BonusItemView.Hide();
            }

            SetAvailableInternal(available);
        }

        public void SetAvailable(bool set)
        {
            SetAvailableInternal(set);
        }
        private void SetAvailableInternal(bool set)
        {
            UnityService.Instance.SetIsAvailablePurchase(m_ProductDefinition.Id, set);
            bool available = UnityService.Instance.IsProductAvailable(m_ProductDefinition.Id);
            m_NotAvailableView.SetActive(!available);
            m_PurchaseButton.interactable = available;
        }

        private async void SpawnAddionalItemView()
        {
            List<ProductItem> productItems = GetAdditionalProductInternal();
            foreach (var item in productItems)
            {
                var handle = m_ItemViewPrefab.InstantiateAsync(m_ItemViewSpawn);
                await handle.Task;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject instantiatedObject = handle.Result;

                    if (instantiatedObject.TryGetComponent(out ItemView itemView))
                    {
                        itemView.gameObject.SetActive(true);
                        itemView.Init(item);
                        m_SpawnedAdditionalItemViews.Add(itemView);
                    }
                    else
                    {
                        Debug.LogError($"ItemView component not found on instantiated object: {instantiatedObject.name}");
                    }
                }
            }
        }
        private void ClearItemView()
        {
            foreach (var itemView in m_SpawnedAdditionalItemViews)
            {
                itemView.gameObject.SetActive(false);
                Destroy(itemView.gameObject);
            }
            m_SpawnedAdditionalItemViews.Clear();
        }

        private List<ProductItem> GetProductItemsInternal()
        {
            bool hasBonus = UnityService.Instance.IsBonusAvailable(m_ProductDefinition.Id);
            return m_ProductDefinition.GetAllProduct(hasBonus);
        }

        private List<ProductItem> GetAdditionalProductInternal()
        {
            return m_ProductDefinition.GetAdditionalProduct();
        }
        private ProductItem GetMainProductInternal()
        {
            return m_ProductDefinition.MainProduct;
        }
        private ProductItem GetBonusProductInternal()
        {
            return m_ProductDefinition.BonusProduct;
        }
    }
}
