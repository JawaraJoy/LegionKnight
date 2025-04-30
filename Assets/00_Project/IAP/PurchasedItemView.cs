using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class PurchasedItemView : ItemView
    {
        [SerializeField]
        private AssetReferenceGameObject m_ItemViewAsset;
        [SerializeField]
        private Transform m_ItemSpawn;

        private readonly List<ItemView> m_SpawnedItemView = new();
        protected override void InitInternal(object defi)
        {
            base.InitInternal(defi);
            if (defi is ProductDefinition product)
            {
                bool hasBonus = UnityService.Instance.IsBonusAvailable(product.Id);
                List<ProductItem> allProducts = product.GetAllProduct(hasBonus);
                foreach (var item in allProducts)
                {
                    SpawnItemView(item);
                }
            }
        }
        private void SpawnItemView(ProductItem item)
        {
            ClearSpawnedItemView();
            Addressables.InstantiateAsync(m_ItemViewAsset, m_ItemSpawn).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    ItemView itemView = handle.Result.GetComponent<ItemView>();
                    itemView.Init(item);
                    m_SpawnedItemView.Add(itemView);
                }
            };
        }

        private void ClearSpawnedItemView()
        {
            foreach (var itemView in m_SpawnedItemView)
            {
                Destroy(itemView.gameObject);
            }
            m_SpawnedItemView.Clear();
        }
    }
}
