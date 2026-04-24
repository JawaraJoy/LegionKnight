using UnityEngine;
using LegionKnight;
using TMPro;

namespace Rush
{
    public class UnitNameView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_NameText;

        public void SetNameText(Unit unit)
        {
            m_NameText.text = unit.Config.BaseInfo.Name;
        }
    }
}
