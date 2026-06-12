using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class PlayerExpView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_ExpText;

        private PlayerScore m_PlayerScore;

        [SerializeField]
        private UnityEvent<int> m_OnExpFromHalvedScoreChanged;

        private void Awake()
        {
            m_PlayerScore = RushPlayer.Instance.PlayerScore;

            //m_PlayerScore.OnExpFromHalvedScoreChanged.AddListener(SetExp);

        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            int score = m_PlayerScore.ExpFromHalvedScore;
            SetExp(score);
        }

        private void SetExp(int exp)
        {
            m_ExpText.text = exp.ToString();
            m_OnExpFromHalvedScoreChanged.Invoke(exp);
        }
    }
}
