using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class RechargePanel : PanelView
    {
        public override void ShowBinding(string uniqueId)
        {
            foreach (UIView binding in m_Bindings)
            {
                binding.Hide();
            }
            base.ShowBinding(uniqueId);
            
        }
    }
}
