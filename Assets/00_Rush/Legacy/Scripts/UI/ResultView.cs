using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
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

        [SerializeField]
        private UnityEvent m_OnShowRewardDone = new();
        public virtual void ShowResults(List<CollectibleConfig> results)
        {
            ShowResultsInternal(results);
        }
        protected virtual void ShowResultsInternal(List<CollectibleConfig> results)
        {
            foreach (ItemView item in m_SpawnedItemViews)
            {
                Destroy(item.gameObject);
            }
            m_SpawnedItemViews.Clear();
            StartCoroutine(ShowingResult(results));
        }
        protected virtual IEnumerator ShowingResult(List<CollectibleConfig> results)
        {
            ShowInternal();
            for (int i = 0; i < results.Count; i++)
            {
                yield return StartCoroutine(SpawningItemView(results[i]));
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
            OnShowRewardDoneInvoke();
        }
        protected virtual IEnumerator SpawningItemView(CollectibleConfig config)
        {
            AsyncOperationHandle<GameObject> handle = m_ItemViewAsset.InstantiateAsync(m_SpawnPost, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out ItemView view))
                {
                    view.Init(config);
                    view.Show();
                    m_SpawnedItemViews.Add(view);
                }
            }
        }

        private void OnShowRewardDoneInvoke()
        {
            m_OnShowRewardDone?.Invoke();
        }
    }
}
