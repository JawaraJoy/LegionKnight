using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    [RequireComponent(typeof(Image))]
    public partial class InvertMaskingAuto : MonoBehaviour, ICanvasRaycastFilter
    {
        [Header("Hole Target")]
        [SerializeField, MMReadOnly] private TutorTarget m_Target;
        [SerializeField] private RectTransform m_Cursor;

        [Header("Hole Appearance")]
        [SerializeField, Range(0f, 0.5f)]
        private float m_HoleCornerRadius = 0.05f;
        [SerializeField]
        private Color m_ImageColor = new(0f, 0f, 0f, 0.8f);
        [SerializeField]
        private Vector2 m_TargetOffset = Vector2.zero;
        [SerializeField]
        private Vector2 m_TargetSizeOffset = Vector2.zero;
        [SerializeField]
        private UnityEvent<Vector2> m_OnHolePositionChanged = new();

        [Header("Runtime Info (ReadOnly)")]
        [SerializeField, MMReadOnly] private RectTransform m_TargetRectTransform;
        [SerializeField, MMReadOnly] private Vector2 m_HolePosition;

        // --- Cached references ---
        private Image m_Image;
        private RectTransform m_RectTransform;
        private Material m_RuntimeMaterial;

        // --- Shader property IDs ---
        private static readonly int HoleRectID = Shader.PropertyToID("_HoleRect");
        private static readonly int HoleRadiusID = Shader.PropertyToID("_HoleRadius");

        // --- Internal tracking ---
        private Vector3 m_LastTargetPos;
        private Vector2 m_LastTargetSize;

        public TutorTarget Target => m_Target;

        #region Unity Lifecycle

        private void Awake()
        {
            CacheComponents();
            SetupMaterial();
            UpdateMask();
            UpdateColor();
        }

        private void OnEnable()
        {
            SetupMaterial();
            UpdateMask();
            UpdateColor();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            CacheComponents();
            SetupMaterial();
            UpdateMask();
            UpdateColor();
        }
#endif

        /*private void LateUpdate()
        {
            if (m_Target == null || m_TargetRectTransform == null)
                return;

            // Only update if target moves or changes size
            Vector3 pos = m_TargetRectTransform.position;
            Vector2 size = m_TargetRectTransform.rect.size;
            if (pos != m_LastTargetPos || size != m_LastTargetSize)
            {
                UpdateMask();
                m_LastTargetPos = pos;
                m_LastTargetSize = size;
            }
        }*/

        private void OnDisable()
        {
            if (m_RuntimeMaterial != null)
            {
#if UNITY_EDITOR
                DestroyImmediate(m_RuntimeMaterial);
#else
                Destroy(m_RuntimeMaterial);
#endif
            }
        }

        #endregion

        #region Setup

        private void CacheComponents()
        {
            if (m_Image == null)
                m_Image = GetComponent<Image>();
            if (m_RectTransform == null)
                m_RectTransform = GetComponent<RectTransform>();
        }

        private void SetupMaterial()
        {
            if (m_Image == null) return;

            var shader = Shader.Find("UI/InvertMaskRoundedRect");
            if (shader == null)
            {
                Debug.LogError("Shader 'UI/InvertMaskRoundedRect' not found!");
                return;
            }

            if (m_RuntimeMaterial == null || m_RuntimeMaterial.shader != shader)
            {
                if (m_RuntimeMaterial != null)
                    DestroyImmediate(m_RuntimeMaterial);

                m_RuntimeMaterial = new Material(shader)
                {
                    name = "InvertMaskRuntimeMat"
                };
                m_Image.material = m_RuntimeMaterial;
            }
        }

        public void SetMaskingTarget(TutorTarget maskingTarget)
        {
            m_Target = maskingTarget;
            m_TargetRectTransform = m_Target != null ? m_Target.GetComponent<RectTransform>() : null;
            if (isActiveAndEnabled)
                StartCoroutine(DelayUpdate(0.05f));
        }

        private IEnumerator DelayUpdate(float delay)
        {
            yield return new WaitForSeconds(delay);
            UpdateMask();
        }

        #endregion

        #region Update & Appearance

        public void SetColor(Color color)
        {
            m_ImageColor = color;
            UpdateColor();
        }

        private void UpdateColor()
        {
            if (m_Image != null)
                m_Image.color = m_ImageColor;
        }
        public void Refresh()
        {
            UpdateMask();
        }
        private void UpdateMask()
        {
            if (m_RuntimeMaterial == null || m_RectTransform == null || m_TargetRectTransform == null)
                return;

            // Get world corners
            Vector3[] maskCorners = new Vector3[4];
            Vector3[] targetCorners = new Vector3[4];
            m_RectTransform.GetWorldCorners(maskCorners);
            m_TargetRectTransform.GetWorldCorners(targetCorners);

            // Calculate sizes
            float maskLeft = maskCorners[0].x;
            float maskBottom = maskCorners[0].y;
            float maskWidth = m_RectTransform.rect.width * m_RectTransform.lossyScale.x;
            float maskHeight = m_RectTransform.rect.height * m_RectTransform.lossyScale.y;

            float targetLeft = targetCorners[0].x;
            float targetBottom = targetCorners[0].y;
            float targetWidth = m_TargetRectTransform.rect.width * m_TargetRectTransform.lossyScale.x + m_TargetSizeOffset.x;
            float targetHeight = m_TargetRectTransform.rect.height * m_TargetRectTransform.lossyScale.y + m_TargetSizeOffset.y;

            float offsetX = m_TargetOffset.x;
            float offsetY = m_TargetOffset.y;

            // Normalize to mask space
            float centerX = ((targetLeft + targetWidth / 2f + offsetX) - maskLeft) / maskWidth;
            float centerY = ((targetBottom + targetHeight / 2f + offsetY) - maskBottom) / maskHeight;
            float sizeX = targetWidth / maskWidth;
            float sizeY = targetHeight / maskHeight;

            m_HolePosition = new Vector2(centerX, centerY);
            m_RuntimeMaterial.SetVector(HoleRectID, new Vector4(centerX, centerY, sizeX, sizeY));
            m_RuntimeMaterial.SetFloat(HoleRadiusID, m_HoleCornerRadius);

            if (m_OnHolePositionChanged?.GetPersistentEventCount() > 0)
                m_OnHolePositionChanged.Invoke(m_HolePosition);

            SetCursorPosition(m_HolePosition);
        }

        private void SetCursorPosition(Vector2 position)
        {
            if (m_Cursor == null) return;

            Vector2 localPos = new(
                (position.x * m_RectTransform.rect.width) - (m_RectTransform.rect.width * 0.5f),
                (position.y * m_RectTransform.rect.height) - (m_RectTransform.rect.height * 0.5f)
            );

            m_Cursor.localPosition = localPos;
        }

        #endregion

        #region Raycast Handling

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (m_RectTransform == null || m_RuntimeMaterial == null)
                return true;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectTransform, sp, eventCamera, out Vector2 local);
            Vector2 uv = Rect.PointToNormalized(m_RectTransform.rect, local);

            Vector4 holeRect = m_RuntimeMaterial.GetVector(HoleRectID);
            float radius = m_HoleCornerRadius;

            Vector2 rectCenter = new(holeRect.x, holeRect.y);
            Vector2 rectSize = new Vector2 (holeRect.z, holeRect.w) * 0.5f;

            Vector2 localToHole = uv - rectCenter;
            Vector2 d = new Vector2 (Mathf.Abs(localToHole.x), Mathf.Abs(localToHole.y)) - rectSize + new Vector2(radius, radius);
            float dist = Mathf.Min(Mathf.Max(d.x, d.y), 0f) + (Vector2.Max(d, Vector2.zero)).magnitude - radius;

            // Let clicks pass through the transparent hole
            return dist >= 0f;
        }

        #endregion
    }
}