using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class ResultView : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_ItemViewAsset;
        [SerializeField]
        private Transform m_SpawnPost;

        [SerializeField]
        private List<ItemView> m_SpawnedItemViews = new();
        public virtual void ShowResults(List<object> results)
        {
            foreach(ItemView item in m_SpawnedItemViews)
            {
                Destroy(item.gameObject);
            }
            m_SpawnedItemViews.Clear();
            StartCoroutine(ShowingResult(results));
        }
        protected virtual IEnumerator ShowingResult(List<object> results)
        {
            ShowInternal();
            for (int i = 0; i < results.Count; i++)
            {
                yield return StartCoroutine(SpawningItemView(results[i]));
                yield return new WaitForSeconds(0.2f);
            }
        }
        protected virtual IEnumerator SpawningItemView(object defi)
        {
            AsyncOperationHandle<GameObject> handle = m_ItemViewAsset.InstantiateAsync(m_SpawnPost, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out ItemView view))
                {
                    view.Init(defi);
                    view.Show();
                    m_SpawnedItemViews.Add(view);
                }
            }
        }
    }
}
