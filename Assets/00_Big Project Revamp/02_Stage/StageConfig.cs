using LegionKnight;
using System;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Stage", menuName = "Rush/Level/Stage")]
    public partial class StageConfig : Configuration, IHasSplashImage
    {
        [SerializeField]
        private StageMode m_StageMode = StageMode.Classic;
        [SerializeField]
        private StageState m_StartingStageState = StageState.Locked;
        [SerializeField]
        private int m_EnergyAmountToPay;
        [SerializeField]
        private Sprite m_SplashImage;
        [SerializeField]
        private VerticalBackgroundConfig m_VerticalBackgroundConfig;
        [SerializeField]
        private PlatformHandlerConfig m_PlatformHandlerConfig;
        [SerializeField]
        private EnemyWaveConfig[] m_EnemyWaveConfigs;
        public Sprite SplashImage => m_SplashImage;
        public VerticalBackgroundConfig VerticalBackgroundConfig => m_VerticalBackgroundConfig;
        public StageMode StageMode => m_StageMode;
        public StageState StartingStageState => m_StartingStageState;  
        public EnemyWaveConfig[] EnemyWaveConfigs => m_EnemyWaveConfigs;
        public PlatformHandlerConfig PlatformHandlerConfig => m_PlatformHandlerConfig;
        public int EnergyAmountToPay => m_EnergyAmountToPay;

        private DateTime m_PlayStartTime;
        public EnemyWaveConfig GetEnemyWaveByIndex(int index)
        {
            return m_EnemyWaveConfigs[index];
        }
        public void OnPlayed()
        {
            // Simpan waktu mulai
            m_PlayStartTime = DateTime.UtcNow;
        }

        public void OnOver(bool gameover)
        {
            // Ambil waktu sekarang
            DateTime endTime = DateTime.UtcNow;

            // Hitung durasi
            TimeSpan durationSpan = endTime - m_PlayStartTime;

            float duration = (float)durationSpan.TotalSeconds;

            string game = gameover ? "GameOver" : "Slay the Boss";
        }
    }
}
