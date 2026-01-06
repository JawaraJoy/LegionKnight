using UnityEngine;

namespace LegionKnight
{
    public partial class ComboViewAgent : MonoBehaviour
    {
        public void SpawnComboNumberUI(int combo)
        {
            CanvasManager.Instance.SpawnComboNumberUI(combo);
        }
    }
}
