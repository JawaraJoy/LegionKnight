/*using System;
using Firebase.Firestore;
using UnityEngine;

namespace Rush
{
    public class FirestoreReviewApi : MonoBehaviour
    {
        private FirebaseFirestore m_Database;

        private void Awake()
        {
            m_Database = FirebaseFirestore.DefaultInstance;
        }

        public void SubmitReview(
            ReviewRequest request,
            Action<ReviewResponse> callback)
        {
            FirestoreReviewData data =
                new FirestoreReviewData()
                {
                    PlayerName = request.PlayerName,
                    Rating = request.Star,
                    Comment = request.Comment,
                    AppVersion = request.AppVersion,
                    CreatedAt = Timestamp.GetCurrentTimestamp()
                };

            m_Database
                .Collection("reviews")
                .AddAsync(data)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        callback?.Invoke(
                            new ReviewResponse()
                            {
                                Success = false,
                                Message = task.Exception?.Message
                            });

                        return;
                    }

                    callback?.Invoke(
                        new ReviewResponse()
                        {
                            Success = true,
                            Message = "Review submitted"
                        });
                });
        }
    }
}*/