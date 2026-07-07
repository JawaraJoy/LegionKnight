using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class UnitDetailPanel : PanelView
    {
        [SerializeField]
        private UnitPreview m_Preview;
        public UnitPreview Preview => m_Preview;
        public void SetPreview(Unit unit)
        {
            ShowInternal();
            m_Preview.SetPreview(unit);
        }
    }
}
