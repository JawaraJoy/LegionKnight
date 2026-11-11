using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public sealed class LeaderboardViewport : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Board;
        [SerializeField] private ScrollRect m_List;

        // The game's canvas uses a 1920-unit reference width.
        private const float c_Width = 1920f;
        private const float c_MinimumHeight = 3000f;

        private void OnEnable()
        {
            ResizeInternal();
            if (m_List == null || m_List.content == null) return;
            m_List.StopMovement();
            m_List.content.anchoredPosition = Vector2.zero;
        }

        private void OnRectTransformDimensionsChange() => ResizeInternal();

        private void ResizeInternal()
        {
            if (m_Board == null) return;
            var host = (RectTransform)transform;
            Rect bounds = host.rect;
            var canvas = GetComponentInParent<Canvas>()?.rootCanvas;
            if (canvas != null && canvas.renderMode != RenderMode.WorldSpace && canvas.targetDisplay == 0)
            {
                var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
                if ((camera == null || camera.targetTexture == null) && Screen.safeArea.size.sqrMagnitude > 0f &&
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(host, Screen.safeArea.min, camera, out var min) &&
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(host, Screen.safeArea.max, camera, out var max))
                {
                    bounds.min = Vector2.Max(bounds.min, min);
                    bounds.max = Vector2.Min(bounds.max, max);
                }
            }

            float scale = Mathf.Min(bounds.width / c_Width, bounds.height / c_MinimumHeight);
            if (scale <= 0f) return;
            m_Board.anchoredPosition = bounds.center - host.rect.center;
            m_Board.sizeDelta = new Vector2(c_Width, bounds.height / scale);
            m_Board.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
