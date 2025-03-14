using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class BosSpawnIndicatorView : UIView
    {
        [SerializeField]
        private Slider m_Slider;

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
    }
    public partial class GameManager
    {
        public void SetBosIndicator(float set)
        {
            GetPanelInternal<GameplayPanel>().SetSlider(set);
        }
    }
    public partial class GameplayPanelAgent
    {
        public void SetBosIndicator(float set)
        {
            GameManager.Instance.SetBosIndicator(set);
        }
    }
}
