using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class StageHandler : MonoBehaviour
    {
        [SerializeField]
        private StageConfig m_UsedStageConfig;
        [SerializeField]
        private StageConfig m_SelectedStageConfig;
        [SerializeField, MMReadOnly]
        private VerticalLoopView m_VerticalLoopView;
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

        public UnityEvent<StageConfig> OnStageStart => m_OnStageStart;
        public UnityEvent<StageConfig> OnStageOver => m_OnStageOver;
        public UnityEvent<StageConfig> OnStageCompleted => m_OnStageCompleted;
        public EnemyWaveHandler EnemyWaveHandler => m_EnemyWaveHandler;
        public PlatformHandler PlatformHandler => m_PlatformHandler;

        public StageConfig UsedStageConfig => m_UsedStageConfig;
        public StageConfig SelectedStageConfig => m_SelectedStageConfig;
        public StageSelectionField[] StageSelections => m_StageSelections;

        [SerializeField, MMReadOnly]
        private int m_CurrentWaveIndex = 0;
        public int CurrentWaveIndex => m_CurrentWaveIndex;

        public bool IsLastWaveIndex
        {
            get
            {
                return m_CurrentWaveIndex >= m_UsedStageConfig.EnemyWaveConfigs.Length - 1;
            }
        }
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
        public void Init(VerticalLoopView loopView)
        {
            m_VerticalLoopView = loopView;
            foreach (var stage in m_StageSelections)
            {
                stage.Init();
            }
        }
        public void SelectStage(StageConfig stage)
        {
            SelectStageInternal(stage);
        }
        private void SelectStageInternal(StageConfig stage)
        {
            if (HasStageSelection(stage, out StageSelectionField stageSelection))
            {
                if (stageSelection.StageState == StageState.Locked) return;
                m_SelectedStageConfig = stage;
                m_PlatformHandler.Prepare(m_UsedStageConfig.PlatformHandlerConfig);
            }
        }
        private void StartCurrentWaveSet()
        {
            if (m_UsedStageConfig == null) return;

            var waves = m_UsedStageConfig.EnemyWaveConfigs;

            if (waves == null || waves.Length == 0)
            {
                Debug.LogWarning("Stage has no waves.");
                return;
            }

            if (m_CurrentWaveIndex < 0)
                m_CurrentWaveIndex = 0;

            if (m_CurrentWaveIndex >= waves.Length)
                m_CurrentWaveIndex = waves.Length - 1;

            m_EnemyWaveHandler.SetNewWave(waves[m_CurrentWaveIndex]);
        }

        private bool IsLoopMode
        {
            get
            {
                return m_UsedStageConfig != null &&
                       (m_UsedStageConfig.StageMode == StageMode.Classic ||
                        m_UsedStageConfig.StageMode == StageMode.Collosal);
            }
        }

        private void HandleWaveSetCleared(EnemyWaveConfig waveConfig)
        {
            m_CurrentWaveIndex++;

            int len = m_UsedStageConfig.EnemyWaveConfigs.Length;

            if (m_CurrentWaveIndex >= len)
            {
                if (IsLoopMode)
                {
                    m_CurrentWaveIndex = 0;
                    StartCurrentWaveSet();
                }
                else
                {
                    m_OnStageCompleted?.Invoke(m_UsedStageConfig);
                }
                return;
            }

            StartCurrentWaveSet();
        }

        public void SetBackground(VerticalBackgroundConfig backgroundConfig)
        {
            m_VerticalLoopView.Init(backgroundConfig);
        }

        public void PlayStage()
        {
            m_UsedStageConfig = m_SelectedStageConfig;

            m_VerticalLoopView.Init(m_UsedStageConfig.VerticalBackgroundConfig);

            // ✅ reset wave index dan start wave set pertama
            m_CurrentWaveIndex = 0;
            if (m_PlatformHandler != null)
            {
                m_PlatformHandler.Prepare(m_UsedStageConfig.PlatformHandlerConfig);
                m_PlatformHandler.Play();
            }
            if (m_EnemyWaveHandler != null)
            {
                m_EnemyWaveHandler.OnWaveSetCleared.RemoveListener(HandleWaveSetCleared);
                m_EnemyWaveHandler.OnWaveSetCleared.AddListener(HandleWaveSetCleared);

                StartCurrentWaveSet();
            }
            // ✅ pastikan tidak double-subscribe
            m_OnStageStart?.Invoke(m_UsedStageConfig);
        }
        public void Resume()
        {
            //m_EnemyWaveHandler?.Resume();
            m_PlatformHandler.Resume();
        }
        public void Pause()
        {
            //m_EnemyWaveHandler?.Pause();
            m_PlatformHandler.Pause();
        }

        private void OnStageOverInvokeInternal()
        {
            m_OnStageOver?.Invoke(m_UsedStageConfig);
        }
        private void OnStageCompletedInvokeInternal()
        {
            m_OnStageCompleted?.Invoke(m_UsedStageConfig);
        }
    }
}
