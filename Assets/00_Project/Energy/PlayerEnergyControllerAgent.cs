using UnityEngine;

namespace LegionKnight
{
    public class PlayerEnergyControllerAgent : MonoBehaviour
    {
        public void ClearPreviousCost()
        {
            Player.Instance.ClearPreviousEnergyCost();
        }
    }
}
