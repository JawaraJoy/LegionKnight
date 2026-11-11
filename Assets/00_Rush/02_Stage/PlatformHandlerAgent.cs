using UnityEngine;

namespace Rush
{
    public class PlatformHandlerAgent : MonoBehaviour
    {
        private PlatformHandler m_PlatformHandler;

        private PlatformHandler PlatformHandler
        {
            get
            {
                if (m_PlatformHandler == null)
                {
                    m_PlatformHandler = RushGameManager.Instance.StageManager.PlatformHandler;
                }
                return m_PlatformHandler;
            }
        }

        public void Play()
        {
            PlatformHandler.Play();
        }
        public void Pause()
        {
            PlatformHandler.Pause();
        }
        public void Resume()
        {
            PlatformHandler.Resume();
        }
    }
}
