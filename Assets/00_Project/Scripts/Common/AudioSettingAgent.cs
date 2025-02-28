using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class AudioSettingAgent : MonoBehaviour
    {
        [SerializeField]
        private string m_ParameterName;

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
        
    }
}
