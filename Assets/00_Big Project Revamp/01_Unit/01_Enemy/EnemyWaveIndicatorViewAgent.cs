using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class EnemyWaveIndicatorViewAgent : MonoBehaviour
    {
        private GameplayPanel m_GameplayPanel;
        private GameplayPanel GameplayPanel
        {
            get
            {
                if (m_GameplayPanel == null)
                {
                    m_GameplayPanel = CanvasManager.Instance.GetPanel<GameplayPanel>();
                }
                return m_GameplayPanel;
            }
        }
        private WaveIndicatorView m_WaveIndicatorView;
        private WaveIndicatorView WaveIndicatorView
        {
            get
            {
                if (m_WaveIndicatorView == null)
                {
                    m_WaveIndicatorView = GameplayPanel.GetBinding<WaveIndicatorView>();
                }
                return m_WaveIndicatorView;
            }
        }
        public void SetSlider(int current, int max)
        {
            WaveIndicatorView.SetSlider(current, max);
        }
        public void SetWaveIcon(Sprite icon)
        {
            WaveIndicatorView.SetWaveIcon(icon);
        }

        public void Show()
        {
            WaveIndicatorView.Show();
        }
        public void Hide()
        {
            WaveIndicatorView.Hide();
        }
    }
}
