using System.Collections;
using UnityEngine;

namespace LegionKnight
{
    public class FlyItem : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_Renderer;
        [SerializeField] private PadDefinition m_PadDefinition;

        private Pad m_Pad;

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

            if (defi is CharacterDefinition charDef)
                m_Renderer.sprite = charDef.Icon;

            if (defi is CurrencyDefinition itemDef)
                m_Renderer.sprite = itemDef.Icon;

            StartCoroutine(FlyToTarget());
        }

        // -------------------------
        // UNIVERSAL POSITION HANDLER
        // -------------------------
        private Vector3 GetTargetWorldPosition(Transform target)
        {
            // If target is a UI element
            if (target is RectTransform rectTarget)
            {
                Canvas canvas = rectTarget.GetComponentInParent<Canvas>();

                // World Space Canvas → already world position
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    Vector3 wp = rectTarget.position;
                    wp.z = 0;
                    return wp;
                }

                // Screen Space Canvas → convert to world point
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, rectTarget.position);

                float zDistance = Mathf.Abs(Camera.main.transform.position.z - transform.position.z);
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));
                worldPos.z = 0;
                return worldPos;
            }

            // Normal world object
            Vector3 pos = target.position;
            pos.z = 0;
            return pos;
        }

        // -------------------------
        // FLY ANIMATION
        // -------------------------
        private IEnumerator FlyToTarget()
        {
            yield return new WaitForSeconds(m_PadDefinition.DelayBeforeFly);

            Vector3 startPos = transform.position;
            Vector3 targetPos = GetTargetWorldPosition(TargetPad.transform);
            float flySpeed = TargetPad.Definition.FlySpeed;

            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * flySpeed;
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}
