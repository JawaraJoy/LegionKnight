using UnityEngine;

namespace LegionKnight
{
    public class PlayerCoinComboAgent : MonoBehaviour
    {
        public void SpawnText(int val)
        {
            Player.Instance.SpawnText(val);
        }
    }
}
