using UnityEngine;

namespace LegionKnight
{
    public partial class TutorialAgent : MonoBehaviour
    {
        private static TutorialManager m_Manager;
        private static TutorialManager GetManagerInternal()
        {
            if (m_Manager == null)
            {
                m_Manager = GameManager.Instance.TutorialManager;
            }
            return m_Manager;
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
        }
    }
}
