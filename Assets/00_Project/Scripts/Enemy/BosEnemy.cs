using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class BosEnemy : MonoBehaviour, IEnemy
    {
        [SerializeField]
        private SpriteRenderer m_BosForm;

        private int m_BosLevel = 0;

        [SerializeField]
        private UnityEvent<BosDefinition> m_OnBossDeath;

        private void Start()
        {
            Register();
        }
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

        public void Death()
        {
            m_OnBossDeath.Invoke(m_BosDefinition);
        }

        public void Register()
        {
            GameManager.Instance.AddEnemy(this);
        }

        public void UnRegister()
        {
            GameManager.Instance.RemoveEnemy(this);
        }
    }
}
