using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class ReviewService : MonoBehaviour
    {
        [SerializeField]
        private FirestoreReviewApi m_Api;

        public void SubmitReview(ReviewRequest request)
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
                OnReviewResponse);
        }

        public void OnReviewApproved()
        {
            m_IsReviewed = true;

            UnityService.Instance.SaveData(
                c_Review,
                true);

            CurrencyManager.Instance.AddCurrency(
                CurrencyType.Diamond,
                200);
        }
    }
}