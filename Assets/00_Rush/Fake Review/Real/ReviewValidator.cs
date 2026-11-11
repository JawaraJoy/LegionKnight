using UnityEngine;

namespace Rush
{
    public static class ReviewValidator
    {
        private static int m_MinLenghtLetters = 20;
        private static int m_MinWordsCount = 3;

        public static int MinLenghtLetters => m_MinLenghtLetters;
        public static int MinWordsCount => m_MinWordsCount;
        public static bool Validate(ReviewRequest request, out string error)
        {
            if (request.Rating <= 0)
            {
                error = "Please select a rating.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.Comment))
            {
                error = "Review cannot be empty.";
                return false;
            }

            if (request.Comment.Length < m_MinLenghtLetters)
            {
                error = $"Review is too short. Min {m_MinLenghtLetters} characters";
                return false;
            }

            string[] words = request.Comment.Trim().Split(' ');

            if (words.Length < m_MinWordsCount)
            {
                error = $"Please provide more than {2} words.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}