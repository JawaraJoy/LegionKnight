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
        [SerializeField] private MaskingTarget m_Target;
        [SerializeField]
        private RectTransform m_Cursor;

        [Header("Hole Appearance")]
        [SerializeField] private float m_HoleCornerRadius = 0.05f; // normalized (0-1)
        [SerializeField] private Color m_ImageColor = Color.white;
        [SerializeField] private Vector2 m_TargetOffset = Vector2.zero;      // Offset for hole position (pixels)
        [SerializeField] private Vector2 m_TargetSizeOffset = Vector2.zero;  // Offset for hole size (pixels)
        [SerializeField]
        private UnityEvent<Vector2> m_OnHolePositionChanged = new ();

        private Vector2 m_HolePosition;

        private Material m_RuntimeMaterial;
        private Image m_Image;
        private RectTransform m_RectTransform;

        private RectTransform m_TargetRectTransform;

        public DialogueDefinition DialogueDefinition => m_Target != null ? m_Target.DialogueDefinition : null;
        public string DialogueId => m_Target != null ? m_Target.DialogueId : string.Empty;
        public string DialogueTitle => m_Target != null ? m_Target.DialogueTitle : string.Empty;
        public string[] DialogueDescriptions => m_Target != null ? m_Target.DialogueDescriptions : new string[0];
        public bool IsTutorialCompleted => m_Target != null ? m_Target.IsTutorialCompleted : false;
        public bool CanTutor => m_Target != null ? m_Target.CanTutor : false;

        private void Awake()
        {
            SetupMaterial();
            UpdateMask();
            UpdateColor();
        }
        private void OnEnable()
        {
            UpdateMask();
            SetupMaterial();
            UpdateColor();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetupMaterial();
            UpdateMask();
            UpdateColor();
        }
#endif

        private void SetupMaterial()
        {
            // Get references to components
            m_TargetRectTransform = m_Target.GetComponent<RectTransform>();
            if (m_Image == null)
                m_Image = GetComponent<Image>();
            if (m_RectTransform == null)
                m_RectTransform = GetComponent<RectTransform>();
            if (m_Image.material == null || m_Image.material.shader.name != "UI/InvertMaskRoundedRect")
            {
                m_RuntimeMaterial = new Material(Shader.Find("UI/InvertMaskRoundedRect"));
                m_Image.material = m_RuntimeMaterial;
            }
            else
            {
                m_RuntimeMaterial = m_Image.material;
            }
        }

        public void SetMaskingTarget(MaskingTarget maskingTarget)
        {
            m_Target = maskingTarget;
            //StartCoroutine(Delay(0.1f)); // Delay to ensure the target is fully initialized
        }

        private IEnumerator Delay(float delay)
        {
            yield return new WaitForSeconds(delay);
            UpdateMask();
            SetupMaterial();
            UpdateColor();
        }

        private void Update()
        {
            // In case the target moves or resizes at runtime
            if (m_Target != null)
                UpdateMask();
                UpdateColor();
                SetupMaterial();
        }

        public void SetColor(Color color)
        {
            m_ImageColor = color;
            UpdateColor();
        }

        private void UpdateMask()
        {
            if (m_RuntimeMaterial == null || m_RectTransform == null || m_Target == null)
                return;

            // Get world corners of both mask and target
            Vector3[] maskCorners = new Vector3[4];
            Vector3[] targetCorners = new Vector3[4];
            m_RectTransform.GetWorldCorners(maskCorners);
            m_TargetRectTransform.GetWorldCorners(targetCorners);

            // Calculate mask rect in world space
            float maskLeft = maskCorners[0].x;
            float maskBottom = maskCorners[0].y;
            float maskWidth = m_RectTransform.rect.width * m_RectTransform.lossyScale.x;
            float maskHeight = m_RectTransform.rect.height * m_RectTransform.lossyScale.y;

            // Calculate target center and size in world space
            float targetLeft = targetCorners[0].x;
            float targetBottom = targetCorners[0].y;
            float targetWidth = m_TargetRectTransform.rect.width * m_TargetRectTransform.lossyScale.x;
            float targetHeight = m_TargetRectTransform.rect.height * m_TargetRectTransform.lossyScale.y;

            // Apply size offset (in pixels)
            float offsetWidth = m_TargetSizeOffset.x;
            float offsetHeight = m_TargetSizeOffset.y;
            targetWidth += offsetWidth;
            targetHeight += offsetHeight;

            // Apply position offset (in pixels)
            float offsetX = m_TargetOffset.x;
            float offsetY = m_TargetOffset.y;

            // Normalized center and size relative to mask
            float centerX = ((targetLeft + targetWidth / 2f + offsetX) - maskLeft) / maskWidth;
            float centerY = ((targetBottom + targetHeight / 2f + offsetY) - maskBottom) / maskHeight;
            float sizeX = targetWidth / maskWidth;
            float sizeY = targetHeight / maskHeight;

            m_RuntimeMaterial.SetVector("_HoleRect", new Vector4(centerX, centerY, sizeX, sizeY));
            m_RuntimeMaterial.SetFloat("_HoleRadius", m_HoleCornerRadius);

            m_HolePosition = new Vector2(centerX, centerY); // Store the hole position for raycast validation
            m_OnHolePositionChanged.Invoke(m_HolePosition);

            SetCursorPosition(m_HolePosition); // Update cursor position
        }

        private void SetCursorPosition(Vector2 position)
        {
            if (m_Cursor != null)
            {
                // Convert normalized position to local position in the mask rect
                Vector2 localPosition = new Vector2(
                    (position.x * m_RectTransform.rect.width) - (m_RectTransform.rect.width * 0.5f),
                    (position.y * m_RectTransform.rect.height) - (m_RectTransform.rect.height * 0.5f)
                );
                m_Cursor.localPosition = localPosition;
            }
        }

        private void UpdateColor()
        {
            if (m_Image != null)
            {
                m_Image.color = m_ImageColor;
            }
        }

        // --- Raycast filter for click-through hole ---
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (m_RectTransform == null || m_Target == null)
                return true;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_RectTransform, sp, eventCamera, out Vector2 local);
            Vector2 uv = Rect.PointToNormalized(m_RectTransform.rect, local);

            // Get latest hole rect
            Vector4 holeRect = m_RuntimeMaterial != null ? m_RuntimeMaterial.GetVector("_HoleRect") : Vector4.zero;
            float radius = m_HoleCornerRadius;

            Vector2 rectCenter = new Vector2(holeRect.x, holeRect.y);
            Vector2 rectSize = new Vector2(holeRect.z, holeRect.w) * 0.5f;

            Vector2 localToHole = uv - rectCenter;
            Vector2 d = new Vector2(Mathf.Abs(localToHole.x), Mathf.Abs(localToHole.y)) - rectSize + new Vector2(radius, radius);
            float dist = Mathf.Min(Mathf.Max(d.x, d.y), 0.0f) + (Vector2.Max(d, Vector2.zero)).magnitude - radius;

            // If inside the hole, let the click pass through (return false)
            return dist >= 0;
        }
    }
}
