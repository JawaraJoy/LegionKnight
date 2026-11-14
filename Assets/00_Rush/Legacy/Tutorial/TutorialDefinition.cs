using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Tutorial", menuName = "Legion Knight/Tutorial/Tutorial")]
    public class TutorialDefinition : ScriptableObject
    {
        [SerializeField] private string m_Id;
        [SerializeField] private bool m_UnlockedAtFirst;
        [SerializeField] private TutorStepDefinition[] m_Steps;
        [SerializeField] private bool m_IsEnabled = true;
        [SerializeField] private TutorialDefinition[] m_UnlockOnCompletion = System.Array.Empty<TutorialDefinition>();
        public TutorStepDefinition[] Steps => m_Steps;
        public string Id => m_Id;
        public bool UnlockedAtFirst => m_UnlockedAtFirst;
        public bool IsEnabled => m_IsEnabled;
        public TutorialDefinition[] UnlockOnCompletion => m_UnlockOnCompletion;
        public string ValidationError
        {
            get
            {
                if (!m_IsEnabled) return "Tutorial is a disabled draft.";
                if (string.IsNullOrWhiteSpace(m_Id)) return "Tutorial ID is empty.";
                if (m_Steps == null || m_Steps.Length == 0) return "Tutorial has no steps.";
                var ids = new HashSet<string>();
                foreach (var step in m_Steps)
                {
                    if (step == null) return "Tutorial has a null step.";
                    if (string.IsNullOrWhiteSpace(step.Id)) return "Step ID is empty: " + step.name;
                    if (!ids.Add(step.Id)) return "Duplicate step ID: " + step.Id;
                    if (!step.RequiresTarget && step.CompletionAction == Rush.TutorialAction.None)
                        return "A step needs a target or a completion action: " + step.Id;
                }
                return string.Empty;
            }
        }
        public void SetIsUnlocked(bool value)
        {
            var manager = TutorialAgent.GetManager();
            if (manager != null && manager.HasContent(this, out var content)) content.SetIsUnlocked(value);
        }
    }
}
