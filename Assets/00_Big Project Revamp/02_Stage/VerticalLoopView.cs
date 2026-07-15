using LegionKnight;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class VerticalLoopView : View, ILateUpdater
    {
        [Header("Config")]
        [SerializeField] 
        private VerticalBackgroundConfig m_Config;
        private Transform m_Player;
        private Camera m_Camera;

        [Header("Scene References")]
        [SerializeField] 
        private SpriteRenderer m_BaseRenderer;
        [SerializeField]
        private SpriteRenderer[] m_SkyRenderers;

        private float m_BaseHeight;
        private float m_SkyStartY;

        private readonly List<float> m_SegmentHeights = new();

        private bool m_SkyActivated;
        private float m_CurrentHighestY;

        public bool IsActive => IsShowInternal;

        private void Start()
        {
            RushGameManager.Instance.StageManager.Init(this);
        }
        public void Init(VerticalBackgroundConfig config)
        {
            m_Player = RushPlayer.Instance.transform;
            m_Camera = PlayerCamera.Instance.Camera;

            m_Config = config;

            m_SkyActivated = false;
            m_CurrentHighestY = 0f;

            ApplyConfig();

            InitializeHeights();
            DeactivateSky();

            ShowInternal();
        }

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterLateUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
           // UpdateBank.Instance.UnregisterLateUpdateTick(gameObject);
        }

        private void ApplyConfig()
        {
            if (m_Config == null)
                return;

            m_BaseRenderer.sprite = m_Config.BaseSprite;

            var sprites = m_Config.SkySegmentSprites;
            int count = Mathf.Min(m_SkyRenderers.Length, sprites.Length);

            for (int i = 0; i < count; i++)
            {
                m_SkyRenderers[i].sprite = sprites[i];
            }
        }

        private void InitializeHeights()
        {
            m_SegmentHeights.Clear();

            m_BaseHeight = m_BaseRenderer.bounds.size.y;

            m_SkyStartY = m_BaseRenderer.transform.position.y
                          + m_BaseHeight
                          + m_Config.SkyStartOffset;

            foreach (var renderer in m_SkyRenderers)
            {
                if (renderer.sprite == null)
                    continue;

                m_SegmentHeights.Add(renderer.bounds.size.y);
            }
        }

        private void DeactivateSky()
        {
            foreach (var renderer in m_SkyRenderers)
            {
                renderer.gameObject.SetActive(false);
            }
        }

        public void LateTick()
        {
            if (m_Player == null || m_Camera == null) return;
            if (!m_SkyActivated)
            {
                TryActivateSky();
                return;
            }

            HandleLooping();
        }

        private void TryActivateSky()
        {
            if (m_Player.position.y < m_SkyStartY)
                return;

            m_SkyActivated = true;

            float spawnY = m_SkyStartY;

            for (int i = 0; i < m_SkyRenderers.Length; i++)
            {
                var renderer = m_SkyRenderers[i];

                renderer.gameObject.SetActive(true);

                renderer.transform.position = new Vector3(
                    renderer.transform.position.x,
                    spawnY,
                    renderer.transform.position.z
                );

                spawnY += m_SegmentHeights[i];
            }

            m_CurrentHighestY = spawnY - m_SegmentHeights[^1];
        }

        private void HandleLooping()
        {
            float camY = m_Camera.transform.position.y;

            for (int i = 0; i < m_SkyRenderers.Length; i++)
            {
                var renderer = m_SkyRenderers[i];
                float height = m_SegmentHeights[i];

                if (renderer.transform.position.y + height < camY - height)
                {
                    m_CurrentHighestY += height;

                    renderer.transform.position = new Vector3(
                        renderer.transform.position.x,
                        m_CurrentHighestY,
                        renderer.transform.position.z
                    );
                }
            }
        }
    }
}