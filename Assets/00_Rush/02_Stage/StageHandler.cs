using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class StageHandler : MonoBehaviour, IReseter
    {
        [SerializeField]
        private StageConfig m_UsedStageConfig;
        [SerializeField]
        private StageConfig m_SelectedStageConfig;
        [SerializeField]
        private GameStateConfig m_GameStateConfig;
        [SerializeField, MMReadOnly]
        private VerticalLoopView m_VerticalLoopView;
        [SerializeField]
        private StageSelectionField[] m_StageSelections;

        [SerializeField]
        private EnemyWaveHandler m_EnemyWaveHandler;
        [SerializeField]
        private PlatformHandler m_PlatformHandler;
        [SerializeField]
        private UnityEvent<StageConfig> m_OnSelectStage;
        [SerializeField]
        private UnityEvent<StageConfig> m_OnStageStart;
        [SerializeField]
        private UnityEvent<StageConfig> m_OnStageOver;
        [SerializeField]
        private UnityEvent<StageConfig> m_OnStageCompleted;
        public UnityEvent<StageConfig> OnSelectStage => m_OnSelectStage;
        public UnityEvent<StageConfig> OnStageStart => m_OnStageStart;
        public UnityEvent<StageConfig> OnStageOver => m_OnStageOver;
        public UnityEvent<StageConfig> OnStageCompleted => m_OnStageCompleted;
        public EnemyWaveHandler EnemyWaveHandler => m_EnemyWaveHandler;
        public PlatformHandler PlatformHandler => m_PlatformHandler;
        public GameStateConfig GameStateConfig => m_GameStateConfig;
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

        // ── StageSelectionField helpers ───────────────────────────────────────
private StageSelectionField FindStageSelection(StageConfig stageConfig)
        {
            if (stageConfig == null || m_StageSelections == null)
            {
                return null;
            }

            foreach (StageSelectionField stageSelection in m_StageSelections)
            {
                if (stageSelection == null || stageSelection.StageConfig == null)
                {
                    continue;
                }

                if (stageSelection.StageConfig.BaseInfo.Id == stageConfig.BaseInfo.Id)
                {
                    return stageSelection;
                }
            }

            return null;
        }

        private bool HasStageSelectionInternal(StageConfig stageConfig, out StageSelectionField stageSelection)
        {
            stageSelection = FindStageSelection(stageConfig);
            return stageSelection != null;
        }
        public bool HasStageSelection(StageConfig stageConfig, out StageSelectionField stageSelection)
        {
            return HasStageSelectionInternal(stageConfig, out stageSelection);
        }
        // ── Init ──────────────────────────────────────────────────────────────
        public void Init(VerticalLoopView loopView)
        {
            m_VerticalLoopView = loopView;
            foreach (var stage in m_StageSelections)
            {
                stage.Init();
            }
        }

        // ── Select ────────────────────────────────────────────────────────────
        public void SelectStage(StageConfig stage)
        {
            SelectStageInternal(stage);
        }

        private void SelectStageInternal(StageConfig stage)
        {
            if (HasStageSelectionInternal(stage, out StageSelectionField stageSelection))
            {
                if (stageSelection.StageState == StageState.Locked) return;
                m_SelectedStageConfig = stage;
                OnSelectStageInvoke(m_SelectedStageConfig);
                m_PlatformHandler.Prepare(m_SelectedStageConfig.PlatformHandlerConfig);
            }
        }

        // ── Wave progression ──────────────────────────────────────────────────
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

        /// <summary>
        /// Classic dan Collosal = loop terus.
        /// Adventure (dan mode lain) = selesai setelah semua wave clear.
        /// </summary>
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
            var waves = m_UsedStageConfig?.EnemyWaveConfigs;
            if (waves == null || m_CurrentWaveIndex >= waves.Length || waves[m_CurrentWaveIndex] != waveConfig) return;

            m_CurrentWaveIndex++;
            int len = waves.Length;

            if (m_CurrentWaveIndex >= len)
            {
                if (IsLoopMode)
                {
                    // Loop kembali ke wave pertama
                    m_CurrentWaveIndex = 0;
                    StartCurrentWaveSet();
                }
                else
                {
                    // Semua wave sudah clear dan bukan loop mode (Adventure, dll)
                    // → set StageState ke Completed dan simpan ke save data
                    m_PlatformHandler?.Pause();
                    SetStageCompleted(m_UsedStageConfig);

                    OnStageCompletedInvoke(m_UsedStageConfig);
                }
                return;
            }

            StartCurrentWaveSet();
        }

        /// <summary>
        /// Cari StageSelectionField yang cocok lalu set state-nya ke Completed.
        /// StageSelectionField.SetStageState() sudah handle save data via UnityService.
        /// </summary>
        private void SetStageCompleted(StageConfig stageConfig)
        {
            if (!HasStageSelectionInternal(stageConfig, out StageSelectionField field))
            {
                Debug.LogWarning($"[StageHandler] StageSelectionField tidak ditemukan untuk: {stageConfig.BaseInfo.Id}");
                return;
            }

            if (field.StageState == StageState.Completed) return; // Sudah completed, skip

            field.SetStageState(StageState.Completed);
            Debug.Log($"[StageHandler] Stage '{stageConfig.BaseInfo.Id}' marked as Completed.");
        }

        // ── Background ────────────────────────────────────────────────────────
        public void SetBackground(VerticalBackgroundConfig backgroundConfig)
        {
            m_VerticalLoopView.Init(backgroundConfig);
        }

        // ── Play ──────────────────────────────────────────────────────────────
        public void PlayStage()
        {
            Player.Instance.PlayerCardDeck.PrepareUsedCards();
            m_UsedStageConfig = m_SelectedStageConfig;

            m_VerticalLoopView.Init(m_UsedStageConfig.VerticalBackgroundConfig);

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
            Player.Instance.PlayerCardDeck.UseCardConfig();
            OnStageStartInvoke(m_UsedStageConfig);
        }

        // ── Pause / Resume ────────────────────────────────────────────────────
        public void Resume()
        {
            if (m_UsedStageConfig != null && m_CurrentWaveIndex >= m_UsedStageConfig.EnemyWaveConfigs.Length) return;
            m_PlatformHandler.Resume();
        }

        public void Pause()
        {
            m_PlatformHandler.Pause();
        }

        // ── Reset ─────────────────────────────────────────────────────────────
        public void ResetProgression()
        {
            m_PlatformHandler.ResetProgression();
            m_EnemyWaveHandler.ResetProgression();
        }
    

public void OnStageCompletedInvoke(StageConfig stageConfig)
        {
            m_OnStageCompleted?.Invoke(stageConfig);
        }


public void OnStageStartInvoke(StageConfig stageConfig)
        {
            m_OnStageStart?.Invoke(stageConfig);
        }


public void OnSelectStageInvoke(StageConfig stageConfig)
        {
            m_OnSelectStage?.Invoke(stageConfig);
        }


private void OnDisable()
        {
            if (m_EnemyWaveHandler != null)
            {
                m_EnemyWaveHandler.OnWaveSetCleared.RemoveListener(HandleWaveSetCleared);
            }
        }
}
}
