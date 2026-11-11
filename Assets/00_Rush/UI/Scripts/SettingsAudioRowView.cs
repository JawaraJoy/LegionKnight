using TMPro;
using UnityEngine;

namespace Rush
{
    [DisallowMultipleComponent]
    public sealed class SettingsAudioRowView : MonoBehaviour
    {
        private const float c_PercentScale = 100f;
        private const string c_PercentFormat = "{0}%";

        [SerializeField] private TMP_Text m_VolumeLabel;
        [SerializeField] private GameObject m_OnVisual;
        [SerializeField] private GameObject m_OffVisual;

        public void SetVolumeDisplay(float volume)
        {
            SetVolumeDisplayInternal(volume);
        }

        public void SetSwitchDisplay(bool isOn)
        {
            SetSwitchDisplayInternal(isOn);
        }

        private void SetVolumeDisplayInternal(float volume)
        {
            int percentage = Mathf.RoundToInt(Mathf.Clamp01(volume) * c_PercentScale);
            m_VolumeLabel.SetText(c_PercentFormat, percentage);
        }

        private void SetSwitchDisplayInternal(bool isOn)
        {
            m_OnVisual.SetActive(isOn);
            m_OffVisual.SetActive(!isOn);
        }
    }
}
