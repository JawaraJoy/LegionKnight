using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public static partial class MasterPanelUtility 
    {
        private static bool m_IsShow = false;
        public static bool IsShow => m_IsShow;
        private static UnityAction<bool> m_OnMasterPanelShow;
        public static UnityAction<bool> OnMasterPanelShow => m_OnMasterPanelShow;
        public static void SetIsShow(bool set)
        {
            m_IsShow = set;
            m_OnMasterPanelShow?.Invoke(m_IsShow);
        }
    }
}
