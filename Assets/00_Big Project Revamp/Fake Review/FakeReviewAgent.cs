
using UnityEngine;

namespace Rush
{
    public class FakeReviewAgent : MonoBehaviour
    {
        private FakeReviewManager m_Manager;
        private FakeReviewManager Manager
        {
            get
            {
                if (m_Manager == null)
                {
                    m_Manager = RushGameManager.Instance.FakeReview;
                }
                return m_Manager;
            }
        }
        public void Init()
        {
            Manager.Init();
        }

        public void StartReview()
        {
            Manager.StartReview();
        }
    }
}
