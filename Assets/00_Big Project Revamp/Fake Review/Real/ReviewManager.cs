using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class ReviewManager : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private int m_SelectedStar;

        [SerializeField]
        private ReviewService m_ReviewService;

        private bool m_IsReviewed;

        private const string c_Review = "reviewed";

        private ReviewPanel m_Panel;

        private ReviewPanel Panel
        {
            get
            {
                if (m_Panel == null)
                {
                    m_Panel =
                        CanvasManager.Instance
                        .GetPanel<ReviewPanel>();
                }

                return m_Panel;
            }
        }

        public void Init()
        {
            bool hasReview =
                UnityService.Instance.HasData(c_Review);

            if (hasReview)
            {
                m_IsReviewed =
                    UnityService.Instance.GetData<bool>(c_Review);
            }
        }

        public void StartReview()
        {
            if (m_IsReviewed)
                return;

            Panel.OpenReview();
        }

        public void SetStar(int star)
        {
            m_SelectedStar = star;
        }

        public void SubmitReview(string comment)
        {
            ReviewRequest request = new ReviewRequest()
            {
                UserId = UnityService.Instance.PlayerId,
                UserName = Player.Instance.PlayerName,

                Rating = m_SelectedStar,

                Comment = comment,

                AppVersion = Application.version,

                Platform = Application.platform.ToString()
            };

            m_ReviewService.SubmitReview(request);
        }

        public void OnReviewApproved()
        {
            m_IsReviewed = true;

            UnityService.Instance.SaveData(c_Review, true);

            //CurrencyManager.Instance.AddCurrency(CurrencyType.Diamond,200);
        }

        public void OnReviewRejected(string reason)
        {
            Debug.Log($"Review rejected : {reason}");
        }
    }

    public partial class RushGameManager
    {
        [SerializeField]
        private ReviewManager m_ReviewManager;

        public ReviewManager ReviewManager
            => m_ReviewManager;
    }

}