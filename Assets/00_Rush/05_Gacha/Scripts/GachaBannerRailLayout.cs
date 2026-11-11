using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class GachaBannerRailLayout : GridLayoutGroup
    {
        private const int c_VisibleBanners = 4;
        [SerializeField] private RectTransform m_Viewport;

        public override void CalculateLayoutInputHorizontal()
        {
            if (m_Viewport != null)
            {
                Rect area = m_Viewport.rect;
                float gaps = spacing.y * (c_VisibleBanners - 1);
                m_CellSize = new Vector2(
                    Mathf.Max(0f, area.width - padding.horizontal),
                    Mathf.Max(0f, (area.height - padding.vertical - gaps) / c_VisibleBanners));
            }
            base.CalculateLayoutInputHorizontal();
        }
    }
}
