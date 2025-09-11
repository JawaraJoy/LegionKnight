using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class SpinWheel : MonoBehaviour
    {
        [SerializeField]
        private SpinWheelDefinition m_Definition;
        public SpinWheelDefinition Definition => m_Definition;

        [SerializeField, MMReadOnly]
        private int m_FreeDraw;
        [SerializeField, MMReadOnly]
        private int m_FreeDrawWatch;
        [SerializeField, MMReadOnly]
        private int m_StepOnIndex = 0;
        [SerializeField, MMReadOnly]
        private int m_MaxStepOnIndex = 0;
        [SerializeField, MMReadOnly]
        private float m_DelayStep;
        [SerializeField, MMReadOnly]
        private SpinRewardDefinition m_SelectedReward;

        [SerializeField]
        private UnityEvent m_OnInitialized;
        [SerializeField]
        private UnityEvent m_OnStepStart;
        [SerializeField]
        private UnityEvent<SpinRewardDefinition> m_OnStepChanged;
        [SerializeField]
        private UnityEvent m_OnStepDone;
        [SerializeField]
        private UnityEvent<SpinRewardDefinition> m_OnClaim;

        public SpinRewardDefinition SelectedReward => m_SelectedReward;
        public int FreeDraw => m_FreeDraw;
        public int FreeDrawWatch => m_FreeDrawWatch;
        [SerializeField, MMReadOnly]
        private bool m_IsBusy = false;

        private string FreeDrawKey = $"spinfree";
        private string FreeDrawWatchKey = $"spinwatchfree";
        private void Start()
        {
            m_MaxStepOnIndex = m_Definition.Rewards.Length - 1;
        }

        public void Init()
        {
            bool hasFreeDraw = UnityService.Instance.HasData(FreeDrawKey);
            bool hasFreeWatchDraw = UnityService.Instance.HasData(FreeDrawWatchKey);
            if (hasFreeDraw)
            {
                m_FreeDraw = UnityService.Instance.GetData<int>(FreeDrawKey);
            }
            else
            {
                m_FreeDraw = m_Definition.FreeDrawAmount;
                m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
            }
            if (hasFreeWatchDraw)
            {
                m_FreeDrawWatch = UnityService.Instance.GetData<int>(FreeDrawWatchKey);
            }
            else
            {
                m_FreeDraw = m_Definition.FreeDrawAmount;
                m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
            }

            TimerDefinition tim = m_Definition.FreeDrawResetTime;
            if (UnityService.Instance.HasData(tim.TimerId))
            {
                if (tim.IsTimeToReset())
                {
                    m_FreeDraw = m_Definition.FreeDrawAmount;
                    m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
                }
            }
            else
            {
                tim.StartTimer();
            }
            m_OnInitialized?.Invoke();
        }

        public bool CanClaim()
        {
            return m_SelectedReward != null && !m_IsBusy;
        }

        [ContextMenu(nameof(TrySpin))]
        private void TrySpin()
        {
            StartCoroutine(StartSpin());
        }

        private bool CanFreeDraw()
        {
            return m_FreeDraw > 0;
        }

        private bool CanFreeWatchDraw()
        {
            return m_FreeDrawWatch > 0;
        }

        public void Spin()
        {
            StartCoroutine(StartSpin());
        }
        public void FreeSpin(UnityAction onDraw)
        {
            if (CanFreeDraw())
            {
                StartCoroutine(StartSpin());
                AddFreeDraw(-1);
                onDraw?.Invoke();
            }
        }
        public void FreeWatchSpin(UnityAction onDraw)
        {
            if (CanFreeWatchDraw())
            {
                UnityService.Instance.ShowRewardedAd(() => AfterWatch(onDraw));
            }
        }
        private void AfterWatch(UnityAction onDraw)
        {
            StartCoroutine(StartSpin());
            AddFreeDrawWatch(-1);
            onDraw?.Invoke();
        }
        private IEnumerator StartSpin()
        {
            m_OnStepStart?.Invoke();
            m_IsBusy = true;
            int minStep = m_Definition.MiniSpinStep;
            int minAdditionalStep = m_Definition.MinAdditionalSpinStep;
            int maxAdditionalStep = m_Definition.MaxAdditionalSpinStep;
            int randomAdditionalStep = Random.Range(minAdditionalStep, maxAdditionalStep);
            int finalStep = minStep + randomAdditionalStep;

            float delayStep = m_Definition.StartStepDelay;
            float midStepGrowth = m_Definition.MidDelayGrowthStep;
            float endStepGrowth = m_Definition.EndDelayGrowthStep;
            for (int i = 0; i < finalStep; i++)
            {
                AddStepIndexInternal(1);
                float stepRate = (float)i / (float)finalStep;
                m_DelayStep = delayStep;
                if (stepRate > 0.8)
                {
                    m_DelayStep = delayStep + midStepGrowth;
                }
                if (stepRate > 0.9f)
                {
                    m_DelayStep = delayStep + endStepGrowth;
                }
                yield return new WaitForSeconds(m_DelayStep);
            }
            m_IsBusy = false;
            m_OnStepDone?.Invoke();
            yield return new WaitForSeconds(m_Definition.ClaimDelay);
            ClaimReward();
        }

        private void AddStepIndexInternal(int step)
        {
            m_StepOnIndex += step;
            
            if (m_StepOnIndex > m_MaxStepOnIndex)
            {
                m_StepOnIndex = 0;
            }
            m_SelectedReward = m_Definition.Rewards[m_StepOnIndex];
            m_OnStepChanged?.Invoke(m_SelectedReward);
        }
        private void ClaimReward()
        {
            m_SelectedReward.Rewards.DirectTakeLoots();
            m_OnClaim?.Invoke(m_SelectedReward);
            m_SelectedReward = null;
        }
        [ContextMenu(nameof(TryClaim))]
        private void TryClaim()
        {
            m_SelectedReward = null;
        }

        private void AddFreeDraw(int amount)
        {
            m_FreeDraw += amount;
            if (m_FreeDraw < 0)
            {
                m_FreeDraw = 0;
            }
            if (m_FreeDraw > m_Definition.FreeDrawAmount)
            {
                m_FreeDraw = m_Definition.FreeDrawAmount;
            }
            UnityService.Instance.SaveData(FreeDrawKey, m_FreeDraw);
        }
        private void AddFreeDrawWatch(int amount)
        {
            m_FreeDrawWatch += amount;
            if (m_FreeDrawWatch < 0)
            {
                m_FreeDrawWatch = 0;
            }
            if (m_FreeDrawWatch > m_Definition.FreeDrawWatchAmount)
            {
                m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
            }
            UnityService.Instance.SaveData(FreeDrawWatchKey, m_FreeDrawWatch);
        }
    }
}
