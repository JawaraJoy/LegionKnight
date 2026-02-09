using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class PotOfLifeAgent : MonoBehaviour
    {
        public void Revive()
        {
            GameManager.Instance.ApplyPotOfLife();
        }
    }
}
