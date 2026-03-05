using UnityEngine;

namespace Rush
{
    public class CategorySkillControllerAgent : MonoBehaviour
    {
        [SerializeField]
        private CategorySkillController m_CategorySkillController;
        [SerializeField]
        private int m_ChargeAmountMultiplyByPerfect = 1;

        public void AddChargeByPerfect(int perfectCombo)
        {
            int totalAmount = (perfectCombo + 1) * m_ChargeAmountMultiplyByPerfect;
            m_CategorySkillController.AddCharge(totalAmount);
        }
    }
}
