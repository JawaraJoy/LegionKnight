using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class LevelUpCheckerAgent : MonoBehaviour
    {
        private bool IsLevelUpTriggered
        {
            get
            {
                return Player.Instance.Progression.LevelUpTrigerred;
            }
        }
        private LevelUpPanel m_LevelUpPanel;
        private LevelUpPanel LevelUpPanel
        {
            get
            {
                if (m_LevelUpPanel == null)
                {
                    m_LevelUpPanel = CanvasManager.Instance.GetPanel<LevelUpPanel>();
                }
                return m_LevelUpPanel;
            }
        }
        public void TryToShowLevelUpPanel()
        {
            if (IsLevelUpTriggered)
            {
                LevelUpPanel.Show();
                Player.Instance.Progression.SetLevelUpTriggered(false);
            }
        }
    }
}
