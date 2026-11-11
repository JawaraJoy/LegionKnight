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
        [SerializeField]
        private TutorFlashHandler m_TutorFlash;
        public TutorialManager TutorialManager => m_TutorialManager;
        public TutorFlashHandler TutorFlash => m_TutorFlash;
    }
}
