using UnityEngine;

namespace LegionKnight
{
    public class PlayerDamageableAgent : MonoBehaviour
    {
        public void TakeDamage(int damage)
        {
            // Implement damage logic here
            Player.Instance.TakeDamage(damage);
            Debug.Log($"Player took {damage} damage.");
        }
    }
}
