using Rush;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class SpawnIndicatorView : UIView
    {
        [SerializeField]
        private Image m_IndicatorImage;
        [SerializeField]
        private Slider m_Slider;

        private EnemyWaveHandler m_EnemyWaveHandler;
        private EnemyWaveHandler EnemyWaveHandler
        {
            get
            {
                if (m_EnemyWaveHandler == null)
                {
                    m_EnemyWaveHandler = RushGameManager.Instance.StageManager.EnemyWaveHandler;
                }
                return m_EnemyWaveHandler;
            }
        }
        private UnityEvent<float> m_OnThresholdRateChanged = new();
        private UnityEvent<Sprite> m_OnWaveIconChanged = new();
        private void Start()
        {
            m_OnThresholdRateChanged = EnemyWaveHandler.OnCurrentThresholdRateChanged;
            m_OnWaveIconChanged = EnemyWaveHandler.OnWaveIconChanged;
        }
        private void OnEnable()
        {
            m_OnThresholdRateChanged.AddListener(SetSlider);
            m_OnWaveIconChanged.AddListener(SetIndicatorImage);
        }
        private void OnDisable()
        {
            m_OnThresholdRateChanged.RemoveListener(SetSlider);
        }
        private void SetSlider(float set)
        {
            m_Slider.value = set;
        }
        private void SetIndicatorImage(Sprite icon)
        {
            m_IndicatorImage.sprite = icon;
        }
    }
    
}
