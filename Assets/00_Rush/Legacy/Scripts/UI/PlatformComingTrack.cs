using UnityEngine;

namespace LegionKnight
{
    public class PlatformComingTrack : UIView
    {
        [SerializeField]
        private GameObject m_RightTrack;
        [SerializeField]
        private GameObject m_LeftTrack;

        public void ShowRightTrack()
        {
            ShowInternal();
            m_RightTrack.SetActive(true);
            m_LeftTrack.SetActive(false);
        }
        public void ShowLeftTrack()
        {
            ShowInternal();
            m_RightTrack.SetActive(false);
            m_LeftTrack.SetActive(true);
        }
    }
}
