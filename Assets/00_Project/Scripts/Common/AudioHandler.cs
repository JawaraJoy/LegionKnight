using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace LegionKnight
{
    public partial class AudioHandler : MonoBehaviour
    {
        [SerializeField]
        private List<AudioParameter> m_AudioParameters = new();

        protected virtual void Start()
        {
            foreach(AudioParameter parameter in m_AudioParameters)
            {
                parameter.Init();
            }
        }
        public bool GetIsMuted(string parameterName)
        {
            return GetParameter(parameterName).IsMuted;
        }
        public float GetVolume(string parameterName)
        {
            return GetParameter(parameterName).Volume;
        }
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
