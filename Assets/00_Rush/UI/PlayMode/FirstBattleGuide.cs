using System;
using LegionKnight;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace Rush
{
    public sealed class FirstBattleGuide : MonoBehaviour
    {
        [Serializable]
        private sealed class StepsField
        {
            [SerializeField] internal TutorTarget Play;
            [SerializeField] internal TutorTarget Classic;
            [SerializeField] internal RectTransform ClassicAction;
        }

        [Serializable]
        private sealed class PanelField
        {
            [SerializeField] internal InvertMaskingAuto Legacy;
            [SerializeField] internal MMF_Player CursorFeedback;
            [SerializeField] internal RectTransform Overlay, Below, Above, Left, Right;
            [SerializeField] internal RectTransform Outline, Callout, Pointer;
            [SerializeField] internal TMP_Text Instruction, Heading;
        }

        [SerializeField] private StepsField m_Steps;
        [SerializeField] private PanelField m_Panel;
        private const float c_ReferenceWidth = 400f;
        private const float c_Gap = 20f, c_PointerSize = 11f, c_Margin = 6f;
        private readonly Vector3[] m_Corners = new Vector3[4];
        private RectTransform m_Target, m_Action;

        // TutorialPanel sets the target before raising its Show event.
        public void Present()
        {
            var step = m_Panel.Legacy.Target;
            bool isClassic = step == m_Steps.Classic;
            bool firstBattle = isClassic || step == m_Steps.Play;
            bool guided = step != null && step.NextButton != null && !string.IsNullOrWhiteSpace(step.Definition.Title);
            m_Panel.Legacy.gameObject.SetActive(!guided);
            m_Panel.Overlay.gameObject.SetActive(guided);
            if (!guided)
            {
                m_Target = m_Action = null;
                m_Panel.CursorFeedback.PlayFeedbacks();
                return;
            }

            m_Target = (RectTransform)step.NextButton.transform;
            m_Action = isClassic ? m_Steps.ClassicAction : m_Target;
            m_Panel.Heading.text = firstBattle ? "First Battle" : "Level Up Character";
            m_Panel.Instruction.text = step.Definition.Title;
            Canvas.ForceUpdateCanvases();
            LayoutInternal();
        }

        private void LateUpdate()
        {
            if (m_Target != null && m_Panel.Overlay.gameObject.activeInHierarchy)
                LayoutInternal();
        }

        private void LayoutInternal()
        {
            var overlay = m_Panel.Overlay;
            Rect area = overlay.rect;
            if (area.width <= 0f || area.height <= 0f) return;
            m_Target.GetWorldCorners(m_Corners);
            Vector2 low = Rect.PointToNormalized(area, overlay.InverseTransformPoint(m_Corners[0]));
            Vector2 high = Rect.PointToNormalized(area, overlay.InverseTransformPoint(m_Corners[2]));
            // Only the four surrounding graphics intercept input. The real button receives the tap.
            SetBounds(m_Panel.Below, Vector2.zero, new Vector2(1f, low.y));
            SetBounds(m_Panel.Above, new Vector2(0f, high.y), Vector2.one);
            SetBounds(m_Panel.Left, new Vector2(0f, low.y), new Vector2(low.x, high.y));
            SetBounds(m_Panel.Right, new Vector2(high.x, low.y), new Vector2(1f, high.y));
            SetBounds(m_Panel.Outline, low, high);

            var board = m_Panel.Callout;
            float scale = Mathf.Min(area.width, area.height) / c_ReferenceWidth;
            Vector2 size = board.rect.size * scale;
            float margin = c_Margin * scale, gap = c_Gap * scale;
            float bottomEdge = Mathf.Lerp(area.yMin, area.yMax, low.y);
            float topEdge = Mathf.Lerp(area.yMin, area.yMax, high.y);
            float lowerSpace = bottomEdge - area.yMin - gap - margin;
            bool below = lowerSpace >= size.y * .9f;
            if (below && lowerSpace < size.y)
            {
                scale *= lowerSpace / size.y;
                size = board.rect.size * scale;
            }
            float y = below ? bottomEdge - gap - size.y : topEdge + gap;
            float x = area.center.x - size.x * .5f;
            board.localScale = Vector3.one * scale;
            board.anchoredPosition = new Vector2(x, Mathf.Clamp(y, area.yMin + margin, area.yMax - size.y - margin));

            var pin = m_Panel.Pointer;
            float actionX = overlay.InverseTransformPoint(m_Action.TransformPoint(m_Action.rect.center)).x;
            float width = c_PointerSize * scale;
            float pinX = Mathf.Clamp(actionX, x + width, x + size.x - width);
            pin.localRotation = below ? Quaternion.Euler(0, 0, 180) : Quaternion.identity;
            pin.sizeDelta = Vector2.one * width;
            pin.anchoredPosition = new Vector2(pinX, board.anchoredPosition.y + (below ? size.y + width * .5f : -width * .5f));
        }

        private void SetBounds(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
        }
    }
}
