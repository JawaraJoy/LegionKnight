using LegionKnight;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class FakeStarReviewButton : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_ActiveStar;
        [SerializeField]
        private Button m_StarButton;

        [SerializeField, Range(1, 5)]
        private int m_StarPower = 1;

        private FakeReviewPanel m_Panel;
        private void Start()
        {
            m_Panel = CanvasManager.Instance.GetPanel<FakeReviewPanel>();
            m_StarButton.onClick.AddListener(SetStar);
        }
        private void SetStar()
        {
            for (int i = 0; i < m_StarPower; i++)
            {
                m_Panel.StarReviewButtons[i].ActivateStar(true);
            }
            RushGameManager.Instance.FakeReview.SetStar(m_StarPower);
            m_Panel.SetConfirmationButton(true);
        }

        public void ActivateStar(bool set)
        {
            m_ActiveStar.SetActive(set);
        }
    }
}
