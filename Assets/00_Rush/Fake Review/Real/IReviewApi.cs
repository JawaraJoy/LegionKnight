using System;

namespace Rush
{
    public interface IReviewApi
    {
        void SubmitReview(
            ReviewRequest request,
            Action<ReviewResponse> callback);
    }
}