using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    [ExecuteAlways, RequireComponent(typeof(GridLayoutGroup))]
    public sealed class ProfileBadgeGrid : MonoBehaviour
    {
        [SerializeField] private float m_HeightToWidth = .32f;
        private void OnEnable() => Fit();
        private void OnRectTransformDimensionsChange() => Fit();
        private void Fit()
        {
            var grid = GetComponent<GridLayoutGroup>();
            float width = ((RectTransform)transform).rect.width - grid.padding.horizontal;
            if (width <= grid.spacing.x) return;
            float cell = (width - grid.spacing.x) / 2f;
            var size = new Vector2(cell, cell * m_HeightToWidth);
            if ((grid.cellSize - size).sqrMagnitude > .01f) grid.cellSize = size;
        }
    }
}
