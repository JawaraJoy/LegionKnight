using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    // Scalable beveled panels and hanging podium banners, drawn in a single UI mesh.
    public sealed class LeaderboardPlaque : MaskableGraphic
    {
        [Serializable]
        private sealed class FrameField
        {
            [SerializeField] private Color m_Trim = Color.white;
            [SerializeField] private Color m_Base = Color.black;
            [SerializeField, Min(0)] private float m_Corner = 32f;
            [SerializeField, Min(0)] private float m_Stroke = 4f;
            [SerializeField] private bool m_Pennant;

            public Color Trim => m_Trim;
            public Color Base => m_Base;
            public float Corner => m_Corner;
            public float Stroke => m_Stroke;
            public bool Pennant => m_Pennant;
        }

        [SerializeField] private FrameField m_Frame = new();
        private const int c_Points = 8;
        private const float c_InnerLineInset = 3.5f;
        private const float c_InnerLineOpacity = 0.45f;
        private const float c_HemSlope = 0.28f;

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect rect = rectTransform.rect;
            if (rect.width <= 0f || rect.height <= 0f) return;
            float stroke = Mathf.Min(m_Frame.Stroke, Mathf.Min(rect.width, rect.height) * 0.04f);
            AddRingInternal(mesh, rect, 0f, m_Frame.Trim);
            AddRingInternal(mesh, rect, stroke, color);
            AddRingInternal(mesh, rect, stroke * c_InnerLineInset, color);
            AddRingInternal(mesh, rect, stroke * (c_InnerLineInset + 1f), color);
            for (int i = 0; i < c_Points; i++)
            {
                int next = (i + 1) % c_Points;
                for (int ring = 0; ring < 3; ring++)
                {
                    int first = ring * c_Points;
                    mesh.AddTriangle(first + i, first + next, first + next + c_Points);
                    mesh.AddTriangle(first + i, first + next + c_Points, first + i + c_Points);
                }
            }
            int inside = 3 * c_Points;
            for (int i = 1; i < c_Points - 1; i++) mesh.AddTriangle(inside, inside + i, inside + i + 1);
        }

        private void AddRingInternal(VertexHelper mesh, Rect rect, float inset, Color tint)
        {
            float x0 = rect.xMin + inset, x1 = rect.xMax - inset;
            float y0 = rect.yMin + inset, y1 = rect.yMax - inset;
            float corner = Mathf.Clamp(m_Frame.Corner - inset, 0f, Mathf.Min(x1-x0, y1-y0) * 0.5f);
            float mid = rect.center.x;
            float hem = m_Frame.Pennant ? corner : 0f;
            Vector2[] points = m_Frame.Pennant
                ? new[] { new Vector2(x0,y1), new Vector2(x1,y1), new Vector2(x1,y0+hem), new Vector2(x1-corner*c_HemSlope,y0+hem*c_HemSlope*2f),
                    new Vector2(mid,y0), new Vector2(x0+corner*c_HemSlope,y0+hem*c_HemSlope*2f), new Vector2(x0,y0+hem), new Vector2(x0,rect.center.y) }
                : new[] { new Vector2(x0+corner,y1), new Vector2(x1-corner,y1), new Vector2(x1,y1-corner), new Vector2(x1,y0+corner),
                    new Vector2(x1-corner,y0), new Vector2(x0+corner,y0), new Vector2(x0,y0+corner), new Vector2(x0,y1-corner) };
            int ring = mesh.currentVertCount / c_Points;
            foreach (var point in points)
            {
                var fill = Color.Lerp(m_Frame.Base, tint, Mathf.InverseLerp(rect.yMin, rect.yMax, point.y));
                if (ring == 0) fill = tint;
                if (ring == 2) fill = Color.Lerp(fill, m_Frame.Trim, c_InnerLineOpacity);
                mesh.AddVert(point, fill, Vector2.zero);
            }
        }
    }
}
