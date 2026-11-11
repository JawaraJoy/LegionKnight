using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class PlatformAlbumGrid : GridLayoutGroup
    {
        [SerializeField] private RectTransform m_Viewport;
        [SerializeField] private int m_VisibleRows = 2;

        public event System.Action CardsChanged;

        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();
            CardsChanged?.Invoke();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            if (m_Viewport != null)
            {
                float width = (rectTransform.rect.width - padding.horizontal - spacing.x * (constraintCount - 1)) / constraintCount;
                float height = (m_Viewport.rect.height - padding.vertical - spacing.y * (m_VisibleRows - 1)) / m_VisibleRows;
                m_CellSize = new Vector2(Mathf.Max(1f, width), Mathf.Max(1f, height));
            }
            base.CalculateLayoutInputHorizontal();
        }
    }
}
