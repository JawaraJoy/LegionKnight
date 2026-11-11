using LegionKnight;
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class VerticalLoopView : View, ILateUpdater
    {
        [SerializeField] private VerticalBackgroundConfig m_Config;
        [SerializeField, MMReadOnly] private Transform m_Player;
        [SerializeField, MMReadOnly] private Camera m_Camera;
        [SerializeField] private SpriteRenderer m_BaseRenderer;
        [SerializeField] private GameObject m_Foreground;
        [SerializeField, Range(0f, 1f)] private float m_BaseGroundHeightRatio = 0.12f;
        [SerializeField] private SpriteRenderer[] m_SkyRenderers;

        private readonly List<SpriteRenderer> m_Segments = new();
        private readonly List<float> m_SegmentHeights = new();
        private readonly List<float> m_BottomOffsets = new();
        private float m_SkyStartY;
        private float m_CycleHeight;
        private const float c_CoverMargin = 1.002f;
        private bool m_BasePoseCaptured;
        private bool m_IsInitialized;
        private Vector3 m_BaseLocalPosition;
        private Vector3 m_BaseLocalScale;
        public bool IsActive => m_Content != null && IsShowInternal;

        private void Awake()
        {
            if (m_Foreground != null) m_Foreground.SetActive(false);
        }

        private void Start()
        {
            RushGameManager.Instance.StageManager.Init(this);
        }

        private void OnEnable()
        {
            if (UpdateBank.Instance != null)
                UpdateBank.Instance.RegisterLateUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            if (UpdateBank.Instance != null)
                UpdateBank.Instance.UnregisterLateUpdateTick(gameObject);
        }

private void LateUpdate()
        {
            // Menu backgrounds must still cover viewport changes while UpdateBank is paused by UI.
            if (IsActive && m_CycleHeight <= Mathf.Epsilon)
                LateTickInternal();
        }


        public void Init(VerticalBackgroundConfig config)
        {
            InitInternal(config);
        }

        private void InitInternal(VerticalBackgroundConfig config)
        {
            m_IsInitialized = false;
            m_Segments.Clear();
            m_SegmentHeights.Clear();
            m_BottomOffsets.Clear();
            m_CycleHeight = 0f;
            if (m_SkyRenderers != null)
            {
                foreach (SpriteRenderer segment in m_SkyRenderers)
                    if (segment != null) segment.gameObject.SetActive(false);
            }

            m_Config = config;
            if (m_Foreground != null)
                m_Foreground.SetActive(config != null && config.ShowForeground);
            if (config == null || m_BaseRenderer == null) return;

            // Layout does not depend on player/camera singleton initialization order.
            if (!m_BasePoseCaptured)
            {
                m_BaseLocalPosition = m_BaseRenderer.transform.localPosition;
                m_BaseLocalScale = m_BaseRenderer.transform.localScale;
                m_BasePoseCaptured = true;
            }
            m_BaseRenderer.transform.localPosition = m_BaseLocalPosition;
            m_BaseRenderer.transform.localScale = m_BaseLocalScale;
            m_BaseRenderer.sprite = config.BaseSprite;
            if (config.SkySegmentSprites != null && config.SkySegmentSprites.Length > 0)
                AlignBattleBackgroundInternal();
            m_SkyStartY = m_BaseRenderer.bounds.max.y + config.SkyStartOffset;
            Sprite[] sprites = config.SkySegmentSprites;
            if (m_SkyRenderers != null)
            {
                for (int i = 0; i < m_SkyRenderers.Length; i++)
                {
                    SpriteRenderer segment = m_SkyRenderers[i];
                    if (segment == null || segment == m_BaseRenderer || m_Segments.Contains(segment))
                        continue;

                    segment.sprite = sprites != null && sprites.Length > 0
                        ? sprites[i % sprites.Length] : null;
                    if (segment.sprite == null) continue;

                    segment.gameObject.SetActive(true);
                    float height = segment.bounds.size.y;
                    if (height <= Mathf.Epsilon)
                    {
                        segment.gameObject.SetActive(false);
                        continue;
                    }

                    m_Segments.Add(segment);
                    m_SegmentHeights.Add(height);
                    m_BottomOffsets.Add(segment.transform.position.y - segment.bounds.min.y);
                    m_CycleHeight += height;
                }
            }

            m_IsInitialized = true;
            LayoutSegmentsInternal(m_SkyStartY);
            LateTickInternal();
            ShowInternal();
        }

        public void LateTick()
        {
            LateTickInternal();
        }

        private void LateTickInternal()
        {
            if (!m_IsInitialized || m_BaseRenderer == null || m_Config == null) return;
            if (m_Camera == null)
            {
                PlayerCamera playerCamera = PlayerCamera.Instance;
                if (playerCamera != null) m_Camera = playerCamera.Camera;
            }
            if (m_Camera == null) return;
            if (m_CycleHeight <= Mathf.Epsilon)
            {
                FitStaticBackgroundInternal();
                return;
            }

            float depth = Vector3.Dot(m_BaseRenderer.bounds.center - m_Camera.transform.position,
                m_Camera.transform.forward);
            float bottomY = Mathf.Min(
                m_Camera.ViewportToWorldPoint(new Vector3(0f, 0f, depth)).y,
                m_Camera.ViewportToWorldPoint(new Vector3(1f, 0f, depth)).y);
            LayoutSegmentsInternal(bottomY);
        }

        private void LayoutSegmentsInternal(float bottomY)
        {
            if (m_CycleHeight <= Mathf.Epsilon) return;

            // Direct cycle calculation handles teleports and descent without a frame-by-frame catch-up.
            float cycle = Mathf.Max(0f, Mathf.Floor((bottomY - m_SkyStartY) / m_CycleHeight));
            float segmentBottom = m_SkyStartY;
            for (int i = 0; i < m_Segments.Count; i++)
            {
                float y = segmentBottom + cycle * m_CycleHeight;
                if (y + m_SegmentHeights[i] <= bottomY)
                    y += m_CycleHeight;

                Vector3 position = m_Segments[i].transform.position;
                position.y = y + m_BottomOffsets[i];
                m_Segments[i].transform.position = position;
                segmentBottom += m_SegmentHeights[i];
            }
        }
    

private void FitStaticBackgroundInternal()
        {
            if (m_BaseRenderer.sprite == null) return;
            Bounds image = m_BaseRenderer.bounds;
            if (image.size.x <= Mathf.Epsilon || image.size.y <= Mathf.Epsilon) return;
            float depth = Vector3.Dot(image.center - m_Camera.transform.position,
                m_Camera.transform.forward);
            if (depth <= m_Camera.nearClipPlane) return;

            Vector3 lower = m_Camera.ViewportToWorldPoint(new Vector3(0f, 0f, depth));
            Vector3 upper = m_Camera.ViewportToWorldPoint(new Vector3(1f, 1f, depth));
            float factor = Mathf.Max(Mathf.Abs(upper.x - lower.x) / image.size.x,
                Mathf.Abs(upper.y - lower.y) / image.size.y) * c_CoverMargin;
            Vector3 scale = m_BaseRenderer.transform.localScale;
            scale.x *= factor;
            scale.y *= factor;
            m_BaseRenderer.transform.localScale = scale;

            Vector3 offset = (lower + upper) * 0.5f - m_BaseRenderer.bounds.center;
            offset.z = 0f;
            m_BaseRenderer.transform.position += offset;
        }

        private void AlignBattleBackgroundInternal()
        {
            RushPlayer player = RushPlayer.Instance;
            if (player == null || m_BaseRenderer.sprite == null) return;
            Bounds bounds = m_BaseRenderer.bounds;
            float groundY = bounds.min.y + bounds.size.y * m_BaseGroundHeightRatio;
            Vector3 position = transform.position;
            position.y += player.transform.position.y - groundY;
            transform.position = position;
        }
}
}
