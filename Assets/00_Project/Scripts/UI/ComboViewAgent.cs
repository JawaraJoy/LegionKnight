using UnityEngine;

namespace LegionKnight
{
    public partial class ComboViewAgent : MonoBehaviour
    {
        public void SpawnComboNumberUI(int combo)
        {
            GameManager.Instance.SpawnComboNumberUI(combo);
        }
    }
}
