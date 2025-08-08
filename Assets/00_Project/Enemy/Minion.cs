using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Minion : MonoBehaviour, IEnemy
    {
        [SerializeField]
        private MinionDefinition m_Definition;
        [SerializeField]
        private SpriteRenderer m_Looks;
        [SerializeField]
        private int m_DynamicLevel = 1;

        [SerializeField]
        private UnityEvent<AbilityDefinition> m_OnSkillInitialize;

        private void Start()
        {
            Register();
        }
        private int DynamicLevelInternal
        {
            get
            {
                int startLevel = m_Definition.StartLevel;
                int level = Mathf.Max(1, m_DynamicLevel + startLevel);
                return level;
            }
        }
        public int DynamicLevel => DynamicLevelInternal;
        public void Init(MinionDefinition defi)
        {
            m_Definition = defi;
            m_Looks.sprite = m_Definition.Looks;
            m_OnSkillInitialize?.Invoke(m_Definition.AbilityDefinition);
        }

        public void SetDynamicLevel(int level)
        {
            m_DynamicLevel = level;
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
