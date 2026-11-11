using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "SkillCat_", menuName = "Rush/Combat/SkillCategory", order = 2)]
    public class SkillCategoryConfig : Configuration
    {
        [SerializeField, Tooltip("Allow cards to learn skills in this category outside the active hero's default deck.")]
        private bool m_AllowsCrossHeroLearning;

        public bool AllowsCrossHeroLearning => m_AllowsCrossHeroLearning;
    }
}
