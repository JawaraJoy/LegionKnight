using Rush;
using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    public class FlyItem : MonoBehaviour
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
                    m_Pad = GameManager.Instance.PadManager.GetPadByDefinition(m_PadDefinition);
                }
                return m_Pad;
            }
        }

        public void Init(ScriptableObject defi, PadDefinition paddefi)
        {
            m_PadDefinition = paddefi;

            if (defi is HeroUnitConfig heroConfig)
                m_Renderer.sprite = heroConfig.CollectibleField.Icon;

            if (defi is ItemConfig itemConfig)
                m_Renderer.sprite = itemConfig.CollectibleField.Icon;

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
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                worldPos.z = 0; // Ensure 2D plane
                return worldPos;
            }

            // Normal world object
            Vector3 pos = target.position;
            pos.z = 0;
            return pos;
        }

        // -------------------------
        // UPDATE-BASED FLYING
        // -------------------------
        private void Update()
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
