using UnityEngine;

namespace Rush
{
    [DisallowMultipleComponent]
    public sealed class BattleHudLayout : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Frame;
        [SerializeField, Min(1)] private float m_MinimumHeight = 580f;
        private Rect m_LastSafe;
        private void OnEnable() => ResizeInternal();
        private void OnRectTransformDimensionsChange() => ResizeInternal();
        private void LateUpdate() { if (m_LastSafe != Screen.safeArea) ResizeInternal(); }
        private void ResizeInternal()
        {
            if (m_Frame == null) return;
            RectTransform host = (RectTransform)transform;
            Rect bounds = host.rect;
            var canvas = GetComponentInParent<Canvas>()?.rootCanvas;
            m_LastSafe = Screen.safeArea;
            if (canvas != null && canvas.renderMode != RenderMode.WorldSpace && canvas.targetDisplay == 0 && m_LastSafe.width > 0)
            {
                var cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
                if ((cam == null || cam.targetTexture == null) && RectTransformUtility.ScreenPointToLocalPointInRectangle(host,m_LastSafe.min,cam,out var min)
                    && RectTransformUtility.ScreenPointToLocalPointInRectangle(host,m_LastSafe.max,cam,out var max))
                { bounds.min = Vector2.Max(bounds.min,min); bounds.max = Vector2.Min(bounds.max,max); }
            }
            float scale = Mathf.Min(bounds.width / 480f, bounds.height / Mathf.Max(1f, m_MinimumHeight));
            if (scale <= 0) return;
            m_Frame.anchoredPosition = bounds.center - host.rect.center;
            m_Frame.localScale = new Vector3(scale,scale,1);
            m_Frame.sizeDelta = bounds.size / scale;
        }
    }
}
