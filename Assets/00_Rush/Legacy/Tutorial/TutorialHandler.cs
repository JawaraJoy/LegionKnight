using LegionKnight.Dialogue;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Rush;

namespace LegionKnight
{
    public partial class TutorialHandler : MonoBehaviour
    {
        private TutorialDefinition m_CurrentTutorial;
        [SerializeField]
        private TutorialContent[] m_Contents;
        [SerializeField, MMReadOnly]
        private List<TutorTarget> m_TutorTargets = new ();
        [SerializeField]
        private UnityEvent<TutorialDefinition> m_OnTutorialStart = new();
        [SerializeField]
        private UnityEvent<TutorTarget> m_OnStepChanged = new();
        [SerializeField]
        private UnityEvent<TutorialDefinition> m_OnTutorialEnd = new();
        public TutorialDefinition CurrentTutorial => m_CurrentTutorial; 

        private static TutorialPanel m_Panel;
        private static int m_CurrentStep;
        private static int m_MaxStep;

        [SerializeField, MMReadOnly]
        private Button m_SecondNextButton;
        private void Start()
        {
            TutorTarget[] targets = FindObjectsByType<TutorTarget>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            m_TutorTargets = new List<TutorTarget>(targets);
        }
        public void Init()
        {
            foreach(TutorialContent content in m_Contents)
            {
                content.Init();
            }
        }
        private TutorialContent GetContent(TutorialDefinition defi)
        {
            TutorialContent content = null;
            foreach (TutorialContent c in m_Contents)
            {
                if (c.Definition == defi)
                {
                    content = c;
                }
            }
            return content;
        }

        public void AddTarget(TutorTarget target)
        {
            if (!m_TutorTargets.Contains(target))
            {
                m_TutorTargets.Add(target);
            }
        }
        public void RemoveTarget(TutorTarget target)
        {
            if (m_TutorTargets.Contains(target))
            {
                m_TutorTargets.Remove(target);
            }
        }
        private bool HasContentInternal(TutorialDefinition defi, out TutorialContent content)
        {
            content = GetContent(defi);
            return content != null;
        }
        public bool HasContent(TutorialDefinition defi, out TutorialContent content)
        {
            content = GetContent(defi);
            return content != null;
        }
        private static TutorialPanel GetPanelInternal()
        {
            if (m_Panel == null)
            {
                m_Panel = CanvasManager.Instance.GetPanel<TutorialPanel>();
            }
            return m_Panel;
        }
        public static TutorialPanel GetPanel()
        {
            return GetPanelInternal();
        }
        private TutorTarget GetTarget(TutorStepDefinition defi)
        {
            TutorTarget target = m_TutorTargets.Find(x => x.Definition == defi);
            if (target == null)
            {
                return null;
            }
            return target;
        }

        private bool HasTargetInternal(TutorStepDefinition defi, out TutorTarget target)
        {
            target = GetTarget(defi);
            return target != null;
        }
        private DialoguePanel m_DialoguePanel;
        private DialoguePanel GetDialoguePanel()
        {
            if (m_DialoguePanel == null)
            {
                m_DialoguePanel = CanvasManager.Instance.GetPanel<DialoguePanel>();
            }
            return m_DialoguePanel;
        }
        private void SetRaycastBlock(bool set)
        {
            Image blockRaycas = GetDialoguePanel().RaycastBlockImage;
            if (blockRaycas != null)
            {
                blockRaycas.raycastTarget = set;
            }
        }
        public void StartTutorial(TutorialDefinition tutorialDefi)
        {
            if (GameSetting.Instance.DeveloperMode) return;
            if (m_CurrentTutorial != null) return;
            if (HasContentInternal(tutorialDefi, out TutorialContent content))
            {
                // Check if the tutorial is already done or locked
                if (content.IsDone || !content.IsUnlocked) return;
                // Start the tutorial if all checks are passed
                m_CurrentTutorial = content.Definition;
                m_CurrentStep = 0;
                m_MaxStep = m_CurrentTutorial.Steps.Length;
                SetStep(m_CurrentStep);
                OnTutorialStart(m_CurrentTutorial);
                content.OnTutorialStart.Invoke(m_CurrentTutorial);
                SetRaycastBlock(false);
            }
        }
        private void NextTutorial()
        {
            if (m_CurrentTutorial == null) return;
            // Check if we can move to the next step safely
            TutorStepDefinition step = m_CurrentTutorial.Steps[m_CurrentStep];
            if (step != null)
            {
                step.OnStepEnd?.Invoke();
            }
            if (m_CurrentStep < m_MaxStep - 1)
            {
                m_CurrentStep++;
                SetStep(m_CurrentStep);
            }
            else
            {
                EndTutorial();
            }
        }
        private void EndTutorial()
        {
            GetPanelInternal().Hide();
            OnTutorialEndInvoke(m_CurrentTutorial);
        }
        private void SetStep(int stepIndex)
        {
            TutorStepDefinition step = m_CurrentTutorial.Steps[stepIndex];
            if (HasTargetInternal(step, out TutorTarget target))
            {
                OnStepChanged(target);
            } 
        }
        private void OnTutorialStart(TutorialDefinition defi)
        {
            m_OnTutorialStart?.Invoke(defi);
            GetPanelInternal().Show();
        }
        private void OnStepChanged(TutorTarget target)
        {
            m_OnStepChanged?.Invoke(target);
            GetPanelInternal().SetTutorial(target);
            GetPanelInternal().Refresh();

            TutorStepDefinition defi = target.Definition;

            defi.OnStepStart?.Invoke();
            bool hasconversation = defi.Conversation;
            if (hasconversation)
            {
                GameManager.Instance.StartConversation(defi.Conversation);
            }
            else
            {
                GetDialoguePanel().Hide();
            }

            if (target.NextButton != null)
            {
                m_SecondNextButton = target.NextButton;
            }
            else
            {
                m_SecondNextButton = GetPanelInternal().NextButton;

            }
            m_SecondNextButton.onClick?.RemoveListener(NextTutorial);
            m_SecondNextButton.onClick.AddListener(NextTutorial);
        }
        private void OnTutorialEndInvoke(TutorialDefinition defi)
        {
            m_OnTutorialEnd?.Invoke(defi);
            SetRaycastBlock(true);
            GetDialoguePanel().Hide();
            GetPanelInternal().Hide();
            if (HasContentInternal(defi, out TutorialContent content)) 
            {
                content.SetIsDone(true);
                content.OnTutorialEnd?.Invoke(m_CurrentTutorial);
            }
            m_CurrentTutorial = null;
        }
    }
}
