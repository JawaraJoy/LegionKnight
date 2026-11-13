using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class SpinWheelFace : MaskableGraphic
    {
        [SerializeField, Min(1)] private int m_SectorCount = 8;
        private const int c_StepsPerSector = 12;
        private static readonly Color32 s_Ivory = new Color32(240, 228, 202, 255);
        private static readonly Color32 s_Indigo = new Color32(60, 64, 92, 255);
        private static readonly Color32 s_Ink = new Color32(45, 48, 64, 255);
        private static readonly Color32 s_Brass = new Color32(183, 157, 116, 255);
        private static readonly Color32 s_Light = new Color32(223, 205, 164, 255);
        private const float c_FullTurn = 360f;
        private const float c_SpokeWidth = .42f;
        private const float c_OuterEdge = .969f;
        private const float c_Engraving = .900f;
        private const float c_Hub = .273f;

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            var rect = rectTransform.rect;
            float radius = Mathf.Min(rect.width, rect.height) * .5f;
            if (radius <= 0f || m_SectorCount < 1) return;
            float sector = c_FullTurn / m_SectorCount;
            for (int i = 0; i < m_SectorCount; i++)
            {
                float start = (i - .5f) * sector;
                var face = i % 2 == 0 ? s_Indigo : s_Ivory;
                for (int j = 0; j < c_StepsPerSector; j++)
                {
                    var a = Direction(start + sector * j / c_StepsPerSector);
                    var b = Direction(start + sector * (j + 1) / c_StepsPerSector);
                    AddBand(mesh, rect.center, a, b, radius, 0f, c_Hub - .023f, s_Indigo);
                    AddBand(mesh, rect.center, a, b, radius, c_Hub - .023f, c_Hub - .009f, s_Ink);
                    AddBand(mesh, rect.center, a, b, radius, c_Hub - .009f, c_Hub, s_Brass);
                    AddBand(mesh, rect.center, a, b, radius, c_Hub, c_Engraving, face);
                    AddBand(mesh, rect.center, a, b, radius, c_Engraving, c_Engraving + .005f, s_Brass);
                    AddBand(mesh, rect.center, a, b, radius, c_Engraving + .005f, c_OuterEdge, face);
                    AddBand(mesh, rect.center, a, b, radius, c_OuterEdge, .979f, s_Ink);
                    AddBand(mesh, rect.center, a, b, radius, .979f, .991f, s_Brass);
                    AddBand(mesh, rect.center, a, b, radius, .991f, 1f, s_Light);
                }
                AddBand(mesh, rect.center, Direction(start - c_SpokeWidth), Direction(start + c_SpokeWidth),
                    radius, c_Hub, c_OuterEdge, s_Brass);
            }
        }

        private static Vector2 Direction(float degrees)
        {
            float angle = degrees * Mathf.Deg2Rad;
            return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
        }

        private void AddBand(VertexHelper mesh, Vector2 center, Vector2 a, Vector2 b,
            float radius, float inner, float outer, Color32 tint)
        {
            int start = mesh.currentVertCount;
            mesh.AddVert(center + a * (radius * inner), tint, Vector2.zero);
            mesh.AddVert(center + a * (radius * outer), tint, Vector2.zero);
            mesh.AddVert(center + b * (radius * outer), tint, Vector2.zero);
            mesh.AddVert(center + b * (radius * inner), tint, Vector2.zero);
            mesh.AddTriangle(start, start + 1, start + 2);
            mesh.AddTriangle(start, start + 2, start + 3);
        }
    }
}
