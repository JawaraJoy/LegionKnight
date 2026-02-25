using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public virtual void Init(TaskDefinition defi)
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

            LootChestDefinition loot = defi.Rewards;
            if (loot != null && m_LootMonitor != null)
            {
                m_LootMonitor.AddLootsView(loot.LootFields.ToList());
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
