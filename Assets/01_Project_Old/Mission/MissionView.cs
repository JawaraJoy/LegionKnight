using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight
{
    public abstract class MissionView : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceGameObject m_LootItemViewAsset;
        [SerializeField]
        private TextMeshProUGUI m_DescriptionText;
        [SerializeField]
        private TextMeshProUGUI m_ProgressText;
        [SerializeField]
        private TextMeshProUGUI m_PowerText;
        [SerializeField]
        private Image m_IconImage;
        [SerializeField]
        private Slider m_ProgressSlider;
        
        [SerializeField]
        private Transform m_LootViewParent;

        [SerializeField]
        private GameObject m_OnProgressContent;
        [SerializeField]
        private GameObject m_OnCompletedContent;
        [SerializeField]
        private GameObject m_OnClaimedContent;
        [SerializeField]
        private LootMonitor m_LootMonitor;
        [SerializeField]
        private Button m_ClaimButton;
        private TaskDefinition m_Definition;
        public TaskDefinition Definition => m_Definition;
        protected abstract MissionController GetControllerInternal();

        private readonly List<LootItemView> m_RewardViews = new();
        private LootItemView GetLootItemView(LootField loot)
        {
            foreach (var view in m_RewardViews)
            {
                if (view.Definition == loot)
                {
                    return view;
                }
            }
            return null;
        }
        public void Init(TaskDefinition defi)
        {
            m_Definition = defi;
            TaskStatus status = GetControllerInternal().GetTaskStatus(defi);
            string desc = defi.Description;
            if (m_IconImage != null)
            {
                if (defi.Icon != null)
                {
                    m_IconImage.sprite = defi.Icon;
                }
            }
            Debug.Log("xx-" + status);
            string progress = $"{status.CurrentScore}/{defi.TargetScore}";
            float progressValue = (float)status.CurrentScore / defi.TargetScore;
            int difficulty = defi.TaskPower;
            TaskState state = status.CurrentState;

            LootDefinition loot = defi.Rewards;
            if (loot != null)
            {
                StartCoroutine(SpawnLootViews(loot));
            }

            m_DescriptionText.text = desc;
            m_ProgressText.text = progress;
            m_PowerText.text = difficulty.ToString();
            m_ProgressSlider.value = progressValue;
            UpdateState(state);

            UnityAction directClaim = new (DirectClaim);
            m_ClaimButton.onClick.RemoveAllListeners();
            m_ClaimButton.onClick.AddListener(directClaim);
        }
        private IEnumerator SpawnLootViews(LootDefinition loot)
        {
            LootField[] currentRewards = loot.LootFields;
            if (currentRewards.Length == 0) yield break;
            for (int i = 0; i < currentRewards.Length; i++)
            {
                if (GetLootItemView(currentRewards[i]) != null)
                {
                    Coroutine spawn = StartCoroutine(SpawningLootItemView(currentRewards[i]));
                    yield return spawn;
                }
            }
        }
        private IEnumerator SpawningLootItemView(LootField lootField)
        {
            var handle = m_LootItemViewAsset.InstantiateAsync(m_LootViewParent);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out LootItemView view) )
                {
                    view.Init(lootField);
                    m_RewardViews.Add(view);
                }
            }
            else
            {
                Debug.LogError($"Failed to load LootItemView from {m_LootItemViewAsset.RuntimeKey}");
            }
        }

        private void UpdateState(TaskState state)
        {
            m_OnProgressContent.SetActive(state == TaskState.OnProgress);
            m_OnCompletedContent.SetActive(state == TaskState.Completed);
            m_OnClaimedContent.SetActive(state == TaskState.Claimed);
        }
        private void DirectClaim()
        {
            if (m_Definition == null) return;
            m_Definition.DirectDailyClaimRewards();
            TaskState state = GetControllerInternal().GetTaskStatus(m_Definition).CurrentState;
            UpdateState(state);

            //--Tenjin Record
            TenjinManager.Instance.SendEventToMissionComplete(m_Definition);
        }
    }
}
