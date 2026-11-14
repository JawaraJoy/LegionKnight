using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Feature Access", menuName = "Rush/Tutorial/Feature Access")]
    public sealed class TutorialAccessPolicy : ScriptableObject
    {
        [SerializeField, Min(1)] private int m_MinimumPlayerLevel = 1;
        [SerializeField] private TutorialDefinition m_RequiredTutorial;
        public int MinimumPlayerLevel => m_MinimumPlayerLevel;
        public TutorialDefinition RequiredTutorial => m_RequiredTutorial;
        public bool IsAllowed(int playerLevel, bool developerMode, TutorialHandler tutorials)
        {
            bool ready = m_RequiredTutorial == null ||
                (tutorials != null && tutorials.HasContent(m_RequiredTutorial, out var content) && content.IsDone);
            return TutorialAccessPolicy.LevelIsAllowed(playerLevel, m_MinimumPlayerLevel, developerMode, ready);
        }
        public static bool LevelIsAllowed(int playerLevel, int requiredLevel, bool developerMode, bool tutorialReady) =>
            developerMode || (playerLevel >= requiredLevel && tutorialReady);
    }
}
