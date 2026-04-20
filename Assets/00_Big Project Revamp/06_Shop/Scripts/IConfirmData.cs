using UnityEngine;

namespace Rush
{
    public interface IConfirmData
    {
        string DescriptionText { get; }

        // Main currency
        int MainCurrencyAmount { get; }
        int OriginalCost { get; }
        bool HasDiscount { get; }
        string MainCurrencyName { get; }
        Sprite MainCurrencyIcon { get; }
        bool IsFree { get; }

        // Alt currency — shop selalu false / 0 / null
        bool HasAltCurrency { get; }
        int AltCurrencyAmount { get; }
        string AltCurrencyName { get; }
        Sprite AltCurrencyIcon { get; }
    }
}