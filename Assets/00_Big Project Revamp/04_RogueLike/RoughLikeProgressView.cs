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
                //Handler.OnForPlayerLevelUp.RemoveListener(SetLevelInternal);
                Handler.OnPlayerLevelChanged.RemoveListener(SetLevelInternal);

                Handler.OnForPlayerExperienceAdded.AddListener(SetFill);
                //Handler.OnForPlayerLevelUp.AddListener(SetLevelInternal);
                Handler.OnPlayerLevelChanged.AddListener(SetLevelInternal);
            }
            else
            {
                Handler.OnForBossExperienceAdded.RemoveListener(SetFill);
                Handler.OnForBossLevelUp.RemoveListener(SetLevelInternal);

                Handler.OnForBossExperienceAdded.AddListener(SetFill);
                Handler.OnForBossLevelUp.AddListener(SetLevelInternal);
            }
        }
        private void SetFill(int current, int max)
        {
            m_FillRate.fillAmount = (float)current / max;
        }
        private void SetLevelInternal(int level)
        {
            m_LevelText.text = $"{level}";
            m_OnLevelChanged.Invoke(level);
        }
        public void SetLevel(int level)
        {
            SetLevelInternal(level);
        }
    }    
}
