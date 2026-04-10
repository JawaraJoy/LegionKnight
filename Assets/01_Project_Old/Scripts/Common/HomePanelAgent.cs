using UnityEngine;

namespace LegionKnight
{
    public partial class HomePanelAgent : MonoBehaviour
    {
        public void SetCurrencyView(Currency currency)
        {
            CanvasManager.Instance.SetHomeCurrencyView(currency);
        }
        public void SetHighScoreView(Currency currency)
        {
            //CanvasManager.Instance.SetHomeHighScoreView(currency);
        }
        
    }
}
