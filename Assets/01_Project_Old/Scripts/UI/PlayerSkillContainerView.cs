using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerSkillContainerView : SkillContainerView
    {
        
    }
    public partial class GameplayPanel
    {
        
    }

    public partial class GameplayPanelAgent
    {
        private GameplayPanel m_Panel;
        private GameplayPanel GetPanelInternal()
        {
            if (m_Panel == null)
            {
                m_Panel = CanvasManager.Instance.GetPanel<GameplayPanel>();
            }
            return m_Panel;
        }
        
    }
}
