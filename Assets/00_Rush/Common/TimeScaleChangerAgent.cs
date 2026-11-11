using UnityEngine;

namespace Rush
{
    public class TimeScaleChangerAgent : MonoBehaviour
    {
        public void SetTimeScale(float newTimeScale)
        {
            Time.timeScale = newTimeScale;
        }
    }
}
