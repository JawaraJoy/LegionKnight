using UnityEngine;

namespace Rush
{
    // Kontrak yang harus dipenuhi semua confirm data
    // agar CurrencyConfirmationPanel tidak perlu tahu detail gacha atau shop
    public interface IConfirmData
    {
        string DescriptionText { get; }
        int MainCurrencyAmount { get; }
        string MainCurrencyName { get; }
        Sprite MainCurrencyIcon { get; }
        bool IsFree { get; }

        // Alt currency — optional, shop tidak pakai
        bool HasAltCurrency { get; }
        int AltCurrencyAmount { get; }
        string AltCurrencyName { get; }
    }
}