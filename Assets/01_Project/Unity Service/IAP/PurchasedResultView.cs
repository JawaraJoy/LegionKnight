using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class PurchasedResultView : ResultView
    {
        public virtual void ShowResults(SellProduct product)
        {
            bool isBonusAvailable = product.IsBonusAvailable;
            List<object> items = new (product.GetAllProductItems(isBonusAvailable));
            ShowResultsInternal(items);
        }
        private void OnEnable()
        {
            UnityService.Instance.OnPurchasedCompleteAddListerner(ShowResults);
        }
        private void OnDisable()
        {
            UnityService.Instance.OnPurchasedCompleteRemoveListerner(ShowResults);
        }
    }
}
