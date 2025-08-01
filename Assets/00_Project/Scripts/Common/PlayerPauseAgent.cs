using UnityEngine;

namespace LegionKnight
{
    public class PlayerPauseAgent : MonoBehaviour
    {
        public void SetPause(bool set)
        {
            Player.Instance.SetPause(set);
        }
    }
}
