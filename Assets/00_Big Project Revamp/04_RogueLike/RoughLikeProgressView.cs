using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush
{
    public class RoughLikeProgressView : UIView
    {
        [SerializeField]
        private RogueLikeForProgressType m_For = RogueLikeForProgressType.Player;
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
        [SerializeField]
        private UnityEvent<int> m_OnLevelChanged;

        private void Start()
        {
            if (m_For == RogueLikeForProgressType.Player)
            {
                Handler.OnForPlayerExperienceAdded.RemoveListener(SetFill);
                Handler.OnForPlayerLevelUp.RemoveListener(SetLevel);

                Handler.OnForPlayerExperienceAdded.AddListener(SetFill);
                Handler.OnForPlayerLevelUp.AddListener(SetLevel);
            }
            else
            {
                Handler.OnForBossExperienceAdded.RemoveListener(SetFill);
                Handler.OnForBossLevelUp.RemoveListener(SetLevel);

                Handler.OnForBossExperienceAdded.AddListener(SetFill);
                Handler.OnForBossLevelUp.AddListener(SetLevel);
            }
        }
        private void SetFill(int current, int max)
        {
            m_FillRate.fillAmount = (float)current / max;
        }
        private void SetLevel(int level)
        {
            m_LevelText.text = $"{level}";
            m_OnLevelChanged.Invoke(level);
        }
    }

    
}
