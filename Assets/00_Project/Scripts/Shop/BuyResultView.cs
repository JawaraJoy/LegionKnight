using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class BuyResultView : ResultView
    {
        public virtual void ShowResults(ShopItemDefinition result)
        {
            ShowResultsInternal(new List<object> {result });
        }


    }
}
