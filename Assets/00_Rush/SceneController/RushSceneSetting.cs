using LegionKnight;
using UnityEngine;

namespace Rush
{
    public partial class RushSceneSetting : SceneHandler
    {
        
    }
    public partial class GameSetting
    {
        [SerializeField]
        private RushSceneSetting m_SceneSetting;
        public RushSceneSetting SceneSetting => m_SceneSetting;
    }
}
