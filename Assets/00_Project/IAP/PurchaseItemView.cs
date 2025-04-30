using UnityEngine;

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
        protected override void InitInternal(object defi)
        {
            base.InitInternal(defi);
            if (defi is ProductItem item)
            {
                m_Icon.sprite = item.GetIcon();
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
                
            }
        }
    }
}
