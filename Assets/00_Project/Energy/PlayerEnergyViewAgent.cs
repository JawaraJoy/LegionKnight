using UnityEngine;

namespace LegionKnight
{
    public class PlayerEnergyViewAgent : MonoBehaviour
    {
        public void SetEnergyView(Energy energy)
        {
            GameManager.Instance.SetEnergyView(energy);
        }
    }
}
