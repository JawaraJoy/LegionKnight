using UnityEngine;

namespace Rush
{
    public class ReviewService : MonoBehaviour
    {
        [SerializeField]
        private GoogleSheetReviewApi m_Api;

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

            if (m_Api == null)
            {
                RushGameManager.Instance
                    .ReviewManager
                    .OnReviewRejected(
                        "Review API is not assigned.");

                return;
            }

            m_Api.SubmitReview(
                request,
                OnReviewSubmitted);
        }

        private void OnReviewSubmitted(ReviewResponse response)
        {
            Debug.Log(
                $"Review Response | Success:{response.Success} | Message:{response.Message}");

            if (response.Success)
            {
                RushGameManager.Instance.ReviewManager.OnReviewSubmitted(response);
            }
            else
            {
                RushGameManager.Instance.ReviewManager.OnReviewRejected(response.Message);
            }
        }
    }
}