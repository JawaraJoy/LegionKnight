using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace LegionKnight
{
    [System.Serializable]
    public partial class AudioParameter
    {
        [SerializeField]
        private string m_ParameterName;
        [SerializeField]
        private AudioMixerGroup m_MixerGroup;
        private float m_Volume;
        private bool m_IsMuted;
        public string ParameterName => m_ParameterName;
        public void SetVolume(float volume)
        {
            SetVolumeInternal(volume);
        }

        private void SetVolumeInternal(float volume)
        {
            m_Volume = volume;
            m_MixerGroup.audioMixer.SetFloat(m_ParameterName, GetDesible(m_Volume));
        }
        public void SetIsMuted(bool set)
        {
            m_IsMuted = !set;
            if (m_IsMuted)
            {
                m_MixerGroup.audioMixer.SetFloat(m_ParameterName, -80f);
            }
            else
            {
                m_MixerGroup.audioMixer.SetFloat(m_ParameterName, GetDesible(m_Volume));
            }
        }

        private float GetDesible(float linear)
        {
            float dB = Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f)) * 20;
            return dB;
        }
    }
}
