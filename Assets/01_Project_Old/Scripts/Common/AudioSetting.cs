using UnityEngine;
using LegionKnight;

namespace Rush
{
    public partial class AudioSetting : AudioHandler
    {
        
    }

    public partial class GameSetting
    {
        [SerializeField]
        private AudioSetting m_AudioSetting;
        public AudioSetting AudioSetting => m_AudioSetting;
        
    }
}
