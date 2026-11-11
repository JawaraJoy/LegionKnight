using UnityEngine;

namespace Rush
{
    public class PlayerCategorySkillControllerAgent : MonoBehaviour
    {
        [SerializeField]
        private SkillCategoryConfig m_SkillCategory;

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
                if (m_SkillController.HasCategoryController(m_SkillCategory, out CategorySkillController categorySkillController))
                {
                    m_CategorySkillController = categorySkillController;
                }
                else
                {
                    Debug.LogError($"Player unit does not have a CategorySkillController for category {m_SkillCategory.name}.");
                }
            }
            else
            {
                Debug.LogError("Player unit does not have a SkillController.");
            }
        }

        public void AddCharge(int chargeAmount)
        {
            if (m_CategorySkillController != null)
            {
                m_CategorySkillController.AddCharge(chargeAmount);
            }
        }
        public void ForceActives()
        {
            if (m_CategorySkillController != null)
            {
                m_CategorySkillController.ForceActives();
            }
        }
    }
}
