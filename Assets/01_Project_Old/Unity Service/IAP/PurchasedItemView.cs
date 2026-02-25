using Rush;
using System.Collections.Generic;
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
        protected override void InitInternal(CollectibleConfig collectibleConfig)
        {
            base.InitInternal(collectibleConfig);
            if (collectibleConfig is ProductDefinition product)
            {
                bool hasBonus = UnityService.Instance.IsBonusAvailable(product.BaseInfo.Id);
                List<ProductItemConfig> allProducts = product.GetAllProduct(hasBonus);
                foreach (var item in allProducts)
                {
                    SpawnItemView(item);
                }
            }
        }
        private void SpawnItemView(ProductItemConfig item)
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
