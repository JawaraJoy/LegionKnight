using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace Rush
{
    public class VideoController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer m_VideoPlayer;

        [SerializeField]
        private UnityEvent m_OnVideoEnd;

        private void Start()
        {
            m_VideoPlayer.Play();
            m_VideoPlayer.loopPointReached += OnVideoEnd;
        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            m_OnVideoEnd.Invoke();
        }

        public void Pause()
        {
            m_VideoPlayer.Pause();
        }

        public void Resume()
        {
            m_VideoPlayer.Play();
        }

        public void Stop()
        {
            m_VideoPlayer.Stop();
        }
    }
}
