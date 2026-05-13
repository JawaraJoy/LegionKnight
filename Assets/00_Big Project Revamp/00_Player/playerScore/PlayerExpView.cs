using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class PlayerExpView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_ExpText;

        private PlayerScore m_PlayerScore;

        [SerializeField]
        private UnityEvent<int> m_OnExpFromHalvedScoreChanged;

        private void Start()
        {
            m_PlayerScore = RushPlayer.Instance.PlayerScore;

            m_PlayerScore.OnExpFromHalvedScoreChanged.AddListener(SetExp);
        }

        private void SetExp(int exp)
        {
            m_OnExpFromHalvedScoreChanged.Invoke(exp);
        }
    }
}
