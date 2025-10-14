using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        private void Awake()
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
                m_Panel = GameManager.Instance.GetPanel<TutorialPanel>();
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
        public void StartTutorial(TutorialDefinition tutorialDefi)
        {
            if (HasContentInternal(tutorialDefi, out TutorialContent content))
            {
                if (content.IsDone || !content.IsUnlocked) return;
                m_CurrentTutorial = content.Definition;
                m_CurrentStep = 0;
                m_MaxStep = m_CurrentTutorial.Steps.Length;
                SetStep(m_CurrentStep);
                OnTutorialStart(tutorialDefi);
            }
        }
        private void NextTutorial()
        {
            if (m_CurrentTutorial == null) return;

            // Check if we can move to the next step safely
            if (m_CurrentStep < m_MaxStep - 1)
            {
                m_CurrentStep++;
                TutorStepDefinition step = m_CurrentTutorial.Steps[m_CurrentStep];

                if (HasTargetInternal(step, out TutorTarget target))
                {
                    OnStepChanged(target);
                }   
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
            if (target.NextButton != null)
            {
                target.NextButton.onClick.RemoveAllListeners();
                target.NextButton.onClick.AddListener(NextTutorial);
            }
            else
            {
                m_SecondNextButton = GetPanelInternal().NextButton;
                m_SecondNextButton.onClick.RemoveAllListeners();
                m_SecondNextButton.onClick.AddListener(NextTutorial);
            }
        }
        private void OnTutorialEndInvoke(TutorialDefinition defi)
        {
            m_OnTutorialEnd?.Invoke(defi);
            GetPanelInternal().Hide();
            if (HasContentInternal(defi, out TutorialContent content)) 
            {
                content.SetIsDone(true);
            }
        }
    }
}
