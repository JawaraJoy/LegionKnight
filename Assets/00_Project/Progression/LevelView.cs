using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class LevelView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_NameText;
        [SerializeField]
        private TextMeshProUGUI m_LevelText;
        [SerializeField]
        private TextMeshProUGUI m_ExpText;
        [SerializeField]
        private Slider m_ExpSlider;

        public void Init()
        {
            float maxExp = Player.Instance.GetPlayerMaxExp();
            float currentExp = Player.Instance.GetPlayerCurrentExp();
            m_ExpText.text = $"{currentExp}/{maxExp}";
            m_ExpSlider.value = Player.Instance.GetPlayerLevelProgressionRate();
            m_LevelText.text = $"LV: {Player.Instance.GetPlayerLevel()}";
            m_NameText.text = Player.Instance.PlayerName;
            //m_LevelText.text = Player.Instance.GetLevel().ToString();
            //m_ExpSlider.value = Player.Instance.GetExp() / Player.Instance.GetMaxExp();
        }
    }

    public partial class HomePanel
    {
        private LevelView GetLevelView()
        {
            return GetBinding<LevelView>();
        }

        public void InitLevelView()
        {
            var levelView = GetLevelView();
            if (levelView != null)
            {
                levelView.Init();
            }
        }
    }

    public partial class GameManager
    {
        public void InitLevelView()
        {
            HomePanel homePanel = GetPanelInternal<HomePanel>();
            if (homePanel != null)
            {
                homePanel.InitLevelView();
            }
        }
    }
}
