using UnityEngine;

namespace LegionKnight
{
    public partial class HomeCurrencyViewAgent : MonoBehaviour
    {
        public void SetCurrencyView(Currency currency)
        {
            GameManager.Instance.SetHomeCurrencyViewAmount(currency);
        }
    }
}
