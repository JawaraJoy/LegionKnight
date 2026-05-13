using UnityEngine;

namespace LegionKnight
{
    // Base class for all views that are used in the UI. Mainly used to have a unique id for each view, so that we can easily find them in the hierarchy and manage them.
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
