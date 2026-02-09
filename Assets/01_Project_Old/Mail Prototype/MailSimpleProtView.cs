using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight.Prototype
{
    public class MailSimpleProtView : UIView
    {
        private MailDefinition m_Definition;
        [SerializeField]
        private AssetReferenceGameObject m_MailItemViewAsset;
        [SerializeField]
        private TextMeshProUGUI m_LabelText;
        [SerializeField]
        private TextMeshProUGUI m_StatusText;
        [SerializeField]
        private TextMeshProUGUI m_RewardStateText;
        [SerializeField]
        private Transform m_ItemViewContainer;
        private readonly List<MailItemView> m_MailItems = new();
        public MailDefinition Definition => m_Definition;
        /*[SerializeField]
        private Button m_ReadMail;*/

        // on button;
        public void ReadMail()
        {
            m_Definition.ReadMail();
        }
        public void Init(MailDefinition defi)
        {
            if (m_Definition == null)
            {
                m_Definition = defi;
            }
            m_LabelText.text = m_Definition.Label;
            m_StatusText.text = m_Definition.GetMailState().ToString();
            m_RewardStateText.text = m_Definition.StateClaimRewardText();
            

            if (m_Definition.GetMailState() == MailState.Hide || m_Definition.GetMailState() == MailState.Delete)
            {
                HideInternal();
            }
            else
            {
                ShowInternal();
                StartCoroutine(SpawItemViews());
            }
        }

        private MailItemView GetItemView(LootField loot)
        {
            foreach (var item in m_MailItems)
            {
                if (item.Definition == loot)
                {
                    return item;
                }
            }
            return null;
        }

        private bool HasItemView(LootField loot, out MailItemView view)
        {
            view = GetItemView(loot);
            return view != null;
        }

        private IEnumerator SpawItemViews()
        {
            for (int i = 0; i < m_Definition.Rewards.Length; i++)
            {
                LootField loot = m_Definition.Rewards[i];
                if (!HasItemView(loot, out MailItemView view))
                {
                    yield return StartCoroutine(SpawnItemView(loot));
                }
                else
                {
                    view.Init(loot);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        private IEnumerator SpawnItemView(LootField loot)
        {
            AsyncOperationHandle<GameObject> handle = m_MailItemViewAsset.InstantiateAsync(m_ItemViewContainer, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out MailItemView view))
                {
                    view.Init(loot);
                    m_MailItems.Add(view);
                }
            }
        }
    }
}
