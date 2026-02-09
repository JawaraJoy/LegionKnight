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
        protected TextMeshProUGUI m_DrawOriginalAmount;
        [SerializeField]
        protected GameObject m_OriginalAmountContent;
        [SerializeField]
        private Image m_DrawIcon;
        [SerializeField]
        protected GachaCurrencyCost m_Cost;

        [SerializeField]
        private UnityEvent<UnityAction> m_OnButtonClick = new();
        [SerializeField]
        private UnityEvent<GachaCurrencyCost> m_OnButtonClickBanner = new();
        protected GachaHandler m_GachaHandler;

        protected GachaHandler GachaHandler
        {
            get
            {
                if (m_GachaHandler == null)
                    m_GachaHandler = GameManager.Instance.GachaMananger;
                return m_GachaHandler;
            }
        }
        public virtual void SetButtonView(GachaCurrencyCost cost, int originalAmount)
        {

            m_Cost = cost;
            m_DrawIcon.sprite = cost.Definition.Icon;
            bool isDiscounted = originalAmount > cost.Amount;
            m_OriginalAmountContent.SetActive(isDiscounted);

            m_DrawOriginalAmount.text = $"<s>x{originalAmount}</s>";
            m_DrawAmount.text = $"x{cost.Amount}";
            
        }
        public void SetDrawAmount(int amount)
        {
            m_DrawAmount.text = amount.ToString();
        }
        protected virtual void OnButtonClick(UnityAction action)
        {
            //m_OnButtonClick?.Invoke(action);
        }
        protected virtual void OnButtonClickBanner(GachaCurrencyCost cost)
        {
            //m_OnButtonClickBanner?.Invoke(cost);
        }
        public void AddButtonAction(UnityAction action)
        {
            //m_DrawButton.onClick.RemoveAllListeners();
            //m_DrawButton.onClick.AddListener(action);
        }
    }
}
