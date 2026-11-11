using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class AllainFeedbackEffect : MonoBehaviour
    {
        [SerializeField] private float m_Lifetime = 1f;
        [SerializeField] private AudioSource m_Audio;
        [SerializeField] private float m_SoundInterval = 0.08f;
        private static readonly Dictionary<AudioClip, float> s_LastSoundTimes = new();

        public void Play() => PlayInternal();
        private void PlayInternal()
        {
            foreach (var particle in GetComponentsInChildren<ParticleSystem>()) particle.Play(false);
            if (m_Audio != null && m_Audio.clip != null)
            {
                float now = Time.unscaledTime;
                bool allowed = !s_LastSoundTimes.TryGetValue(m_Audio.clip, out float last) ||
                    now < last || now - last >= m_SoundInterval;
                if (allowed)
                {
                    s_LastSoundTimes[m_Audio.clip] = now;
                    m_Audio.Play();
                }
            }
            Destroy(gameObject, m_Lifetime);
        }
    }
}
