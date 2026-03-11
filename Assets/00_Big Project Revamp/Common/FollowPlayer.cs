using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Flags]
    public enum FollowAxisConstraint
    {
        None = 0,
        XUp = 1 << 0, // Follow X hanya saat bertambah
        XDown = 1 << 1, // Follow X hanya saat berkurang
        YUp = 1 << 2, // Follow Y hanya saat bertambah
        YDown = 1 << 3, // Follow Y hanya saat berkurang
        ZUp = 1 << 4, // Follow Z hanya saat bertambah
        ZDown = 1 << 5, // Follow Z hanya saat berkurang
    }

    public class FollowPlayer : MonoBehaviour, ILateUpdater
    {
        [SerializeField, MMReadOnly]
        private Transform m_PostToFollow;

        [SerializeField]
        private float m_SmoothTime = 0.2f;

        [SerializeField]
        private float m_FollowDelay = 1f;

        [SerializeField, Tooltip("Batasi axis mana yang diikuti berdasarkan arah perubahan. None = ikuti semua axis bebas.")]
        private FollowAxisConstraint m_AxisConstraint = FollowAxisConstraint.None;

        private Vector3 m_Velocity;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_LastAllowedPosition; // posisi target yang sudah difilter

        private float m_DelayTimer;
        private bool m_IsWaitingForDelay;

        public bool IsActive => m_PostToFollow != null;

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterLateUpdateTick(gameObject, this);
        }

        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterLateUpdateTick(gameObject);
        }

        private void Start()
        {
            Transform target = RushPlayer.Instance.EnemySpawnPost;
            SetPostToFollowInternal(target);
        }

        public void SetPostToFollow(Transform target)
        {
            SetPostToFollowInternal(target);
        }

        private void SetPostToFollowInternal(Transform target)
        {
            m_PostToFollow = target;
            if (m_PostToFollow != null)
            {
                m_LastTargetPosition = m_PostToFollow.position;
                m_LastAllowedPosition = m_PostToFollow.position;
                m_DelayTimer = 0f;
                m_IsWaitingForDelay = false;
            }
        }

        public void LateTick()
        {
            if (m_PostToFollow == null)
                return;

            Vector3 currentTargetPos = m_PostToFollow.position;

            // Detect position change
            if (currentTargetPos != m_LastTargetPosition)
            {
                m_LastTargetPosition = currentTargetPos;
                m_DelayTimer = 0f;
                m_IsWaitingForDelay = true;
            }

            // Handle delay
            if (m_IsWaitingForDelay)
            {
                m_DelayTimer += Time.deltaTime;
                if (m_DelayTimer >= m_FollowDelay)
                    m_IsWaitingForDelay = false;
                else
                    return;
            }

            // Terapkan constraint per axis
            Vector3 filteredTarget = m_AxisConstraint == FollowAxisConstraint.None
                ? currentTargetPos
                : ApplyAxisConstraintInternal(currentTargetPos);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                filteredTarget,
                ref m_Velocity,
                m_SmoothTime
            );
        }

        /// <summary>
        /// Filter target position berdasarkan AxisConstraint.
        /// Tiap axis hanya diupdate jika arah perubahannya sesuai flag yang diset.
        /// Jika axis tidak punya flag sama sekali, axis itu bebas diikuti.
        /// </summary>
        private Vector3 ApplyAxisConstraintInternal(Vector3 targetPos)
        {
            Vector3 result = m_LastAllowedPosition;

            result.x = ResolveAxisInternal(
                m_LastAllowedPosition.x,
                targetPos.x,
                FollowAxisConstraint.XUp,
                FollowAxisConstraint.XDown
            );

            result.y = ResolveAxisInternal(
                m_LastAllowedPosition.y,
                targetPos.y,
                FollowAxisConstraint.YUp,
                FollowAxisConstraint.YDown
            );

            result.z = ResolveAxisInternal(
                m_LastAllowedPosition.z,
                targetPos.z,
                FollowAxisConstraint.ZUp,
                FollowAxisConstraint.ZDown
            );

            m_LastAllowedPosition = result;
            return result;
        }

        /// <summary>
        /// Tentukan apakah nilai axis boleh diupdate berdasarkan arah perubahan dan flag constraint.
        /// Jika axis tidak punya flag sama sekali → bebas ikuti.
        /// Jika ada flag → hanya ikuti jika arahnya sesuai.
        /// </summary>
        private float ResolveAxisInternal(float current, float target, FollowAxisConstraint upFlag, FollowAxisConstraint downFlag)
        {
            bool hasUpFlag = (m_AxisConstraint & upFlag) != 0;
            bool hasDownFlag = (m_AxisConstraint & downFlag) != 0;

            // Tidak ada constraint pada axis ini → bebas ikuti
            if (!hasUpFlag && !hasDownFlag)
                return target;

            float delta = target - current;

            if (delta > 0f && hasUpFlag) return target; // naik dan boleh naik
            if (delta < 0f && hasDownFlag) return target; // turun dan boleh turun

            return current; // arah tidak diizinkan → tahan posisi
        }
    }
}