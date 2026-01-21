using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class BosSpawnIndicatorView : UIView
    {
        [SerializeField]
        private Image m_IndicatorImage;
        [SerializeField]
        private Slider m_Slider;

        private LevelDefinition m_LevelDefinition;
        private void Start()
        {
            m_LevelDefinition = GameManager.Instance.LevelDefinition;
            Sprite defaultIcon = m_LevelDefinition.BosDefinition?.Icon;
            if (defaultIcon == null)
            {
                HideInternal();
            }
            else
            {
                m_IndicatorImage.sprite = defaultIcon;
            }    
        }
        public void SetSlider(float set)
        {
            m_Slider.value = set;
        }
    }
    public partial class GameplayPanel
    {
        public void SetSlider(float set)
        {
            GetBinding<BosSpawnIndicatorView>().SetSlider(set);
        }

        public void SetActiveBosIndicatorView(bool set)
        {
            BosSpawnIndicatorView view = GetBinding<BosSpawnIndicatorView>();
            if (set)
            {
                view.Show();
            }
            else
            {
                view.Hide();
            }
        }
    }
    public partial class CanvasManager
    {
        public void SetBosIndicator(float set)
        {
            GetPanelInternal<GameplayPanel>().SetSlider(set);
        }
        public void SetActiveBosIndicatorView(bool set)
        {
            GetPanelInternal<GameplayPanel>().SetActiveBosIndicatorView(set);
        }
    }
    public partial class GameplayPanelAgent
    {
        public void SetBosIndicator(float set)
        {
            CanvasManager.Instance.SetBosIndicator(set);
        }
        public void SetActiveBosIndicatorView(bool set)
        {
            CanvasManager.Instance.SetActiveBosIndicatorView(set);
        }
    }
}
