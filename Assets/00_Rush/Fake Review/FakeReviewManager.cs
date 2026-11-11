using UnityEngine;

namespace Rush
{
    public class FakeReviewManager : FakeReview
    {
        
    }
    public partial class RushGameManager
    {
        [SerializeField]
        private FakeReviewManager m_FakeReview;
        public FakeReviewManager FakeReview => m_FakeReview;
    }
}
