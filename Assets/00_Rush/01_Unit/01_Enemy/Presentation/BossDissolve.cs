using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rush
{
    [DisallowMultipleComponent]
    public sealed class BossDissolve : MonoBehaviour
    {
        [SerializeField] private Shader m_Shader;
        private static readonly int s_Progress = Shader.PropertyToID("_Dissolve");
        private static readonly int s_Bounds = Shader.PropertyToID("_LocalFrame");
        private readonly Dictionary<Material, Material> m_Previous = new();
        private readonly Dictionary<Material, Material> m_Replacements = new();
        private SkeletonRenderer m_Renderer;

        public void Begin(SkeletonAnimation animation)
        {
            if (m_Shader == null || !animation.TryGetComponent(out m_Renderer)) return;
            var data = animation.skeletonDataAsset;
            Bounds frame = default;
            if (animation.TryGetComponent<MeshFilter>(out var filter) && filter.sharedMesh != null)
                frame = filter.sharedMesh.bounds;
            if (frame.size.y <= Mathf.Epsilon)
            {
                var sk = animation.Skeleton.Data;
                frame.SetMinMax(new Vector3(sk.X, sk.Y), new Vector3(sk.X + sk.Width, sk.Y + sk.Height));
            }
            var localFrame = new Vector4(frame.min.x, frame.min.y, 1f / Mathf.Max(.01f, frame.size.x), 1f / Mathf.Max(.01f, frame.size.y));
            foreach (var atlas in data.atlasAssets)
                foreach (var mat in atlas.Materials) Install(mat, localFrame, BlendMode.One, BlendMode.OneMinusSrcAlpha);
            foreach (var item in data.blendModeMaterials.additiveMaterials) Install(item.material, localFrame, BlendMode.One, BlendMode.One);
            foreach (var item in data.blendModeMaterials.screenMaterials) Install(item.material, localFrame, BlendMode.One, BlendMode.OneMinusSrcColor);
            foreach (var item in data.blendModeMaterials.multiplyMaterials) Install(item.material, localFrame, BlendMode.DstColor, BlendMode.OneMinusSrcAlpha);
        }

        private void Install(Material key, Vector4 frame, BlendMode src, BlendMode dst)
        {
            if (key == null || m_Replacements.ContainsKey(key)) return;
            var overrides = m_Renderer.CustomMaterialOverride;
            overrides.TryGetValue(key, out var previous);
            var source = previous != null ? previous : key;
            var effect = new Material(m_Shader) { mainTexture = source.mainTexture };
            effect.SetFloat("_StraightAlphaInput", source.IsKeywordEnabled("_STRAIGHT_ALPHA_INPUT") ? 1f : 0f);
            effect.SetInt("_SrcBlend", (int)src);
            effect.SetInt("_DstBlend", (int)dst);
            if (source.HasProperty("_StencilRef")) effect.SetFloat("_StencilRef", source.GetFloat("_StencilRef"));
            if (source.HasProperty("_StencilComp")) effect.SetFloat("_StencilComp", source.GetFloat("_StencilComp"));
            effect.SetVector(s_Bounds, frame);
            m_Previous.Add(key, previous);
            m_Replacements.Add(key, effect);
            overrides[key] = effect;
        }

        public void SetProgress(float progress)
        {
            foreach (var mat in m_Replacements.Values) mat.SetFloat(s_Progress, progress);
        }

        private void OnDisable()
        {
            foreach (var pair in m_Replacements)
            {
                if (m_Renderer != null)
                {
                    var overrides = m_Renderer.CustomMaterialOverride;
                    if (overrides.TryGetValue(pair.Key, out var current) && current == pair.Value)
                    {
                        if (m_Previous[pair.Key] != null) overrides[pair.Key] = m_Previous[pair.Key];
                        else overrides.Remove(pair.Key);
                    }
                }
                if (Application.isPlaying) Destroy(pair.Value);
                else DestroyImmediate(pair.Value);
            }
            m_Previous.Clear();
            m_Replacements.Clear();
            m_Renderer = null;
        }
    }
}
