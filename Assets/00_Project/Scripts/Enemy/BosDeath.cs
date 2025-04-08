using UnityEngine;

namespace LegionKnight
{
    public partial class BosDeath : MonoBehaviour
    {
       public void EraseBosDamageables()
        {
            // Get all the GameObjects with the BosDamageable component
            BosDamageable[] damageables = FindObjectsByType<BosDamageable>(FindObjectsSortMode.None);
            // Loop through each GameObject and destroy it
            foreach (BosDamageable damageable in damageables)
            {
                Destroy(damageable.gameObject);
            }
        }
    }
}
