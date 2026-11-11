using System;

namespace Rush
{
    [Serializable]
    public class ReviewResponse
    {
        public bool Success;

        public string Message;

        public string DocumentId;
    }
}