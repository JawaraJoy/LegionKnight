using UnityEngine;
using Rush;

namespace LegionKnight
{
    public enum ProductType
    {
        Main,
        Additional,
        Bonus
    }
    public partial class PurchaseItemView : ItemView
    {

        [SerializeField]
        private ProductType m_ProductType = ProductType.Main;
        protected override void InitInternal(CollectibleConfig collectibleConfig)
        {
            base.InitInternal(collectibleConfig);
            if (collectibleConfig is ProductItemConfig item)
            {
                m_Icon.sprite = item.CollectibleConfig.CollectibleField.Icon;
                string amountText = item.Amount.ToString();
                switch (m_ProductType)
                {
                    case ProductType.Main:
                        m_Amount.text = "x" + amountText;
                        break;
                    case ProductType.Additional:
                        m_Amount.text = "x" + amountText;
                        break;
                    case ProductType.Bonus:
                        m_Amount.text = "+" + amountText + " At First Purchase";
                        break;

                }
                if (TryGetComponent(out TextView textView))
                {
                    textView.SetText(item.BaseInfo.Name);
                }
            }
        }
    }
}
