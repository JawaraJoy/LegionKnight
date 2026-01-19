// CLEAN REWRITE – Infinite Wheel Scroll (Drag + Snap + Scale + Fade + Curve)
// Uses m_ prefix, ScriptableObject data, clean architecture, no unused parameters.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegionKnight
{
    public class InfiniteWheelScroll : UIView, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform m_ContentRoot;
        [SerializeField] private WheelItemUI m_ItemPrefab;

        [Header("Data")]
        [SerializeField] private List<WheelItemDefinition> m_ItemDefinitions = new();

        [Header("Wheel Layout")]
        [SerializeField] private int m_VisibleItemCount = 9;
        [SerializeField] private float m_Radius = 300f;
        [SerializeField] private float m_AngleStep = 20f;
        [SerializeField] private float m_CurveHeight = 0.4f;

        [Header("Visuals")]
        [SerializeField] private float m_CenterItemScale = 1.25f;
        [SerializeField] private float m_SideItemScale = 0.8f;
        [SerializeField] private float m_FadeStrength = 0.3f;

        [Header("Animation")]
        [SerializeField] private float m_SnapSpeed = 8f;
        [SerializeField] private float m_DragMultiplier = 0.25f;
        [SerializeField] private float m_DragVelocityDamp = 4f;

        [Header("Events")]
        [SerializeField] private UnityEvent<WheelItemDefinition> m_OnCenteredItemChanged;

        // Internal
        private readonly List<RectTransform> m_SpawnedItems = new();
        private float m_CurrentAngle = 0f;
        private float m_TargetAngle = 0f;
        private float m_DragVelocity = 0f;
        private bool m_IsDragging = false;
        private float m_LastPointerX;
        private int m_CurrentIndex = 0;
        private bool m_CenterEventFired = false;

        // ------------------------------------------------------

        private void Start()
        {
            RefreshInternal();
        }

        [ContextMenu("Refresh Wheel")]
        private void RefreshInternal()
        {
            GenerateItems();
            ForceSnap();
            UpdateWheelVisuals();
        }

        public void Init()
        {
            PlayerPlatformDeck platformDeck = Player.Instance.GetPlayerPlatformDeck();
            List<PlatformUnit> platformUnits = new(platformDeck.GetPlatformUnits());
            foreach (PlatformUnit unit in platformUnits)
            {
                WheelItemDefinition item = unit.StanbyPlatform.WheelDefi;
                if (!m_ItemDefinitions.Contains(item))
                {
                    m_ItemDefinitions.Add(item);
                }
            }
            RefreshInternal();
        }

        // ------------------------------------------------------
        private void Update()
        {
            if (m_IsDragging)
            {
                UpdateWheelVisuals();
                return;
            }

            if (Mathf.Abs(m_DragVelocity) > 0.01f)
            {
                m_CurrentAngle += m_DragVelocity * Time.deltaTime;
                m_DragVelocity = Mathf.Lerp(m_DragVelocity, 0, Time.deltaTime * m_DragVelocityDamp);
                UpdateWheelVisuals();
                return;
            }

            // Snap smoothly
            m_CurrentAngle = Mathf.Lerp(m_CurrentAngle, m_TargetAngle, Time.deltaTime * m_SnapSpeed);

            if (Mathf.Abs(m_CurrentAngle - m_TargetAngle) < 0.001f)
            {
                m_CurrentAngle = m_TargetAngle;
                m_CenterEventFired = false;
            }

            UpdateWheelVisuals();
        }

        // ------------------------------------------------------
        #region Item Generation
        private void GenerateItems()
        {
            m_SpawnedItems.Clear();
            foreach (Transform t in m_ContentRoot) Destroy(t.gameObject);

            if (m_ItemDefinitions.Count == 0) return;

            for (int i = 0; i < m_VisibleItemCount; i++)
            {
                int defIndex = i % m_ItemDefinitions.Count;

                var itemUI = Instantiate(m_ItemPrefab, m_ContentRoot);
                itemUI.Setup(m_ItemDefinitions[defIndex]);

                var rect = itemUI.Rect;
                m_SpawnedItems.Add(rect);

                int localIndex = i;
                itemUI.Button.onClick.AddListener(() => MoveToIndex(localIndex));
            }
        }
        #endregion

        // ------------------------------------------------------
        #region Wheel Core
        private void UpdateWheelVisuals()
        {
            int count = m_SpawnedItems.Count;
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                float angle = (i * m_AngleStep) + m_CurrentAngle;
                float rad = angle * Mathf.Deg2Rad;

                float x = Mathf.Sin(rad) * m_Radius;
                float y = Mathf.Cos(rad) * m_Radius * m_CurveHeight;

                RectTransform item = m_SpawnedItems[i];
                item.anchoredPosition = new Vector2(x, y);

                // Depth sorting
                item.SetSiblingIndex(Mathf.RoundToInt(y));

                // Fade
                float fadeT = Mathf.Clamp01(1f - Mathf.Abs(Mathf.DeltaAngle(angle, 0)) / 180f);
                float alpha = Mathf.Lerp(m_FadeStrength, 1f, fadeT);
                item.GetComponent<CanvasGroup>().alpha = alpha;

                // Scale
                float scaleT = Mathf.Clamp01(1f - Mathf.Abs(Mathf.DeltaAngle(angle, 0)) / m_AngleStep);
                float scale = Mathf.Lerp(m_SideItemScale, m_CenterItemScale, scaleT);
                item.localScale = Vector3.one * scale;
            }

            UpdateCenteredItem();
        }

        private void UpdateCenteredItem()
        {
            int count = m_SpawnedItems.Count;
            int newIndex = Mathf.RoundToInt(-m_CurrentAngle / m_AngleStep);
            newIndex = (newIndex % count + count) % count;

            if (newIndex != m_CurrentIndex)
            {
                m_CurrentIndex = newIndex;
                m_CenterEventFired = false;
            }

            if (!m_CenterEventFired && Mathf.Abs(m_CurrentAngle - m_TargetAngle) < 0.01f)
            {
                m_CenterEventFired = true;
                int defIndex = m_CurrentIndex % m_ItemDefinitions.Count;
                m_OnCenteredItemChanged?.Invoke(m_ItemDefinitions[defIndex]);
            }
        }

        private void ForceSnap()
        {
            m_TargetAngle = Mathf.Round(m_CurrentAngle / m_AngleStep) * m_AngleStep;
            m_CurrentAngle = m_TargetAngle;
            m_CenterEventFired = false;
        }
        #endregion

        // ------------------------------------------------------
        #region Navigation
        public void MoveLeft() => MoveToIndex(m_CurrentIndex - 1);
        public void MoveRight() => MoveToIndex(m_CurrentIndex + 1);

        public void MoveToIndex(int index)
        {
            int total = m_SpawnedItems.Count;
            index = (index % total + total) % total;
            m_TargetAngle = -index * m_AngleStep;
            m_CenterEventFired = false;
        }
        #endregion

        // ------------------------------------------------------
        #region Drag Handling
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_IsDragging = true;
            m_DragVelocity = 0;
            m_LastPointerX = eventData.position.x;
        }

        public void OnDrag(PointerEventData eventData)
        {
            float delta = eventData.position.x - m_LastPointerX;
            m_CurrentAngle += delta * m_DragMultiplier;
            m_DragVelocity = delta * (m_DragMultiplier * 2f);
            m_LastPointerX = eventData.position.x;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            m_IsDragging = false;
            m_TargetAngle = Mathf.Round(m_CurrentAngle / m_AngleStep) * m_AngleStep;
            m_CenterEventFired = false;
        }
        #endregion
    }
}
