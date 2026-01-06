using UnityEngine;

namespace LegionKnight
{
    public class PlayerCoinComboText : TextMeshSpawner
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerCoinComboText m_CoinComboText;

        public void SpawnText(int val)
        {
            m_CoinComboText.SpawnText(val);
        }
    }
}
