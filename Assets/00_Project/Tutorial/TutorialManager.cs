using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class TutorialManager : TutorialHandler
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private TutorialManager m_TutorialManager;
        public TutorialManager TutorialManager => m_TutorialManager;
    }
}
