using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class CreditPanel : PanelView
    {
        [SerializeField]
        private AssetReferenceGameObject m_CreditViewAsset;
        [SerializeField]
        private Transform m_ContianerView;

        [SerializeField, MMReadOnly]
        private List<CreditView> m_CreditViews = new();
        private CreditManager m_CreditManager;
        private CreditManager GetManager()
        {
            if (m_CreditManager == null)
            {
                m_CreditManager = GameManager.Instance.CreditManager;
            }
            return m_CreditManager;
        }
        private CreditDefinition m_Definition;
        private CreditDefinition GetDefinition()
        {
            if (m_Definition == null)
            {
                m_Definition = GetManager().Definition;
            }
            return m_Definition;
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            CreditField[] credits = GetManager().Definition.Credits;
            StartCoroutine(SpawingCreditViews(credits));
        }
        private CreditView GetTextView(string jobdesk)
        {
            CreditView view = m_CreditViews.Find(x => x.Credit.JobDesk == jobdesk);
            if (view == null)
            {
                return null;
            }
            return view;
        }

        private bool HasTextView(string jobdesk, out CreditView view)
        {
            view = GetTextView(jobdesk);
            return view != null;
        }

        private IEnumerator SpawingCreditViews(CreditField[] credit)
        {
            for (int i = 0; i < credit.Length; i++)
            {
                if (HasTextView(credit[i].JobDesk, out CreditView view))
                {
                    view.Init(credit[i], true);
                }
                else
                {
                    yield return StartCoroutine(SpawningCreditView(credit[i]));
                }
            }
        }

        private IEnumerator SpawningCreditView(CreditField credit)
        {
            AsyncOperationHandle<GameObject> handle = m_CreditViewAsset.InstantiateAsync(m_ContianerView, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out CreditView view))
                {
                    view.Init(credit, true);
                    m_CreditViews.Add(view);
                }
            }
        }
    }
}
