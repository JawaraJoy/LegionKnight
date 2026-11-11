using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class HomeHeaderLayout : LayoutGroup
    {
        [Serializable]
        private struct SlotField
        {
            [SerializeField] private RectTransform m_Target;
            [SerializeField] private Rect m_Bounds;

            public RectTransform Target => m_Target;
            public Rect Bounds => m_Bounds;
        }

        private const float c_DesignWidth = 1080f;
        [SerializeField] private SlotField[] m_Slots = Array.Empty<SlotField>();

        public override void CalculateLayoutInputVertical() { }
        public override void SetLayoutHorizontal() => ApplyAxisInternal(0);
        public override void SetLayoutVertical() => ApplyAxisInternal(1);

        private void ApplyAxisInternal(int axis)
        {
            float scale = rectTransform.rect.width / c_DesignWidth;
            foreach (var slot in m_Slots)
            {
                SetChildAlongAxis(slot.Target, axis, slot.Bounds.position[axis] * scale,
                    slot.Bounds.size[axis] * scale);
            }
        }
    }
}
