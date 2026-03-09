using UnityEngine;

namespace Rush
{
    public class PlayerSkillAgent : MonoBehaviour
    {
        private Unit m_PlayerUnit;
        private SkillController m_SkillController;
        private CategorySkillController m_CategorySkillController;

        private void Start()
        {
            m_PlayerUnit = RushPlayer.Instance.Unit;
            if (m_PlayerUnit == null)
            {
                Debug.LogError("Player unit is not assigned.");
                return;
            }
            if (m_PlayerUnit.HasBind(out SkillController skillController))
            {
                m_SkillController = skillController;

            }
        }
        public void ForceActive(SkillConfig config)
        {
            if (m_SkillController == null)
            {
                Debug.LogError("SkillController is not assigned.");
                return;
            }
            m_SkillController.ForceActive(config);
        }
    }
}
