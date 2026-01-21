using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public abstract partial class DrawButtonView : UIView
    {
        [SerializeField]
        protected Button m_DrawButton;
        [SerializeField]
        protected TextMeshProUGUI m_DrawAmount;
        [SerializeField]
        private Image m_DrawIcon;
        [SerializeField]
        protected GachaCurrencyCost m_Cost;

        [SerializeField]
        private UnityEvent<UnityAction> m_OnButtonClick = new();
        [SerializeField]
        private UnityEvent<GachaCurrencyCost> m_OnButtonClickBanner = new();
        public virtual void SetButtonView(GachaCurrencyCost cost)
        {

            m_Cost = cost;
            m_DrawIcon.sprite = m_Cost.Definition.Icon;
            m_DrawAmount.text = $"x{m_Cost.Amount}";
        }
        public void SetDrawAmount(int amount)
        {
            m_DrawAmount.text = amount.ToString();
        }
        protected virtual void OnButtonClick(UnityAction action)
        {
            m_OnButtonClick?.Invoke(action);
        }
        protected virtual void OnButtonClickBanner(GachaCurrencyCost cost)
        {
            m_OnButtonClickBanner?.Invoke(cost);
        }
        public void AddButtonAction(UnityAction action)
        {
            m_DrawButton.onClick.RemoveAllListeners();
            m_DrawButton.onClick.AddListener(action);
        }
    }
}
