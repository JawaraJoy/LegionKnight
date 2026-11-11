using UnityEngine;

namespace Rush
{
    public class UnitConfigExtension_DefaultAudio
    {

    }
    [System.Serializable] 
    public class DefaultAudioField
    {
        [SerializeField]
        private AudioClip m_Entrance;
        public AudioClip Entrance => m_Entrance;
    }

    public partial class UnitConfig
    {
        [SerializeField]
        private DefaultAudioField m_DefaultAudio;
        public DefaultAudioField DefaultAudio => m_DefaultAudio;
    }
}
