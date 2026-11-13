using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class FlyItem : MonoBehaviour, IUpdater
    {
        [SerializeField] private SpriteRenderer m_Renderer;
        [SerializeField] private PadDefinition m_PadDefinition;

        private const float c_DropDistance = .85f;
        private const float c_DropDuration = .32f;
        private Transform m_Target;
        private Vector3 m_Origin;
        private Vector3 m_FlightStart;
        private float m_Elapsed;
        private bool m_IsBossDrop;
        public bool IsActive => gameObject.activeInHierarchy;

        private void OnEnable() => UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        private void OnDisable()
        {
            m_Target = null;
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }

        public void Init(CollectibleConfig item, PadDefinition pad) => Init(item, pad, null, false);

        public void Init(CollectibleConfig item, PadDefinition pad, Transform hero, bool bossDrop)
        {
            m_PadDefinition = pad;
            m_IsBossDrop = bossDrop;
            m_Target = hero;
            if (!bossDrop && pad != null)
            {
                var targetPad = RushGameManager.Instance.FlyCollectManager.GetPadByDefinition(pad);
                m_Target = targetPad != null ? targetPad.transform : null;
            }
            if (m_Target == null || pad == null || item == null)
            {
                gameObject.SetActive(false);
                return;
            }
            m_Renderer.sprite = item.CollectibleField.Icon;
            m_Origin = transform.position;
            m_FlightStart = bossDrop ? m_Origin + Vector3.down * c_DropDistance : m_Origin;
            m_Elapsed = 0f;
        }

        private Vector3 GetTargetWorldPosition()
        {
            if (m_Target is RectTransform rect)
            {
                var screen = RectTransformUtility.WorldToScreenPoint(null, rect.position);
                var world = PlayerCamera.Instance.Camera.ScreenToWorldPoint(screen);
                world.z = 0f;
                return world;
            }
            var position = m_Target.position;
            position.z = 0f;
            return position;
        }

        public void Tick()
        {
            if (m_Target == null || !m_Target.gameObject.activeInHierarchy)
            {
                gameObject.SetActive(false);
                return;
            }
            m_Elapsed += Time.deltaTime;
            float delay = m_IsBossDrop ? c_DropDuration : m_PadDefinition.DelayBeforeFly;
            if (m_Elapsed < delay)
            {
                if (m_IsBossDrop)
                {
                    float t = m_Elapsed / c_DropDuration;
                    transform.position = Vector3.Lerp(m_Origin, m_FlightStart, t * t);
                }
                return;
            }
            float progress = Mathf.Clamp01((m_Elapsed - delay) * m_PadDefinition.FlySpeed);
            transform.position = Vector3.Lerp(m_FlightStart, GetTargetWorldPosition(), progress);
            if (progress >= 1f) gameObject.SetActive(false);
        }
    }
}
