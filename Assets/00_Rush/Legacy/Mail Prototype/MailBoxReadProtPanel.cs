using MoreMountains.Tools;
using Rush;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight.Prototype
{
    public class MailBoxReadProtPanel : PanelView
    {
        [SerializeField]
        private AssetReferenceGameObject m_MailItemViewAsset;
        [SerializeField, MMReadOnly]
        private MailDefinition m_Definition;
        [SerializeField]
        private TextMeshProUGUI m_LabelText;
        [SerializeField]
        private TextMeshProUGUI m_RewardStateText;
        [SerializeField]
        private TextMeshProUGUI m_DescriptionText;
        [SerializeField]
        private Transform m_RewardsViewContainer;
        [SerializeField]
        private Button m_ClaimButton;
        [SerializeField]
        private Button m_DeleteButton;

        private readonly List<MailItemView> m_MailItems = new();
        private HomePanel m_HomePanel;
        private CommonUIView m_NotifView;

        private HomePanel GetHomePanel()
        {
            if (m_HomePanel == null)
            {
                m_HomePanel = CanvasManager.Instance.GetPanel<HomePanel>();
            }
            return m_HomePanel;
        }
        private CommonUIView GetNotifView()
        {
            if (m_NotifView == null)
            {
                m_NotifView = GetHomePanel().GetBinding<CommonUIView>();
            }
            return m_NotifView;
        }
        private void Start()
        {
            m_ClaimButton.onClick.RemoveAllListeners();
            m_ClaimButton.onClick.AddListener(ClaimRewards);

            m_DeleteButton.onClick.RemoveAllListeners();
            m_DeleteButton.onClick.AddListener(DeleteMail);
        }
        public void ReadMail(MailDefinition defi)
        {
            ShowInternal();
            m_Definition = defi;
            InitInternal();
            HideAllItemRewards();
            StartCoroutine(SpawningItemViews());
        }

        private void HideAllItemRewards()
        {
            foreach(MailItemView item in m_MailItems)
            {
                item.Hide();
            }
        }
        public void Refresh()
        {
            InitInternal();
        }    
        private void InitInternal()
        {
            m_LabelText.text = m_Definition.Label;
            m_DescriptionText.text = m_Definition.Description;
            m_RewardStateText.text = m_Definition.StateClaimRewardText();

            bool hasRewards = m_Definition.HasRewards();
            bool hasClaim = m_Definition.HasClaim();
            m_RewardsViewContainer.gameObject.SetActive(hasRewards);
            m_ClaimButton.interactable = !hasClaim;
        }
        private void ClaimRewards()
        {
            m_Definition.ClaimReward();
        }
        private void DeleteMail()
        {
            m_Definition.DeleteMail();
            HideInternal();
        }
        private MailItemView GetItemView(LootField loot)
        {
            foreach (var item in m_MailItems)
            {
                if (item.LootField == loot)
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

        private IEnumerator SpawningItemViews()
        {
            for (int i = 0; i < m_Definition.Rewards.Length; i++)
            {
                LootField loot = m_Definition.Rewards[i];
                if (!HasItemView(loot, out MailItemView view))
                {
                    yield return StartCoroutine(SpawningItemView(loot));
                }
                else
                {
                    view.Init(loot);
                    view.Show();
                }
                yield return new WaitForEndOfFrame();
            }
        }
        private IEnumerator SpawningItemView(LootField loot)
        {
            AsyncOperationHandle<GameObject> handle = m_MailItemViewAsset.InstantiateAsync(m_RewardsViewContainer, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out MailItemView view))
                {
                    view.Init(loot);
                    view.Show();
                    m_MailItems.Add(view);
                }
            }
        }

        protected override void OnHideInvoke()
        {
            base.OnHideInvoke();
            GetNotifView().Hide();
        }
    }
}
