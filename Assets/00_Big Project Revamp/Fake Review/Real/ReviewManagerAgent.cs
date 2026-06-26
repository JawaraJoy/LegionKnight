using UnityEngine;

namespace Rush
{
    public class ReviewManagerAgent : MonoBehaviour
    {
        public void Init()
        {
            RushGameManager.Instance.ReviewManager.Init();
        }
        public void StartReview()
        {
            RushGameManager.Instance.ReviewManager.StartReview();
        }
    }
}
