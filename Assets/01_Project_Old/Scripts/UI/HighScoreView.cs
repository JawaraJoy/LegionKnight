using UnityEngine;

namespace LegionKnight
{
    public partial class HighScoreView : CurrencyView
    {
        private void OnEnable()
        {
            
        }
        protected override void ShowInternal()
        {
            Currency playerHighScore = Player.Instance.CurrencyControl.GetCurrency(m_ItemConfig);
            SetViewInternal(playerHighScore);
        }
    }
}
