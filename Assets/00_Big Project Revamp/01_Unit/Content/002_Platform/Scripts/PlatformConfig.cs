using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Platform", menuName = "Rush/Unit/Platform", order = 1)]
    public class PlatformConfig : Configuration, IHasIcon, IHasAttacker
    {
        [SerializeField]
        private Platform2D m_PlatformPrefab;
        [SerializeField]
        private int m_PrewarmCount = 5;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField, Range(0.01f, 1f)]
        private float m_ChanceToSpawn = 1f;
        [SerializeField]
        private float m_WidthRate = 3f;
        [SerializeField, Range(0.1f, 1f)]
        private float m_PerfectTouchRange = 0.3f;
        [SerializeField]
        private float m_Speed = 5f;
        [SerializeField]
        private AttackerField m_AttackerField;
        [SerializeField]
        private PlatformSkillField m_SkillOnLeftTouch;
        [SerializeField]
        private PlatformSkillField m_SkillOnRightTouch;
        public float ChanceToSpawn => m_ChanceToSpawn;
        public float PerfectTouchRange => m_PerfectTouchRange;
        public PlatformSkillField SkillOnLeftTouch => m_SkillOnLeftTouch;
        public PlatformSkillField SkillOnRightTouch => m_SkillOnRightTouch;
        public Sprite Icon => m_Icon;
        public float Speed => m_Speed;
        public int PrewarmCount => m_PrewarmCount;
        public Platform2D PlatformPrefab => m_PlatformPrefab;
        public float WidthRate => m_WidthRate;
        public AttackerField AttackerField => m_AttackerField;
    }
}
