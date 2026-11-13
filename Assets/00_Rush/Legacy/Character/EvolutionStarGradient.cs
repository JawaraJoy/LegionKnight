using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    [DisallowMultipleComponent]
    public sealed class EvolutionStarGradient : BaseMeshEffect
    {
        private static readonly Color32[] Spectrum =
        {
            new Color32(255, 101, 133, 255), new Color32(255, 161, 79, 255),
            new Color32(255, 229, 109, 255), new Color32(99, 226, 159, 255),
            new Color32(88, 207, 250, 255), new Color32(148, 137, 255, 255),
            new Color32(251, 136, 225, 255)
        };

        public static Color32 FillColor(int tier)
        {
            switch (tier)
            {
                case 1: return new Color32(255, 213, 79, 255);
                case 2: return new Color32(255, 141, 58, 255);
                case 3: return new Color32(248, 82, 96, 255);
                default: return Color.white;
            }
        }

        protected override void OnEnable()
        {
            var image = (Image)graphic;
            image.type = Image.Type.Simple;
            image.useSpriteMesh = false;
            base.OnEnable();
        }

        public override void ModifyMesh(VertexHelper mesh)
        {
            if (!IsActive() || mesh.currentVertCount != 4) return;
            var bl = UIVertex.simpleVert;
            var tl = bl;
            var tr = bl;
            var br = bl;
            mesh.PopulateUIVertex(ref bl, 0);
            mesh.PopulateUIVertex(ref tl, 1);
            mesh.PopulateUIVertex(ref tr, 2);
            mesh.PopulateUIVertex(ref br, 3);
            mesh.Clear();
            for (int i = 0; i < Spectrum.Length; i++)
            {
                float t = (float)i / (Spectrum.Length - 1);
                mesh.AddVert(Along(bl, br, t, Spectrum[i]));
                mesh.AddVert(Along(tl, tr, t, Spectrum[i]));
                if (i == 0) continue;
                int n = i * 2;
                mesh.AddTriangle(n - 2, n - 1, n + 1);
                mesh.AddTriangle(n + 1, n, n - 2);
            }
        }

        private static UIVertex Along(UIVertex left, UIVertex right, float t, Color32 color)
        {
            left.position = Vector3.Lerp(left.position, right.position, t);
            left.uv0 = Vector4.Lerp(left.uv0, right.uv0, t);
            color.a = left.color.a;
            left.color = color;
            return left;
        }
    }
}
