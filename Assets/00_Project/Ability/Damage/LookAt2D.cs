using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Bson;

namespace Rush
{
    public enum LookMode
    {
        OnStart,
        Continuous
    }

    [DisallowMultipleComponent]
    public class LookAt2D : MonoBehaviour
    {
        [Header("Look At Settings")]
        public LookMode lookMode = LookMode.Continuous;
        [Header("Targeting")]
        [Tooltip("Only consider targets within this radius (world units). Set <= 0 for unlimited.")]
        public float searchRadius = 0f;

        [Tooltip("Filter which layers are considered as valid targets.")]
        public LayerMask targetLayers = ~0;

        [Tooltip("How often (seconds) to rescan for the nearest target.")]
        [Min(0.02f)] public float rescanInterval = 0.25f;

        [Header("Facing/Rotation")]
        [Tooltip("Rotate this transform's Z so its local forward points at the target.")]
        public bool rotateToward = true;

        [Tooltip("If true, don't rotate; instead flip the SpriteRenderer's X based on target direction.")]
        public bool flipSpriteXInstead = false;

        [Tooltip("The axis your sprite considers 'forward'. Right for typical sprites, Up for top-down arrows, etc.")]
        public Vector2 localForward = Vector2.right;

        [Tooltip("Degrees to add after computing the look angle (useful if your sprite points up by default).")]
        public float rotationOffsetDegrees = 0f;

        [Tooltip("Max angular speed in degrees/second. Set <= 0 for instant rotation.")]
        public float maxTurnSpeed = 720f;

        [Header("Components (optional)")]
        [Tooltip("Assign if you want flipSpriteXInstead to control this SpriteRenderer. If null, tries to GetComponent.")]
        public SpriteRenderer spriteRenderer;

        Transform _currentTarget;

        void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void OnEnable()
        {
            StartCoroutine(RescanLoop());
        }

        IEnumerator RescanLoop()
        {
            var wait = new WaitForSeconds(rescanInterval);
            while (enabled)
            {
                _currentTarget = FindNearestTarget();
                yield return wait;
            }
        }

        void Start()
        {
            StartLookInternal();
        }
        public void StartLook()
        {
            StartLookInternal();
        }
        private void StartLookInternal()
        {
            if (lookMode == LookMode.OnStart)
            {
                _currentTarget = FindNearestTarget();
                if (_currentTarget != null)
                {
                    // instantly face the target
                    Vector2 toTarget = (Vector2)_currentTarget.position - (Vector2)transform.position;
                    if (toTarget.sqrMagnitude > Mathf.Epsilon)
                    {
                        float targetAngle = ComputeLookAngle(toTarget);
                        SetZRotation(targetAngle); // instant set
                    }
                }
            }
        }

        void Update()
        {
            if (lookMode == LookMode.Continuous)
            {
                if (_currentTarget == null) _currentTarget = FindNearestTarget();
                if (_currentTarget != null) LookAtTarget();
            }
        }

        void LookAtTarget()
        {
            Vector2 toTarget = (Vector2)_currentTarget.position - (Vector2)transform.position;
            if (toTarget.sqrMagnitude < Mathf.Epsilon) return;

            if (flipSpriteXInstead && spriteRenderer != null)
            {
                bool faceLeft = toTarget.x < 0f;
                spriteRenderer.flipX = faceLeft;
            }
            else if (rotateToward)
            {
                float targetAngle = ComputeLookAngle(toTarget);
                if (maxTurnSpeed <= 0f)
                    SetZRotation(targetAngle);
                else
                {
                    float currentZ = NormalizeAngle(transform.eulerAngles.z);
                    float newZ = Mathf.MoveTowardsAngle(currentZ, targetAngle, maxTurnSpeed * Time.deltaTime);
                    SetZRotation(newZ);
                }
            }
        }

        float ComputeLookAngle(Vector2 direction)
        {
            // World angle toward target (relative to +X axis)
            float worldAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Local forward reference angle (e.g., if forward is Up, add -90 to align)
            float forwardRef = Mathf.Atan2(localForward.y, localForward.x) * Mathf.Rad2Deg;
            float finalAngle = worldAngle - forwardRef + rotationOffsetDegrees;
            return NormalizeAngle(finalAngle);
        }

        void SetZRotation(float zDegrees)
        {
            Vector3 e = transform.eulerAngles;
            e.z = zDegrees;
            transform.eulerAngles = e;
        }

        float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

        Transform FindNearestTarget()
        {
            Transform nearest = null;
            float bestDistSq = float.PositiveInfinity;

            if (searchRadius > 0f)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, searchRadius, targetLayers);
                for (int i = 0; i < hits.Length; i++)
                {
                    Transform t = hits[i].transform;
                    float dSq = ((Vector2)t.position - (Vector2)transform.position).sqrMagnitude;
                    if (dSq < bestDistSq)
                    {
                        bestDistSq = dSq;
                        nearest = t;
                    }
                }

                if (nearest != null) return nearest;
            }

            // Fallback: Physics2D.OverlapCollider or scene scan
            GameObject[] candidates = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            Vector2 p = transform.position;
            for (int i = 0; i < candidates.Length; i++)
            {
                GameObject go = candidates[i];
                if (((1 << go.layer) & targetLayers) == 0) continue;
                float dSq = ((Vector2)go.transform.position - p).sqrMagnitude;
                if (searchRadius > 0f && dSq > searchRadius * searchRadius) continue;
                if (dSq < bestDistSq)
                {
                    bestDistSq = dSq;
                    nearest = go.transform;
                }
            }

            return nearest;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (searchRadius > 0f)
            {
                Gizmos.color = new Color(1, 1, 1, 0.5f);
                Gizmos.DrawWireSphere(transform.position, searchRadius);
            }

            if (_currentTarget != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _currentTarget.position);
            }
        }
#endif
    }
}
/// <summary>
/// Make a 2D object "look at" (rotate toward) the nearest target with a given tag.
/// Works for top‑down and side‑scroll games. Optionally flips a SpriteRenderer instead of rotating.
/// </summary>

