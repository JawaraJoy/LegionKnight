using UnityEngine;

namespace Rush
{
    public static class ReviewValidator
    {
        public static bool Validate(ReviewRequest request, out string error)
        {
            if (request.Star <= 0)
            {
                error = "Please select a rating.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.Comment))
            {
                error = "Review cannot be empty.";
                return false;
            }

            if (request.Comment.Length < 20)
            {
                error = "Review is too short.";
                return false;
            }

            string[] words = request.Comment.Trim().Split(' ');

            if (words.Length < 3)
            {
                error = "Please provide more details.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}