using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class CreditView : UIView
    {
        private CreditField m_Credit;
        [SerializeField]
        private AssetReferenceGameObject m_TextViewAsset;
        [SerializeField]
        private Transform m_ViewContainer;
        [SerializeField]
        private TextMeshProUGUI m_JobDeskName;

        [SerializeField, MMReadOnly]
        private List<TextView> m_StaffNameViews = new();
        public CreditField Credit => m_Credit;
        public void Init(CreditField defi, bool openOnInited)
        {
            m_Credit = defi;

            m_JobDeskName.text = defi.JobDesk;

            string[] staffNames = defi.StaffNames;
            StartCoroutine(SpawingStaffNames(staffNames));

            if (openOnInited)
            {
                ShowInternal();
            }
            else
            {
                HideInternal();
            }
        }

        private TextView GetTextView(string text)
        {
            TextView view = m_StaffNameViews.Find(x => x.Text.text == text);
            if (view == null)
            {
                return null;
            }
            return view;
        }

        private bool HasTextView(string text, out TextView view)
        {
            view = GetTextView(text);
            return view != null;
        }

        private IEnumerator SpawingStaffNames(string[] staffNames)
        {
            for (int i = 0; i < staffNames.Length; i++)
            {
                if (HasTextView(staffNames[i], out TextView view))
                {
                    view.SetText(staffNames[i]);
                }
                else
                {
                    yield return StartCoroutine(SpawningStaffName(staffNames[i]));
                }   
            }
        }

        private IEnumerator SpawningStaffName(string staffName)
        {
            AsyncOperationHandle<GameObject> handle = m_TextViewAsset.InstantiateAsync(m_ViewContainer, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out TextView view))
                {
                    view.SetText(staffName);
                    view.Show();
                    m_StaffNameViews.Add(view);
                }
            }
        }
    }
}
