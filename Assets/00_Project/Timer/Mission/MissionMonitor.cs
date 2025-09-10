using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace LegionKnight
{
    public abstract class MissionMonitor : UIView
    {
        [SerializeField]
        private AssetReferenceGameObject m_MissionViewAsset;
        [SerializeField]
        private TextMeshProUGUI m_TitleText;
        [SerializeField]
        private TextMeshProUGUI m_OveralProgressAmountText;
        [SerializeField]
        private Slider m_OverallProgressSlider;
        [SerializeField]
        private Transform m_MissionViewParent;
        [SerializeField]
        private TextMeshProUGUI m_ResetTimerText;
        [SerializeField]
        private TaskThresholdView[] m_TaskThresholdViews;

        protected MissionController m_Controller;


        protected abstract MissionController GetControllerInternal();

        private readonly List<MissionView> m_MissionViews = new();

        private MissionView GetMissionView(TaskDefinition defi)
        {
            foreach (var view in m_MissionViews)
            {
                if (view.Definition == defi)
                {
                    return view;
                }
            }
            return null;
        }
        public void SetTaskProgressSlide(float val)
        {
            SetTaskProgressSlideInternal(val);
        }
        protected void SetTaskProgressSlideInternal(float val)
        {
            m_OverallProgressSlider.value = val;
            int power = GetControllerInternal().CurrentTaskPower;
            m_OveralProgressAmountText.text = power.ToString();
            InitThresholdViews(GetControllerInternal());
        }
        private bool HasMissionView(TaskDefinition defi)
        {
            return GetMissionView(defi) != null;
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            InitInternal(GetControllerInternal());
        }
        public void Init(MissionController controller)
        {
            InitInternal(controller);
        }

        private void InitInternal(MissionController controller)
        {
            m_TitleText.text = controller.BehaviourName;
            foreach (var task in controller.Task)
            {
                if (!HasMissionView(task.Definition))
                {
                    StartCoroutine(SpawningMissionView(task.Definition));
                }
                else
                {
                    GetMissionView(task.Definition).Init(task.Definition);
                }
            }
            //InitThresholdViews(controller);
            float powerRate = (float)controller.CurrentTaskPower / (float)controller.MaxTaskPower;
            SetTaskProgressSlideInternal(powerRate);

            TimerDefinition defi = controller.ResetTime;
            m_ResetTimerText.text = defi.GetRemainingTimeToReset();
        }

        private void InitThresholdViews(MissionController controller)
        {
            for (int i = 0; i < m_TaskThresholdViews.Length; i++)
            {
                TaskThreshold threshold = controller.TaskThresholds[i];
                TaskThresholdView thresholdView = m_TaskThresholdViews[i];
                thresholdView.Init(threshold);
            }
        }
        private IEnumerator SpawningMissionView(TaskDefinition defi)
        {
            AsyncOperationHandle<GameObject> handle = m_MissionViewAsset.InstantiateAsync(m_MissionViewParent);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject obj = handle.Result;
                if (obj.TryGetComponent(out MissionView view))
                {
                    view.Init(defi);
                    m_MissionViews.Add(view);
                }
            }
        }
    }
}
