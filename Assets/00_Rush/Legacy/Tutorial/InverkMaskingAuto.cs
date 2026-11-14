using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    [RequireComponent(typeof(Image))]
    public partial class InvertMaskingAuto : MonoBehaviour, ICanvasRaycastFilter
    {
        private const float c_MinSize = 0.001f;
        private const float c_Half = 0.5f;
        private static readonly int s_HoleRectId = Shader.PropertyToID("_HoleRect");
        private static readonly int s_HoleRadiusId = Shader.PropertyToID("_HoleRadius");
        [SerializeField, MMReadOnly] private TutorTarget m_Target;
        [SerializeField] private RectTransform m_Cursor;
        [SerializeField, Range(0f, c_Half)] private float m_HoleCornerRadius = 0.05f;
        [SerializeField] private Color m_ImageColor = new(0f, 0f, 0f, 0.8f);
        [SerializeField] private Vector2 m_TargetOffset = Vector2.zero;
        [SerializeField] private Vector2 m_TargetSizeOffset = Vector2.zero;
        [SerializeField] private UnityEvent<Vector2> m_OnHolePositionChanged = new();
        [SerializeField, MMReadOnly] private RectTransform m_TargetRectTransform;
        [SerializeField, MMReadOnly] private Vector2 m_HolePosition;
        private Image m_Image;
        private RectTransform m_RectTransform;
        private Material m_RuntimeMaterial;
        private readonly Vector3[] m_Corners = new Vector3[4];
        public TutorTarget Target => m_Target;

        private void Awake() => SetupInternal();
        private void OnEnable()
        {
            SetupInternal();
            Canvas.willRenderCanvases += RefreshBeforeRender;
        }
        private void OnDisable()
        {
            Canvas.willRenderCanvases -= RefreshBeforeRender;
            if (m_Image != null) m_Image.material = null;
            if (m_RuntimeMaterial == null) return;
            if (Application.isPlaying) Destroy(m_RuntimeMaterial);
            else DestroyImmediate(m_RuntimeMaterial);
            m_RuntimeMaterial = null;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (isActiveAndEnabled) SetupInternal();
        }
#endif
        private void SetupInternal()
        {
            m_Image = GetComponent<Image>();
            TryGetComponent(out m_RectTransform);
            if (m_RuntimeMaterial == null)
            {
                var shader = Shader.Find("UI/InvertMaskRoundedRect");
                if (shader == null) return;
                m_RuntimeMaterial = new Material(shader) { name = "InvertMaskRuntimeMat", hideFlags = HideFlags.DontSave };
            }
            m_Image.material = m_RuntimeMaterial;
            m_Image.color = m_ImageColor;
            UpdateMaskInternal();
        }
        public void SetMaskingTarget(TutorTarget target)
        {
            m_Target = target;
            m_TargetRectTransform = null;
            if (target != null) target.TryGetComponent(out m_TargetRectTransform);
            UpdateMaskInternal();
        }
        public void SetColor(Color color)
        {
            m_ImageColor = color;
            if (m_Image != null) m_Image.color = color;
        }
        public void Refresh() => UpdateMaskInternal();
        private void RefreshBeforeRender() => UpdateMaskInternal();
        private void UpdateMaskInternal()
        {
            if (m_RuntimeMaterial == null || m_RectTransform == null) return;
            if (m_Target == null || !m_Target.gameObject.activeInHierarchy || m_TargetRectTransform == null)
            {
                m_RuntimeMaterial.SetVector(s_HoleRectId, Vector4.zero);
                return;
            }
            var rect = m_RectTransform.rect;
            if (rect.width <= c_MinSize || rect.height <= c_MinSize) return;
            m_TargetRectTransform.GetWorldCorners(m_Corners);
            var min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            var max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            foreach (var corner in m_Corners)
            {
                Vector2 local = m_RectTransform.InverseTransformPoint(corner);
                min = Vector2.Min(min, local);
                max = Vector2.Max(max, local);
            }
            var size = max - min + m_TargetSizeOffset;
            var center = (max + min) * c_Half + m_TargetOffset;
            m_HolePosition = Rect.PointToNormalized(rect, center);
            m_RuntimeMaterial.SetVector(s_HoleRectId, new Vector4(m_HolePosition.x, m_HolePosition.y, size.x / rect.width, size.y / rect.height));
            m_RuntimeMaterial.SetFloat(s_HoleRadiusId, m_HoleCornerRadius);
            if (m_Cursor != null)
            {
                Vector3 world = m_RectTransform.TransformPoint(center);
                m_Cursor.position = world;
            }
            OnHolePositionChangedInvoke();
        }
        private void OnHolePositionChangedInvoke() => m_OnHolePositionChanged?.Invoke(m_HolePosition);
        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            if (m_RectTransform == null || m_RuntimeMaterial == null || m_Target == null || !m_Target.gameObject.activeInHierarchy) return true;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectTransform, screenPoint, eventCamera, out var local)) return true;
            Vector2 uv = Rect.PointToNormalized(m_RectTransform.rect, local);
            Vector4 hole = m_RuntimeMaterial.GetVector(s_HoleRectId);
            var d = new Vector2(Mathf.Abs(uv.x - hole.x), Mathf.Abs(uv.y - hole.y)) -
                new Vector2(hole.z, hole.w) * c_Half + Vector2.one * m_HoleCornerRadius;
            float distance = Mathf.Min(Mathf.Max(d.x, d.y), 0f) + Vector2.Max(d, Vector2.zero).magnitude - m_HoleCornerRadius;
            return distance >= 0f;
        }
    }
}
