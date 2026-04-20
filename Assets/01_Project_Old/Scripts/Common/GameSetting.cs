using UnityEngine;

namespace Rush
{

    public partial class GameSetting : Singleton<GameSetting>
    {
        [SerializeField]
        private int m_FrameTarget = 30;
        [SerializeField]
        private bool m_DeveloperMode = false;
        public bool DeveloperMode => m_DeveloperMode;

        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = m_FrameTarget;
        }
    }
}
