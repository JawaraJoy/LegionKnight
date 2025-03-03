using UnityEngine;

namespace LegionKnight
{
    public partial class HomeHighScoreViewAgent : MonoBehaviour
    {
        public void SetHighScoreView(Currency currency)
        {
            GameManager.Instance.SetHomeHighScoreView(currency);
        }
    }
}
