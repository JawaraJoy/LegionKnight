using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class FakeReview : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private int m_SelectedStar;

        private bool m_IsReviewed = false;

        [SerializeField]
        private UnityEvent<int> m_OnStarSelected;

        [SerializeField]
        private Currency m_Reward;

        private const string c_Review = "fakereview";

        private FakeReviewPanel m_Panel;
        private FakeReviewPanel Panel
        {
            get
            {
                if (m_Panel == null)
                {
                    m_Panel = CanvasManager.Instance.GetPanel<FakeReviewPanel>();
                }
                return m_Panel;
            }
        }
        public void Init()
        {
            bool hasReviewed = UnityService.Instance.HasData(c_Review);
            if (hasReviewed)
            {
                m_IsReviewed = UnityService.Instance.GetData<bool>(c_Review);
            }
        }

        public void StartReview()
        {
            if (m_IsReviewed == false)
            {
                Panel.OpenReview();
            }
        }

        public void SetStar(int star)
        {
            m_SelectedStar = star;
            m_OnStarSelected?.Invoke(m_SelectedStar);
        }
        public void ApplyReview(string desc)
        {
            string playerName = Player.Instance.PlayerName;
            string review = $"{playerName}<Star({m_SelectedStar}>; {desc})";

            TenjinManager.Instance.SendEvent("review", review);
            m_IsReviewed = true;
            UnityService.Instance.SaveData(c_Review, m_IsReviewed);
        }
    }
}
