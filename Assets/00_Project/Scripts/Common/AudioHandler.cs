using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace LegionKnight
{
    public partial class AudioHandler : MonoBehaviour
    {
        [SerializeField]
        private List<AudioParameter> m_AudioParameters = new();
        private AudioParameter GetParameter(string parameterName)
        {
            AudioParameter match = m_AudioParameters.Find(x => x.ParameterName == parameterName);
            return match;
        }
        public void SetVolume(string paramterName, float volume)
        {
            GetParameter(paramterName).SetVolume(volume);
        }
        public void SetIsMute(string parameterName, bool set)
        {
            GetParameter(parameterName).SetIsMuted(set);
        }
    }
}
