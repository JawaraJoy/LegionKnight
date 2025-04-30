using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public partial class NotEnoughToPurchasedView : UIView
    {
        private ProductDefinition m_Definition;
        [SerializeField]
        private TextMeshProUGUI m_ReasonText;
        public void SetShow(SellProduct product)
        {
            m_Definition = product.Definition;
            m_ReasonText.text = product.Message;
            ShowInternal();
        }

        private void OnEnable()
        {
            UnityService.Instance.OnPurchasedFailedAddListerner(SetShow);
        }

        private void OnDisable()
        {
            UnityService.Instance.OnPurchasedFailedRemoveListerner(SetShow);
        }
    }
}
