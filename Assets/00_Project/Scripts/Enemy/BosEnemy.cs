using System.Linq;
using UnityEngine;

namespace LegionKnight
{
    public partial class BosEnemy : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_BosForm;

        private int m_BosLevel = 0;
        public void Init(BosDefinition definition)
        {
            m_BosDefinition = definition;
            Stat bosStat = m_BosDefinition.FinalStat(0);
            SetBosLevelInternal(m_BosDefinition.StartLevel);
            int atk = bosStat.Attack;
            int def = bosStat.Defense;
            int health = bosStat.Health;

            m_Damageable.SetHealth(health);
            m_Damageable.SetDamage(atk);
            m_Damageable.SetDefend(def);

            InitSkill(m_BosDefinition.Skills.ToList());
            m_BosSkill.SetOwner(SkillOwner.Boss);
        }
        public void SetLocalPosition(Vector2 post)
        {
            transform.localPosition = post;
        }

        public void SetBosLevel(int level)
        {
            SetBosLevelInternal(level);
        }

        protected void SetBosLevelInternal(int level)
        {
            m_BosLevel = level;
        }

        public int GetBosLevel()
        {
            return m_BosLevel;
        }
    }
}
