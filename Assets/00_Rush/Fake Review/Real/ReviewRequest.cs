using System;

namespace Rush
{
    [Serializable]
    public class ReviewRequest
    {
        public string UserId;
        public string UserName;

        public int Rating;

        public string Comment;

        public string AppVersion;

        public string Platform;
    }
}