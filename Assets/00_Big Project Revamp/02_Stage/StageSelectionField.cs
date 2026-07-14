using LegionKnight;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class StageSelectionField
    {
        [SerializeField]
        private StageConfig m_StageConfig;
        [SerializeField]
        private StageState m_StageState = StageState.Locked;

        private string StateKey => "stageState" + m_StageConfig.BaseInfo.Id;
        public StageConfig StageConfig => m_StageConfig;
        public StageState StageState => m_StageState;
        public StageSelectionField(StageConfig stageConfig, StageState stageState)
        {
            m_StageConfig = stageConfig;
            m_StageState = stageState;
        }
        public void Init()
        {
            SetStageStateInternal(m_StageConfig.StartingStageState);
        }

        private void SetStageStateInternal(StageState state)
        {
            m_StageState = state;
        }
        public void SetStageState(StageState state)
        {
            SetStageStateInternal(state);
        }
    }
}
