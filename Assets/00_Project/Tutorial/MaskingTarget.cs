using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class MaskingTarget : MonoBehaviour
    {
        [SerializeField]
        private DialogueDefinition m_DialogueDefinition;
        [SerializeField]
        private UnityEvent<MaskingTarget> m_OnTutorialStart = new();
        [SerializeField]
        private UnityEvent<MaskingTarget> m_OnTutorialEnd = new();

        [SerializeField]
        private bool m_CanTutorial = false;
        [SerializeField]
        private bool m_TutorialCompleted = false;
        public DialogueDefinition DialogueDefinition => m_DialogueDefinition;
        public string DialogueId => m_DialogueDefinition != null ? m_DialogueDefinition.Id : string.Empty;
        public string DialogueTitle => m_DialogueDefinition != null ? m_DialogueDefinition.Title : string.Empty;
        public string[] DialogueDescriptions => m_DialogueDefinition != null ? m_DialogueDefinition.Descriptions : new string[0];

        private string CanTutorialKey => $"{m_DialogueDefinition.Id}canTutorial";
        private string TutorialCompletedKey => $"{m_DialogueDefinition.Id}complete";
        public bool IsTutorialCompleted => m_TutorialCompleted;
        public void Init()
        {
            InitInternal();
        }
        private void InitInternal()
        {
            if (UnityService.Instance.HasData(TutorialCompletedKey))
            {
                m_TutorialCompleted = UnityService.Instance.GetData<bool>(TutorialCompletedKey);
            }
            else
            {
                m_TutorialCompleted = false;
            }
            if (UnityService.Instance.HasData(CanTutorialKey))
            {
                m_CanTutorial = UnityService.Instance.GetData<bool>(CanTutorialKey);
            }
            Debug.Log($"Tutorial {m_DialogueDefinition.Id} is Completed = {m_TutorialCompleted}");
        }
        public void SetCanTutor(bool canTutor)
        {
            m_CanTutorial = canTutor;
            UnityService.Instance.SaveData(CanTutorialKey, m_CanTutorial);
        }
        public string GetDialogueDescription(int index)
        {
            if (m_DialogueDefinition != null)
            {
                return m_DialogueDefinition.GetDescription(index);
            }
            return string.Empty;
        }
        public int DialogueDescriptionCount => m_DialogueDefinition != null ? m_DialogueDefinition.DescriptionCount : 0;
        public void StartDialogue()
        {
            if (m_TutorialCompleted)
            {
                Debug.Log("Tutorial already completed.");
                return;
            }
            if (m_DialogueDefinition != null)
            {
                m_DialogueDefinition.StartTutorial();
                OnTutorialStartInvoke();
            }
            else
            {
                Debug.LogError("DialogueDefinition is null.");
            }
        }
        public void EndDialogue()
        {
            if (m_TutorialCompleted)
            {
                Debug.Log("Tutorial already completed.");
                return;
            }
            m_TutorialCompleted = true;
            UnityService.Instance.SaveData(TutorialCompletedKey, m_TutorialCompleted);
            OnTutorialEndInvoke();
            GameManager.Instance.EndTutorialPanel();
        }

        private void OnTutorialStartInvoke()
        {
            m_OnTutorialStart?.Invoke(this);
        }
        private void OnTutorialEndInvoke()
        {
            m_OnTutorialEnd?.Invoke(this);
        }
    }
}
