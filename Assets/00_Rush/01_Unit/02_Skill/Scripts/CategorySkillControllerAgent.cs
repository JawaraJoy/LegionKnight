using UnityEngine;

namespace Rush
{
    public class CategorySkillControllerAgent : MonoBehaviour
    {
        [SerializeField]
        private CategorySkillController m_CategorySkillController;
        [SerializeField]
        private int m_ChargeAmountMultiplyByPerfect = 1;

        [SerializeField] private BattleCardRunBonuses m_CardBonuses;

        public void AddChargeByPerfect(int perfectCombo)
        {
            int totalAmount = (perfectCombo + 1) * m_ChargeAmountMultiplyByPerfect;
            if (perfectCombo > 0 && m_CardBonuses != null) totalAmount += m_CardBonuses.PerfectCharge;
            m_CategorySkillController.AddCharge(totalAmount);
        }
    }
}
