using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class TutorialHandler : MonoBehaviour
    {
        private MaskingTarget m_CurrentMaskingTarget;
        [SerializeField]
        private List<MaskingTarget> m_MaskingTargets = new ();

        [SerializeField]
        private UnityEvent<MaskingTarget> m_OnTutorialStart = new();
        [SerializeField]
        private UnityEvent<MaskingTarget> m_OnTutorialDialogueChanged = new();
        [SerializeField]
        private UnityEvent<MaskingTarget> m_OnTutorialEnd = new();
        public MaskingTarget CurrentMaskingTarget => m_CurrentMaskingTarget;
        public string DialogueId => m_CurrentMaskingTarget != null ? m_CurrentMaskingTarget.DialogueId : string.Empty;
        public string DialogueTitle => m_CurrentMaskingTarget != null ? m_CurrentMaskingTarget.DialogueId : string.Empty;
        public string[] DialogueDescriptions => m_CurrentMaskingTarget != null ? m_CurrentMaskingTarget.DialogueDescriptions : new string[0];

        private int m_DescriptionIndex = 0;
        public int DescriptionIndex => m_DescriptionIndex;
        public bool CanTutor => m_CurrentMaskingTarget != null ? m_CurrentMaskingTarget.CanTutor : false;
        public string GetDialogueDescription(int index)
        {
            if (m_CurrentMaskingTarget != null)
            {
                return m_CurrentMaskingTarget.GetDialogueDescription(index);
            }
            return string.Empty;
        }
        public int DialogueDescriptionCount => m_CurrentMaskingTarget != null ? m_CurrentMaskingTarget.DialogueDescriptionCount : 0;
        public void Init()
        {
            m_MaskingTargets.Clear();
            MaskingTarget[] maskingTargets = FindObjectsByType<MaskingTarget>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (MaskingTarget maskingTarget in maskingTargets)
            {
                if (!m_MaskingTargets.Contains(maskingTarget))
                {
                    m_MaskingTargets.Add(maskingTarget);
                    maskingTarget.Init();
                }
            }
        }
        public void SetUnlockTutor(string id, bool unlock)
        {
            GetMaskingTargetInternal(id)?.SetUnlockTutor(unlock);
        }
        private MaskingTarget GetMaskingTargetInternal(string id)
        {
            foreach (MaskingTarget maskingTarget in m_MaskingTargets)
            {
                if (maskingTarget.DialogueId == id)
                {
                    return maskingTarget;
                }
            }
            Debug.LogError($"Masking target with ID {id} not found.");
            return null;
        }
        public void StartTutorial(DialogueDefinition defi)
        {
            m_CurrentMaskingTarget = GetMaskingTargetInternal(defi.Id);
            if (m_CurrentMaskingTarget == null)
            {
                Debug.LogError($"Masking target with ID {defi} not found.");
                return;
            }
            if (!m_CurrentMaskingTarget.CanTutor)
            {
                Debug.Log("Tutorial Cant be played");
                OnTutorialEndInvoke(m_CurrentMaskingTarget);
                return;
            }
            if (m_CurrentMaskingTarget != null)
            {
                OnTutorialStart(m_CurrentMaskingTarget);
            }
            //GameManager.Instance.PlayTutorialPanel(m_CurrentMaskingTarget);
        }

        public void NextDialogue()
        {
            if (m_CurrentMaskingTarget == null)
            {
                Debug.LogError("Current masking target is null.");
                return;
            }
            if (m_CurrentMaskingTarget.IsTutorialCompleted)
            {
                Debug.Log("Tutorial already completed.");
                return;
            }
            m_DescriptionIndex++;
            if (m_DescriptionIndex >= m_CurrentMaskingTarget.DialogueDescriptionCount)
            {
                m_DescriptionIndex = m_CurrentMaskingTarget.DialogueDescriptionCount;
                return;
            }
            OnTutorialDialogueChanged(m_CurrentMaskingTarget);
        }

        public void EndDialogue()
        {
            if (m_CurrentMaskingTarget == null)
            {
                Debug.LogError("Current masking target is null.");
                return;
            }
            m_CurrentMaskingTarget.EndDialogue();
            OnTutorialEndInvoke(m_CurrentMaskingTarget);
            m_CurrentMaskingTarget = null;
        }

        private void OnTutorialStart(MaskingTarget maskingTarget)
        {
            m_OnTutorialStart?.Invoke(maskingTarget);
        }
        private void OnTutorialDialogueChanged(MaskingTarget maskingTarget)
        {
            m_OnTutorialDialogueChanged?.Invoke(maskingTarget);
        }
        private void OnTutorialEndInvoke(MaskingTarget maskingTarget)
        {
            m_OnTutorialEnd?.Invoke(maskingTarget);
        }
    }
}
