using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class StageSelectView : UIView
    {
        [SerializeField]
        private StageConfig m_StageConfig;

        [SerializeField]
        private Image m_LevelImage;
        [SerializeField]
        private TextMeshProUGUI m_LevelNameText;
        [SerializeField]
        private Button m_StartButton;
        [SerializeField]
        private GameObject m_LockImage;
        [SerializeField]
        private GameObject m_CompleteImage;
        private void OnEnable()
        {
            Init();
        }
        private void Init()
        {
            bool devMode = GameSetting.Instance.DeveloperMode;
        }
        public void StartLevel()
        {
            if (m_StageConfig != null)
            {
                
            }
            else
            {
                Debug.LogError("stage config is not set.");
            }
        }
    }
}
