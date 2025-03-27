using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class ItemView : UIView
    {
        [SerializeField]
        protected Image m_Icon;
        [SerializeField]
        protected TextMeshProUGUI m_Amount;
        [SerializeField]
        private UnityEvent<object> m_OnDefinitionSet = new();

        protected object m_Definition;
        public object Definition => m_Definition;
        public void Init(object defi)
        {
            InitInternal(defi);
        }
        protected virtual void InitInternal(object defi)
        {
            m_Definition = defi;
            OnDefinitionSetInvoke(defi);
        }
        protected virtual void OnDefinitionSetInvoke(object defi)
        {
            m_OnDefinitionSet?.Invoke(defi);
        }
    }
}
