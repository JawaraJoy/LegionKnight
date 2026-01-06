using System.Collections.Generic;
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
            string priceText = product.metadata.localizedPriceString;
            string currencyCode = product.metadata.isoCurrencyCode;
            m_PriceText.text = $"{currencyCode}, {priceText},-";
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
