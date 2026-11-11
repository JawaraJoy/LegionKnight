using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Tutorial", menuName = "Legion Knight/Tutorial/Tutorial")]
    public class TutorialDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private bool m_UnlockedAtFirst;
        [SerializeField]
        private TutorStepDefinition[] m_Steps;
        public TutorStepDefinition[] Steps => m_Steps;
        public string Id => m_Id;
        public bool UnlockedAtFirst => m_UnlockedAtFirst;

        public void SetIsUnlocked(bool isUnlocked)
        {
            if (TutorialAgent.GetManager().HasContent(this, out TutorialContent content))
            {
                content.SetIsUnlocked(isUnlocked);
            }
        }
    }
}
