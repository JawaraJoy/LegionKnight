using UnityEngine;

namespace LegionKnight
{
    public partial class HomePanelAgent : MonoBehaviour
    {
        public void SetCurrencyView(Currency currency)
        {
            GameManager.Instance.SetHomeCurrencyView(currency);
        }
        public void SetHighScoreView(Currency currency)
        {
            GameManager.Instance.SetHomeHighScoreView(currency);
        }
    }
}
