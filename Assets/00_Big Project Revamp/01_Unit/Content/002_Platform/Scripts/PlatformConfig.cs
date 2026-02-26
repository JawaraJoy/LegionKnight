using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Platform", menuName = "Rush/Unit/Platform", order = 1)]
    public class PlatformConfig : CollectibleConfig
    {
        [SerializeField]
        private Platform2D m_PlatformPrefab;
        [SerializeField]
        private int m_PrewarmCount = 5;
        [SerializeField, Range(0.01f, 1f)]
        private float m_ChanceToSpawn = 1f;
        [SerializeField]
        private float m_WidthRate = 3f;
        [SerializeField, Range(0.1f, 1f)]
        private float m_PerfectTouchRange = 0.3f;
        [SerializeField]
        private float m_Speed = 5f;
        [SerializeField]
        private SkillConfig m_AttackSkill;
        [SerializeField]
        private PlatformSkillField m_LeftSkillField;
        [SerializeField]
        private PlatformSkillField m_RightSkillField;
        public float ChanceToSpawn => m_ChanceToSpawn;
        public float PerfectTouchRange => m_PerfectTouchRange;
        public float Speed => m_Speed;
        public int PrewarmCount => m_PrewarmCount;
        public Platform2D PlatformPrefab => m_PlatformPrefab;
        public float WidthRate => m_WidthRate;
        public PlatformSkillField LeftSkillField => m_LeftSkillField;
        public PlatformSkillField RightSkillField => m_RightSkillField;
        public SkillConfig AttackSkill => m_AttackSkill;
    }
}
