using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class AudioPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource m_AudioSource;

        protected void PlayInternal(bool loop)
        {
            m_AudioSource.loop = loop;
            m_AudioSource.Play();
        }
        protected void PlayInternal(AudioClip clip, bool loop)
        {
            m_AudioSource.clip = clip;
            m_AudioSource.loop = loop;
            m_AudioSource.Play();
        }
        protected void PlayOneShotInternal(AudioClip clip)
        {
            m_AudioSource.PlayOneShot(clip);
        }

        protected void StopInternal()
        {
            m_AudioSource.Stop();
        }
        public void Play(bool loop)
        {
            PlayInternal(loop);
        }
        public void Play(AudioClip clip, bool loop)
        {
            PlayInternal(clip, loop);
        }
        public void PlayOneShot(AudioClip clip)
        {
            PlayOneShotInternal(clip);
        }

        public void Stop()
        {
            StopInternal();
        }
    }
}
