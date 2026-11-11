using LegionKnight;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class StageSelectionField
    {
        private const string c_StateKeyPrefix = "stageState";
        [SerializeField]
        private StageConfig m_StageConfig;
        [SerializeField]
        private StageState m_StageState = StageState.Locked;

        private string StateKey => c_StateKeyPrefix + m_StageConfig.BaseInfo.Id;
        public StageConfig StageConfig => m_StageConfig;
        public StageState StageState => m_StageState;
        public StageSelectionField(StageConfig stageConfig, StageState stageState)
        {
            m_StageConfig = stageConfig;
            m_StageState = stageState;
        }
public void Init()
        {
            if (m_StageConfig == null)
            {
                Debug.LogError("[StageSelectionField] StageConfig is not assigned.");
                return;
            }

            if (UnityService.Instance.HasData(StateKey))
            {
                m_StageState = UnityService.Instance.GetData<StageState>(StateKey);
                return;
            }

            SetStageStateInternal(m_StageConfig.StartingStageState);
        }

private void SetStageStateInternal(StageState state)
        {
            if (m_StageConfig == null)
            {
                Debug.LogError("[StageSelectionField] Cannot save state without a StageConfig.");
                return;
            }

            m_StageState = state;
            UnityService.Instance.SaveData(StateKey, state);
        }
        public void SetStageState(StageState state)
        {
            SetStageStateInternal(state);
        }
    }
}
