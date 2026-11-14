using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class WaveIndicatorView : SliderView
    {
        [SerializeField]
        private Image m_WaveIcon;

        [SerializeField] private TMPro.TMP_Text m_WaveNumber;
        [SerializeField] private Image m_Progress;
        private int m_LastNumber = -1;
        public bool UsesBattleHud => m_WaveNumber != null;
        private void LateUpdate()
        {
            if (!UsesBattleHud) return;
            var game = RushGameManager.Instance;
            if (game == null || game.StageManager == null) return;
            int number = game.StageManager.BattleWaveNumber;
            if (number == m_LastNumber) return;
            m_LastNumber = number;
            m_WaveNumber.text = $"Wave {number:00}";
        }
        protected override void SetSliderInternal(int current, int max)
        {
            if (!UsesBattleHud) { base.SetSliderInternal(current,max); return; }
            m_Rate = max > 0 ? Mathf.Clamp01((float)current/max) : 0f;
            m_Progress.fillAmount = m_Rate;
        }
        public void SetWaveIcon(Sprite icon)
        {
            if (m_WaveIcon == null) return;
            m_WaveIcon.sprite = icon;
            if (UsesBattleHud) m_WaveIcon.enabled = icon != null;
        }
    }
}
