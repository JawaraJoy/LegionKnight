using UnityEngine;
using UnityEngine.Audio;

namespace LegionKnight
{
    public partial class AudioSetting : AudioHandler
    {
        
    }

    public partial class GameSetting
    {
        [SerializeField]
        private AudioSetting m_AudioSetting;
        public bool GetIsMuted(string parameterName) => m_AudioSetting.GetIsMuted(parameterName);
        public float GetVolume(string parameterName) => m_AudioSetting.GetVolume(parameterName);
        public void SetVolume(string parameterName, float volume)
        {
            m_AudioSetting.SetVolume(parameterName, volume);
        }
        public void SetIsMute(string parameterName, bool set)
        {
            m_AudioSetting.SetIsMute(parameterName, set);
        }
        public void PlayBGM()
        {
            m_AudioSetting.PlayBGM();
        }
        public void PlayBGM(AudioClip clip)
        {
            m_AudioSetting.PlayBGM(clip);
        }
        public void StopBGM()
        {
            m_AudioSetting.StopBGM();
        }
    }
}
