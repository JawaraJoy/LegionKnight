using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

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
            GameSetting.Instance.SetVolume(m_ParameterName, volume);
        }

        public void SetIsMute(bool enable)
        {
            GameSetting.Instance.SetIsMute(m_ParameterName, enable);
        }
        private void OnEnableInvoke()
        {
            float volume = GameSetting.Instance.GetVolume(m_ParameterName);
            bool unMuted = !GameSetting.Instance.GetIsMuted(m_ParameterName);

            m_OnEnableVolume?.Invoke(volume);
            m_OnEnableMuted?.Invoke(unMuted);
        }

    }
}
