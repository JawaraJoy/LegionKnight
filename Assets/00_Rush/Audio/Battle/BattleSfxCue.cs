using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Rush
{
    [CreateAssetMenu(menuName = "Rush/Audio/Battle SFX Cue")]
    public sealed class BattleSfxCue : ScriptableObject
    {
        [Serializable]
        private sealed class SoundField
        {
            [SerializeField] private AudioClip m_Clip;
            [SerializeField] private AudioMixerGroup m_Mixer;
            [SerializeField, Range(0f, 1f)] private float m_Volume = 0.5f;
            [SerializeField, Range(0.1f, 3f)] private float m_Pitch = 1f;

            public AudioClip Clip => m_Clip;
            public AudioMixerGroup Mixer => m_Mixer;
            public float Volume => m_Volume;
            public float Pitch => m_Pitch;
        }

        [Serializable]
        private sealed class PlaybackField
        {
            [SerializeField] private BattleSfxVoice m_VoicePrefab;
            [SerializeField, Min(0f)] private float m_MinimumInterval = 0.08f;
            [SerializeField, Min(1)] private int m_MaximumVoices = 3;

            public BattleSfxVoice VoicePrefab => m_VoicePrefab;
            public float MinimumInterval => m_MinimumInterval;
            public int MaximumVoices => m_MaximumVoices;
        }

        [SerializeField] private SoundField m_Sound = new();
        [SerializeField] private PlaybackField m_Playback = new();
        [NonSerialized] private double m_LastPlayedAt = double.NegativeInfinity;
        private readonly List<BattleSfxVoice> m_Voices = new();

        public void Play() => PlayInternal();

        private void PlayInternal()
        {
            if (!Application.isPlaying || AudioListener.pause || m_Sound.Clip == null ||
                m_Playback.VoicePrefab == null)
                return;

            double now = Time.unscaledTimeAsDouble;
            if (now < m_LastPlayedAt) m_LastPlayedAt = double.NegativeInfinity;
            if (now - m_LastPlayedAt < m_Playback.MinimumInterval) return;

            BattleSfxVoice available = null;
            for (int i = m_Voices.Count - 1; i >= 0; i--)
            {
                BattleSfxVoice voice = m_Voices[i];
                if (voice == null) m_Voices.RemoveAt(i);
                else if (!voice.gameObject.activeSelf) available = voice;
            }
            if (available == null)
            {
                if (m_Voices.Count >= m_Playback.MaximumVoices) return;
                available = Instantiate(m_Playback.VoicePrefab);
                available.name = name + " Voice";
                m_Voices.Add(available);
            }

            m_LastPlayedAt = now;
            available.Play(m_Sound.Clip, m_Sound.Mixer, m_Sound.Volume, m_Sound.Pitch);
        }
    }
}
