using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class EnemyWaveIndicatorViewAgent : MonoBehaviour
    {
        private GameplayPanel m_GameplayPanel;
        private NewGameplayPanel m_NewGameplayPanel; // should be removed after old gameplay panel is fully reconstructed
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
        private NewGameplayPanel NewGameplayPanel
        {
            get
            {
                if (m_NewGameplayPanel == null)
                {
                    m_NewGameplayPanel = CanvasManager.Instance.GetPanel<NewGameplayPanel>();
                }
                return m_NewGameplayPanel;
            }
        }
        private WaveIndicatorView m_WaveIndicatorView;
        private WaveIndicatorView WaveIndicatorView
        {
            get
            {
                if (m_WaveIndicatorView == null)
                {
                    if (GameplayPanel == null)
                    {
                        if (NewGameplayPanel.HasBinding(out WaveIndicatorView binded))
                        {
                            m_WaveIndicatorView = binded;
                        }
                        else
                        {
                            m_WaveIndicatorView = null;
                        }
                    }
                    if (NewGameplayPanel == null)
                    {
                        if (GameplayPanel.HasBinding(out WaveIndicatorView binded))
                        {
                            m_WaveIndicatorView = binded;
                        }
                        else
                        {
                            m_WaveIndicatorView = null;
                        }
                    }
                }
                return m_WaveIndicatorView;
            }
        }
        public void SetSlider(int current, int max)
        {
            if (WaveIndicatorView != null)
            {
                WaveIndicatorView.SetSlider(current, max);
            }
        }
        public void SetWaveIcon(Sprite icon)
        {
            if (WaveIndicatorView != null)
            {
                WaveIndicatorView.SetWaveIcon(icon);
            }
        }

        public void Show()
        {
            if (WaveIndicatorView != null)
            {
                WaveIndicatorView.Show();
            }
        }
        public void Hide()
        {
            if (WaveIndicatorView != null)
            {
                WaveIndicatorView.Hide();
            }
        }
    }
}
