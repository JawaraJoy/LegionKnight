using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    [RequireComponent(typeof(Image))]
    public partial class ImageInvertMasking : MonoBehaviour, ICanvasRaycastFilter
    {
        [SerializeField] private Vector2 m_HoleCenter = new Vector2(0.5f, 0.5f); // normalized (0-1)
        [SerializeField] private Vector2 m_HoleSize = new Vector2(0.3f, 0.2f);   // normalized (0-1)
        [SerializeField] private float m_HoleCornerRadius = 0.05f;               // normalized (0-1)
        [SerializeField] private Color m_ImageColor = Color.white;

        private Material m_RuntimeMaterial;
        private Image m_Image;

        private void Awake()
        {
            SetupMaterial();
            UpdateMask();
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
            if (m_Image == null)
                m_Image = GetComponent<Image>();
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

        public void SetHole(Vector2 normalizedCenter, Vector2 normalizedSize, float normalizedRadius)
        {
            m_HoleCenter = normalizedCenter;
            m_HoleSize = normalizedSize;
            m_HoleCornerRadius = normalizedRadius;
            UpdateMask();
        }

        public void SetColor(Color color)
        {
            m_ImageColor = color;
            UpdateColor();
        }

        private void UpdateMask()
        {
            if (m_RuntimeMaterial != null)
            {
                m_RuntimeMaterial.SetVector("_HoleRect", new Vector4(m_HoleCenter.x, m_HoleCenter.y, m_HoleSize.x, m_HoleSize.y));
                m_RuntimeMaterial.SetFloat("_HoleRadius", m_HoleCornerRadius);
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
            RectTransform rt = (RectTransform)transform;
            Vector2 local;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, sp, eventCamera, out local);
            Vector2 uv = Rect.PointToNormalized(rt.rect, local);

            // Calculate if the point is inside the rounded rect hole
            Vector2 rectCenter = m_HoleCenter;
            Vector2 rectSize = m_HoleSize * 0.5f;
            float radius = m_HoleCornerRadius;

            Vector2 localToHole = uv - rectCenter;
            Vector2 d = new Vector2(Mathf.Abs(localToHole.x), Mathf.Abs(localToHole.y)) - rectSize + new Vector2(radius, radius);
            float dist = Mathf.Min(Mathf.Max(d.x, d.y), 0.0f) + (Vector2.Max(d, Vector2.zero)).magnitude - radius;

            // If inside the hole, let the click pass through (return false)
            return dist >= 0;
        }
    }
}
