using UnityEngine;

namespace LegionKnight
{
    public partial class UIView : View
    {
        [SerializeField]
        protected string m_UniqueId;
        protected RectTransform RectContent => m_Content.GetComponent<RectTransform>();
        public virtual string UniqueId => m_UniqueId;
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
