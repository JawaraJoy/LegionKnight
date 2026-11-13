using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class ShopBundleGrid : GridLayoutGroup
    {
        [SerializeField] private float m_HeightToWidth = 1.36f;

        public override void CalculateLayoutInputHorizontal()
        {
            int columns = Mathf.Max(1, constraintCount);
            float width = Mathf.Max(1f, (rectTransform.rect.width - padding.horizontal - spacing.x * (columns - 1)) / columns);
            m_CellSize = new Vector2(width, width * m_HeightToWidth);
            base.CalculateLayoutInputHorizontal();
        }
    }
}
