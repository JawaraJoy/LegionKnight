using UnityEngine;

namespace LegionKnight
{
    public partial class TutorialAgent : MonoBehaviour
    {
        private static TutorialManager m_Manager;
        private static TutorFlashHandler m_FlashHandler;
        private static TutorialManager GetManagerInternal()
        {
            if (m_Manager == null)
            {
                m_Manager = GameManager.Instance.TutorialManager;
            }
            return m_Manager;
        }
        private static TutorFlashHandler GetFlashInternal()
        {
            if (m_FlashHandler == null)
            {
                m_FlashHandler = GameManager.Instance.TutorFlash;
            }
            return m_FlashHandler;
        }
        public static TutorialManager GetManager()
        {
            return GetManagerInternal();
        }
        public void StartTutorial(TutorialDefinition tutorialDefi)
        {
            GetManagerInternal().StartTutorial(tutorialDefi);
        }
        public void Init()
        {
            GetManagerInternal().Init();
            //GetFlashInternal().Init();
        }
    }
}
