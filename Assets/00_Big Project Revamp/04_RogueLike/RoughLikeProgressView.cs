using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class RoughLikeProgressView : UIView
    {
        private RogueLikeManager m_Handler;

        private RogueLikeManager Handler
        {
            get
            {
                if (m_Handler == null)
                {
                    m_Handler = RushGameManager.Instance.RogueLikeManager;
                }
                return m_Handler;
            }
        }
        [SerializeField]
        private Image m_FillRate;
        [SerializeField]
        private TextMeshProUGUI m_LevelText;

        private void Start()
        {
            Handler.OnExperienceAdded.RemoveAllListeners();
            Handler.OnLevelUp.RemoveAllListeners();

            Handler.OnExperienceAdded.AddListener(SetFill);
            Handler.OnLevelUp.AddListener(SetLevel);
        }
        private void SetFill(int current, int max)
        {
            m_FillRate.fillAmount = (float)current / max;
        }
        private void SetLevel(int level)
        {
            m_LevelText.text = $"{level}";
        }
    }
}
