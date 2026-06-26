using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class ReviewStarButton : MonoBehaviour
    {
        [SerializeField]
        private Button m_Button;

        [SerializeField]
        private GameObject m_ActiveStar;

        [SerializeField]
        [Range(1, 5)]
        private int m_StarValue;

        private ReviewPanel m_Panel;

        private void Start()
        {
            m_Panel = CanvasManager.Instance.GetPanel<ReviewPanel>();

            m_Button.onClick.AddListener(SelectStar);
        }

        private void SelectStar()
        {
            foreach (var star in m_Panel.Stars)
            {
                star.SetActive(false);
            }

            for (int i = 0; i < m_StarValue; i++)
            {
                m_Panel.Stars[i].SetActive(true);
            }

            RushGameManager.Instance
                .ReviewManager
                .SetStar(m_StarValue);

            m_Panel.SetSubmitButton(true);
        }

        public void SetActive(bool active)
        {
            m_ActiveStar.SetActive(active);
        }
    }
}