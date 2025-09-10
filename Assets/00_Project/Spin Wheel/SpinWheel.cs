using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class SpinWheel : MonoBehaviour
    {
        [SerializeField]
        private SpinWheelDefinition m_Definition;
        

        public SpinWheelDefinition Definition => m_Definition;

        [SerializeField, MMReadOnly]
        private int m_StepOnIndex = 0;
        [SerializeField, MMReadOnly]
        private int m_MaxStepOnIndex = 0;
        [SerializeField, MMReadOnly]
        private float m_DelayStep;
        [SerializeField, MMReadOnly]
        private SpinRewardDefinition m_SelectedReward;

        private void Start()
        {
            m_MaxStepOnIndex = m_Definition.Rewards.Length - 1;
        }

        [ContextMenu(nameof(TrySpin))]
        private void TrySpin()
        {
            StartCoroutine(StartSpin());
        }
        private IEnumerator StartSpin()
        {
            
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
        }

        private void AddStepIndexInternal(int step)
        {
            m_StepOnIndex += step;
            
            if (m_StepOnIndex > m_MaxStepOnIndex)
            {
                m_StepOnIndex = 0;
            }
            m_SelectedReward = m_Definition.Rewards[m_StepOnIndex];
        }
    }
}
