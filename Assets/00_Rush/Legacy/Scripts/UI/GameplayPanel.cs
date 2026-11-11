using Rush;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class GameplayPanel : PanelView
    {
        [SerializeField]
        private GameObject m_BossDetailContent;
        [SerializeField]
        private Button   m_BossDetailButton;
        public GameObject BossDetailContent => m_BossDetailContent;

        private EnemyWaveHandler m_EnemyWaveHandler;
        private UnitDetailPanel m_UnitDetailPanel;

        private void Awake()
        {
            m_EnemyWaveHandler = RushGameManager.Instance.StageManager.EnemyWaveHandler;
            m_UnitDetailPanel = CanvasManager.Instance.GetPanel<UnitDetailPanel>();
            m_BossDetailButton.onClick.AddListener(ShowBossDetail);

        }
        public void ShowBossDetailContent(bool set)
        {
            m_BossDetailContent.SetActive(set);
        }

        private void ShowBossDetail()
        {
            if (m_EnemyWaveHandler == null)
            {
                Debug.LogError("EnemyWaveHandler is null");
                return;
            }
            Unit bossUnitConfig = m_EnemyWaveHandler.BossUnitExisten;
            if (bossUnitConfig == null)
            {
                Debug.LogError("BossUnitConfig is null");
                return;
            }
            if (m_UnitDetailPanel == null)
            {
                Debug.LogError("UnitDetailPanel is null");
                return;
            }
            m_UnitDetailPanel.SetPreview(bossUnitConfig);
        }
    }
}
