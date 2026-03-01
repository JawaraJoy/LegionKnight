using Rush;
using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    public class FlyItem : MonoBehaviour, IUpdater
    {
        [SerializeField] private SpriteRenderer m_Renderer;
        [SerializeField] private PadDefinition m_PadDefinition;

        private Pad m_Pad;

        // Flight state
        private bool isFlying = false;
        private float flySpeed;
        private float lerpT;
        private Vector3 startPos;
        private Vector3 targetPos;

        private Pad TargetPad
        {
            get
            {
                if (m_Pad == null)
                {
                    m_Pad = RushGameManager.Instance.FlyCollectManager.GetPadByDefinition(m_PadDefinition);
                }
                return m_Pad;
            }
        }

        public bool IsActive => gameObject.activeInHierarchy;

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }
        public void Init(CollectibleConfig defi, PadDefinition paddefi)
        {
            m_PadDefinition = paddefi;
            m_Renderer.sprite = defi.CollectibleField.Icon;
            // Prepare movement values
            flySpeed = TargetPad.Definition.FlySpeed;
            lerpT = 0f;

            // Start delayed flight
            StartCoroutine(StartFlightAfterDelay());
        }

        private IEnumerator StartFlightAfterDelay()
        {
            yield return new WaitForSeconds(m_PadDefinition.DelayBeforeFly);

            startPos = transform.position;
            targetPos = GetTargetWorldPosition(TargetPad.transform);

            isFlying = true;
        }

        // -------------------------
        // UNIVERSAL POSITION HANDLER
        // -------------------------
        private Vector3 GetTargetWorldPosition(Transform target)
        {
            // If the target is UI (RectTransform) convert to world position
            if (target is RectTransform rect)
            {
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, rect.position);
                Vector3 worldPos = PlayerCamera.Instance.Camera.ScreenToWorldPoint(screenPos);
                worldPos.z = 0; // Ensure 2D plane
                return worldPos;
            }

            // Normal world object
            Vector3 pos = target.position;
            pos.z = 0;
            return pos;
        }


        public void Tick()
        {
            if (!isFlying)
                return;

            // Update target every frame (UI may move)
            targetPos = GetTargetWorldPosition(TargetPad.transform);

            lerpT += Time.deltaTime * flySpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, lerpT);

            // End
            if (lerpT >= 1f)
            {
                isFlying = false;
                gameObject.SetActive(false);
            }
        }
    }
}
