using System;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class EquipCardTutorial : MonoBehaviour
    {
        [Serializable]
        private class NavigationField
        {
            [SerializeField] internal HomePanel Home;
            [SerializeField] internal PreparationPanel Preparation;
            [SerializeField] internal Button OpenPreparation;
            [SerializeField] internal Button OpenCard;
            [SerializeField] internal RectTransform CardViewport;
        }

        [Serializable]
        private class GuideField
        {
            [SerializeField] internal RectTransform Overlay;
            [SerializeField] internal RectTransform Below, Above, Left, Right;
            [SerializeField] internal RectTransform Outline, Callout, Pointer;
            [SerializeField] internal TMP_Text Instruction;
            [SerializeField] internal Button Skip;
        }

        [SerializeField] private NavigationField m_Navigation;
        [SerializeField] private GuideField m_Guide;

        private const string c_UnlockKey = "equipplatformunlock";
        private const string c_DoneKey = "equipplatformisdone";
        private const string c_PreparationText = "Tap <b>Preparation</b> to open your loadout.";
        private const string c_CardText = "Tap <b>Card</b> to open your deck.";
        private const float c_Margin = .055f, c_CalloutWidth = .89f, c_CalloutHeight = .217f;
        private const float c_PointerGap = .056f, c_PointerWidth = .027f;
        private const float c_ReferenceWidth = 400f;
        private readonly Vector3[] m_Corners = new Vector3[4];
        private bool m_Pending, m_Released;
        private Button m_Target;

        private void OnEnable()
        {
            m_Guide.Skip.onClick.AddListener(FinishInternal);
        }

        private void Start()
        {
            if (GameSetting.Instance.DeveloperMode) return;
            var save = UnityService.Instance;
            if (!m_Pending && save.GetData<bool>(c_UnlockKey) && !save.GetData<bool>(c_DoneKey))
            {
                m_Pending = true;
                m_Released = true;
            }
        }

        // The level-up panel calls these from its existing show and dismiss events.
        public void Arm()
        {
            var save = UnityService.Instance;
            if (!isActiveAndEnabled || GameSetting.Instance.DeveloperMode || save.GetData<bool>(c_DoneKey)) return;
            m_Pending = true;
            m_Released = false;
            if (!save.GetData<bool>(c_UnlockKey)) save.SaveData(c_UnlockKey, true);
        }

        public void Begin()
        {
            if (isActiveAndEnabled && m_Pending) m_Released = true;
        }

        private void LateUpdate()
        {
            if (!m_Pending || !m_Released || !m_Navigation.Home.Content.activeInHierarchy)
            {
                HideGuideInternal();
                return;
            }

            bool inPreparation = m_Navigation.Preparation.Content.activeInHierarchy;
            if (inPreparation && m_Navigation.Preparation.SelectCharacterMode == SelectCharacterMode.Card)
            {
                FinishInternal();
                return;
            }

            var target = inPreparation ? m_Navigation.OpenCard : m_Navigation.OpenPreparation;
            if (!target.isActiveAndEnabled || !target.IsInteractable())
            {
                HideGuideInternal();
                return;
            }

            // Update after the navigation layout has settled, including its expanding tabs.
            if (m_Target != target)
            {
                Canvas.ForceUpdateCanvases();
                m_Guide.Instruction.text = inPreparation ? c_CardText : c_PreparationText;
                m_Target = target;
            }

            var bounds = BoundsInOverlay((RectTransform)target.transform);
            if (inPreparation) bounds = Intersection(bounds, BoundsInOverlay(m_Navigation.CardViewport));
            var area = m_Guide.Overlay.rect;
            bounds = Intersection(bounds, area);
            if (bounds.width <= 0 || bounds.height <= 0)
            {
                HideGuideInternal();
                return;
            }

            LayoutInternal(area, bounds);
            if (!m_Guide.Overlay.gameObject.activeSelf) m_Guide.Overlay.gameObject.SetActive(true);
        }

        private Rect BoundsInOverlay(RectTransform rect)
        {
            rect.GetWorldCorners(m_Corners);
            var low = m_Guide.Overlay.InverseTransformPoint(m_Corners[0]);
            var high = m_Guide.Overlay.InverseTransformPoint(m_Corners[2]);
            return Rect.MinMaxRect(low.x, low.y, high.x, high.y);
        }

        private static Rect Intersection(Rect a, Rect b) => Rect.MinMaxRect(
            Mathf.Max(a.xMin, b.xMin), Mathf.Max(a.yMin, b.yMin),
            Mathf.Min(a.xMax, b.xMax), Mathf.Min(a.yMax, b.yMax));

        private void LayoutInternal(Rect area, Rect hole)
        {
            // Four raycastable panels leave the actual target free for Unity's EventSystem.
            Place(m_Guide.Below, area.xMin, area.yMin, area.xMax, hole.yMin);
            Place(m_Guide.Above, area.xMin, hole.yMax, area.xMax, area.yMax);
            Place(m_Guide.Left, area.xMin, hole.yMin, hole.xMin, hole.yMax);
            Place(m_Guide.Right, hole.xMax, hole.yMin, area.xMax, hole.yMax);
            Place(m_Guide.Outline, hole.xMin, hole.yMin, hole.xMax, hole.yMax);

            float unit = Mathf.Min(area.width, area.height);
            float width = unit * c_CalloutWidth, height = unit * c_CalloutHeight;
            float gap = unit * c_PointerGap;
            float left = Mathf.Clamp(hole.center.x - width * .5f, area.xMin + unit * c_Margin,
                area.xMax - unit * c_Margin - width);
            float bottom = Mathf.Min(hole.yMax + gap, area.yMax - height);
            var board = m_Guide.Callout;
            board.localScale = Vector3.one * (unit / c_ReferenceWidth);
            board.anchoredPosition = new Vector2(left, bottom);
            float radius = unit * c_PointerWidth * .5f;
            float tipX = Mathf.Clamp(hole.center.x, left + radius, left + width - radius);
            Place(m_Guide.Pointer, tipX - radius, bottom - radius * 2, tipX + radius, bottom);
        }

        private void Place(RectTransform rect, float left, float bottom, float right, float top)
        {
            rect.anchoredPosition = new Vector2(left, bottom);
            rect.sizeDelta = new Vector2(right - left, top - bottom);
        }

        private void FinishInternal()
        {
            if (!m_Pending) return;
            m_Pending = m_Released = false;
            HideGuideInternal();
            UnityService.Instance.SaveData(c_DoneKey, true);
        }

        private void HideGuideInternal()
        {
            if (m_Guide.Overlay.gameObject.activeSelf) m_Guide.Overlay.gameObject.SetActive(false);
            m_Target = null;
        }

        private void OnDisable()
        {
            m_Guide.Skip.onClick.RemoveListener(FinishInternal);
            HideGuideInternal();
        }
    }
}