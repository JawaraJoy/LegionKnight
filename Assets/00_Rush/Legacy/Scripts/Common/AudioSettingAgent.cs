using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Rush;

namespace LegionKnight
{
    public partial class AudioSettingAgent : MonoBehaviour
    {
        [SerializeField]
        private string m_ParameterName;
        [SerializeField]
        private UnityEvent<float> m_OnEnableVolume;
        [SerializeField]
        private UnityEvent<bool> m_OnEnableMuted;
        private void OnEnable()
        {
            OnEnableInvoke();
        }
        public void SetVolume(float volume)
        {
            SetVolumeInternal(volume);
        }
        private void SetVolumeInternal(float volume)
        {
            GameSetting.Instance.AudioSetting.SetVolume(m_ParameterName, volume);
        }

        public void SetIsMute(bool enable)
        {
            GameSetting.Instance.AudioSetting.SetIsMute(m_ParameterName, enable);
        }
        private void OnEnableInvoke()
        {
            float volume = GameSetting.Instance.AudioSetting.GetVolume(m_ParameterName);
            bool unMuted = !GameSetting.Instance.AudioSetting.GetIsMuted(m_ParameterName);

            m_OnEnableVolume?.Invoke(volume);
            m_OnEnableMuted?.Invoke(unMuted);
        }

    }
}
