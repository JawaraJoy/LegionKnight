using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class UnitDetailPanel : PanelView
    {
        [SerializeField]
        private UnitPreview m_Preview;
        [SerializeField] private BossDetailsView m_BossDetails;
        public UnitPreview Preview => m_Preview;
        public void SetPreview(Unit unit)
        {
            if (unit == null || unit.Config == null) return;
            ShowInternal();
            if (!IsShown) return;
            if (m_BossDetails != null) m_BossDetails.Display(unit);
            else m_Preview.SetPreview(unit);
        }
    }
}
