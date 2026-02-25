using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class StageHandler : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private StageConfig m_UsedStageConfig;
        [SerializeField, MMReadOnly]
        private StageConfig m_SelectedStageConfig;
        [SerializeField]
        private StageSelectionField[] m_StageSelections;

        [SerializeField]
        private EnemyWaveHandler m_EnemyWaveHandler;
        [SerializeField]
        private PlatformHandler m_PlatformHandler;
        [SerializeField]
        private UnityEvent<StageConfig> m_OnStageStart;
        [SerializeField]
        private UnityEvent<StageConfig> m_OnStageOver;
        [SerializeField]
        private UnityEvent<StageConfig> m_OnStageCompleted;

        public EnemyWaveHandler EnemyWaveHandler => m_EnemyWaveHandler;
        public PlatformHandler PlatformHandler => m_PlatformHandler;
        
        public StageConfig UsedStageConfig => m_UsedStageConfig;
        public StageConfig SelectedStageConfig => m_SelectedStageConfig;
        public StageSelectionField[] StageSelections => m_StageSelections;

        private StageSelectionField GetStageSelection(StageConfig stageConfig)
        {
            foreach (var stage in m_StageSelections)
            {
                if (stage.StageConfig.BaseInfo.Id == stageConfig.BaseInfo.Id)
                {
                    return stage;
                }
            }
            return null;
        }
        private bool HasStageSelection(StageConfig stageConfig, out StageSelectionField stageSelection)
        {
            stageSelection = GetStageSelection(stageConfig);
            return stageSelection != null;
        }
        public void Init()
        {
            foreach (var stage in m_StageSelections)
            {
                stage.Init();
            }
        }
        private void SelectStageInternal(StageConfig stage)
        {
            if (HasStageSelection(stage, out StageSelectionField stageSelection))
            {
                if (stageSelection.StageState == StageState.Locked) return;
                m_SelectedStageConfig = stage;
            }
        }
    }
}
