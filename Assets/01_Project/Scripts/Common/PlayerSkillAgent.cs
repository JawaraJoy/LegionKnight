using UnityEngine;

namespace LegionKnight
{
    public class PlayerSkillAgent : MonoBehaviour
    {
        private PlayerSkill m_Skill;

        private PlayerSkill Skill
        {
            get
            {
                if (m_Skill == null)
                {
                    m_Skill = Player.Instance.Skill;
                }
                return m_Skill;
            }
        }
        public void AddManaOvertime(int add, float time)
        {
            Player.Instance.AddManaOvertime(add, time);
        }

        public void SetCanActiveSkill(bool canActive)
        {
            Skill.SetCanActive(canActive);
        }
    }
}
