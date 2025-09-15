using UnityEngine;

namespace LegionKnight
{
    public class PlayerRegentAgent : MonoBehaviour
    {
        public void Heal(int amount)
        {
            Player.Instance.Heal(amount);
        }
    }
}
