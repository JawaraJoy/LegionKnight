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
        public void SetVolume(string volumeName, float volume)
        {
            m_AudioSetting.SetVolume(volumeName, volume);
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
        
    }
}
