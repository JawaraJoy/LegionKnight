using LegionKnight;
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
        private SceneConfig m_SceneConfig;
        [SerializeField]
        private Sprite m_SplashImage;
        [SerializeField]
        private BackgroundSetField m_BackgroundSetField;
        [SerializeField]
        private PlatformHandlerConfig m_PlatformHandlerConfig;
        [SerializeField]
        private EnemyWaveConfig[] m_EnemyWaveConfigs;
        public Sprite SplashImage => m_SplashImage;
        public BackgroundSetField BackgroundSetField => m_BackgroundSetField;
        public StageMode StageMode => m_StageMode;
        public StageState StartingStageState => m_StartingStageState;  
        public EnemyWaveConfig[] EnemyWaveConfigs => m_EnemyWaveConfigs;

        public EnemyWaveConfig GetEnemyWaveByIndex(int index)
        {
            return m_EnemyWaveConfigs[index];
        }
    }
}
