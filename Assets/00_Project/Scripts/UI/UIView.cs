using UnityEngine;

namespace LegionKnight
{
    public partial class UIView : View
    {
        [SerializeField]
        private string m_UniqueId;
        public string UniqueId => m_UniqueId;
        public void SetUniqueId(string set)
        {
            SetUniqueIdInternal(set);
        }
        protected virtual void SetUniqueIdInternal(string set)
        {
            m_UniqueId = set;
        }
    }
}
