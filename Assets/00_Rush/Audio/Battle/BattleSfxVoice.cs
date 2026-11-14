using UnityEngine;
using UnityEngine.Audio;

namespace Rush
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class BattleSfxVoice : MonoBehaviour
    {
        [SerializeField] private AudioSource m_Source;

        public void Play(AudioClip clip, AudioMixerGroup mixer, float volume, float pitch) =>
            PlayInternal(clip, mixer, volume, pitch);

        private void PlayInternal(AudioClip clip, AudioMixerGroup mixer, float volume, float pitch)
        {
            if (m_Source == null && !TryGetComponent(out m_Source)) return;
            m_Source.clip = clip;
            m_Source.outputAudioMixerGroup = mixer;
            m_Source.volume = volume;
            m_Source.pitch = pitch;
            gameObject.SetActive(true);
            m_Source.Play();
        }

        private void Update()
        {
            if (AudioListener.pause || m_Source == null || m_Source.isPlaying) return;
            gameObject.SetActive(false);
        }
    }
}
