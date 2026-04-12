using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class PlayerTimerAgent : MonoBehaviour
    {
        public void InitTimer()
        {
            Player.Instance.TimerManager.Init();
        }
    }
}
