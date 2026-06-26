using UnityEngine;

namespace Rush
{
    public class ReviewService : MonoBehaviour
    {
        [SerializeField]
        private FirestoreReviewApi m_Api;

        public void SubmitReview(
            ReviewRequest request)
        {
            if (!ReviewValidator.Validate(
                request,
                out string error))
            {
                RushGameManager.Instance
                    .ReviewManager
                    .OnReviewRejected(error);

                return;
            }

            m_Api.SubmitReview(
                request,
                OnReviewSubmitted);
        }

        private void OnReviewSubmitted(
            ReviewResponse response)
        {
            if (response.Success)
            {
                RushGameManager.Instance
                    .ReviewManager
                    .OnReviewSubmitted(response);
            }
            else
            {
                RushGameManager.Instance
                    .ReviewManager
                    .OnReviewRejected(response.Message);
            }
        }
    }
}