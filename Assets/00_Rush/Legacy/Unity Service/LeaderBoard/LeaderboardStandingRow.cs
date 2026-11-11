using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    [RequireComponent(typeof(Image))]
    public sealed class LeaderboardStandingRow : RankScoreView
    {
        [SerializeField] private Color m_LightBand;
        [SerializeField] private Color m_DarkBand;

        protected override void ShowInternal()
        {
            GetComponent<Image>().color = (transform.GetSiblingIndex() % 2 == 1) ? m_LightBand : m_DarkBand;
            base.ShowInternal();
        }
    }
}
