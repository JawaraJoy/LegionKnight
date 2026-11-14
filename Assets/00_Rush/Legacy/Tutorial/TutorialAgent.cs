using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class TutorialAgent : MonoBehaviour
    {
        [SerializeField] private Button m_ResumeButton;
        private TutorialManager m_BoundManager;
        public static TutorialManager GetManager() => GameManager.Instance != null ? GameManager.Instance.TutorialManager : null;
        public void StartTutorial(TutorialDefinition definition) => GetManager()?.StartTutorial(definition);
        public void ResumeTutorial() => GetManager()?.ResumeTutorial();
        public void Init()
        {
            var manager = GetManager();
            manager?.Init();
            GameManager.Instance.TutorFlash?.Init();
            BindResumeInternal(manager);
        }
        private void OnEnable()
        {
            if (m_ResumeButton != null) m_ResumeButton.onClick.AddListener(ResumeTutorial);
            if (Application.isPlaying && m_ResumeButton != null) BindResumeInternal(GetManager());
        }
        private void OnDisable()
        {
            if (m_ResumeButton != null) m_ResumeButton.onClick.RemoveListener(ResumeTutorial);
            if (m_BoundManager != null) m_BoundManager.AvailabilityChanged -= RefreshResumeInternal;
        }
        private void BindResumeInternal(TutorialManager manager)
        {
            if (m_BoundManager != null) m_BoundManager.AvailabilityChanged -= RefreshResumeInternal;
            m_BoundManager = manager;
            if (manager != null) manager.AvailabilityChanged += RefreshResumeInternal;
            RefreshResumeInternal();
        }
        private void RefreshResumeInternal()
        {
            if (m_ResumeButton != null) m_ResumeButton.gameObject.SetActive(m_BoundManager != null && m_BoundManager.HasDeferredTutorial);
        }
    }
}
