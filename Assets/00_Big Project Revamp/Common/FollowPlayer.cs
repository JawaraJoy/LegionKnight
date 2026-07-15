
using UnityEngine;

namespace Rush
{
    [System.Flags]
    public enum FollowAxisConstraint
    {
        None = 0,
        XUp = 1 << 0,
        XDown = 1 << 1,
        YUp = 1 << 2,
        YDown = 1 << 3,
        ZUp = 1 << 4,
        ZDown = 1 << 5
    }

    public class FollowPlayer : MonoBehaviour, ILateUpdater
    {
        [Header("Target")]
        [SerializeField]
        private FollowPostOnPlayer m_PostTypeToFollow = FollowPostOnPlayer.EnemyPost;
        private Transform m_PostToFollow;

        [Header("Follow")]
        [SerializeField]
        private float m_SmoothTime = 0.2f;

        [SerializeField]
        private float m_FollowDelay = 0f;

        [SerializeField]
        private float m_MinFollowDistance = 0.5f;

        [Header("Formation")]
        [SerializeField]
        private float m_FormationRadius = 2f;

        [SerializeField]
        private float m_RingSpacing = 1.5f;

        [SerializeField]
        private int m_FirstRingCapacity = 6;

        [Header("Axis Constraint")]
        [SerializeField]
        private FollowAxisConstraint m_AxisConstraint = FollowAxisConstraint.None;

        [Header("Random Offset")]
        [SerializeField] private float m_XOffsetMin = -0.3f;
        [SerializeField] private float m_XOffsetMax = 0.3f;
        [SerializeField] private float m_YOffsetMin = -0.3f;
        [SerializeField] private float m_YOffsetMax = 0.3f;
        [SerializeField] private float m_ZOffsetMin = -0.3f;
        [SerializeField] private float m_ZOffsetMax = 0.3f;

        private Vector3 m_Velocity;
        private Vector3 m_TargetOffset;

        private int m_FollowerIndex;
        private int m_TotalFollowers = 1;

        private float m_DelayTimer;
        private bool m_IsWaitingDelay;

        private Vector3 m_LastTargetPosition;

        public bool IsActive => m_PostToFollow != null;

        private void OnEnable()
        {
            UpdateBank.Instance.RegisterLateUpdateTick(gameObject, this);
        }

        private void Start()
        {
            SetPostToFollow(m_PostTypeToFollow);
        }

        public void SetFormationIndex(int index, int total)
        {
            m_FollowerIndex = index;
            m_TotalFollowers = Mathf.Max(1, total);
        }

        public void SetPostToFollow(FollowPostOnPlayer postType)
        {
            Transform target = null;

            switch (postType)
            {
                case FollowPostOnPlayer.EnemyPost:
                    target = RushPlayer.Instance.EnemySpawnPost;
                    break;

                case FollowPostOnPlayer.SummonPost:
                    target = RushPlayer.Instance.SummonSpawnPost;
                    break;
            }

            SetPostToFollowInternal(target);
        }

        public void NullTheFollow()
        {
            SetPostToFollowInternal(null);
        }

        private void SetPostToFollowInternal(Transform target)
        {
            m_PostToFollow = target;

            if (target == null)
                return;

            m_LastTargetPosition = target.position;

            GenerateRandomOffset();

            m_DelayTimer = 0f;
            m_IsWaitingDelay = false;
        }

        private void GenerateRandomOffset()
        {
            m_TargetOffset = new Vector3(
                Random.Range(m_XOffsetMin, m_XOffsetMax),
                Random.Range(m_YOffsetMin, m_YOffsetMax),
                Random.Range(m_ZOffsetMin, m_ZOffsetMax)
            );
        }

        public void LateTick()
        {
            if (m_PostToFollow == null)
                return;

            Vector3 targetCenter = m_PostToFollow.position;

            if (targetCenter != m_LastTargetPosition)
            {
                m_LastTargetPosition = targetCenter;

                m_DelayTimer = 0f;
                m_IsWaitingDelay = true;
            }

            if (m_IsWaitingDelay)
            {
                m_DelayTimer += Time.deltaTime;

                if (m_DelayTimer < m_FollowDelay)
                    return;

                m_IsWaitingDelay = false;
            }

            Vector3 formationTarget = GetFormationPosition(targetCenter);

            formationTarget = ApplyAxisConstraint(formationTarget);

            Vector3 dir = formationTarget - transform.position;
            float sqrDist = dir.sqrMagnitude;

            if (sqrDist <= m_MinFollowDistance * m_MinFollowDistance)
                return;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                formationTarget,
                ref m_Velocity,
                m_SmoothTime
            );
        }

        private Vector3 GetFormationPosition(Vector3 center)
        {
            int ringIndex = 0;
            int capacity = m_FirstRingCapacity;

            int index = m_FollowerIndex;

            while (index >= capacity)
            {
                index -= capacity;
                ringIndex++;
                capacity += m_FirstRingCapacity;
            }

            float radius = m_FormationRadius + (ringIndex * m_RingSpacing);

            float angleStep = 360f / capacity;
            float angle = angleStep * index;

            float rad = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Cos(rad),
                Mathf.Sin(rad),
                0f
            ) * radius;

            return center + offset + m_TargetOffset;
        }

        private Vector3 ApplyAxisConstraint(Vector3 target)
        {
            Vector3 result = transform.position;
            Vector3 delta = target - transform.position;

            // X Axis
            if (AllowAxis(delta.x, FollowAxisConstraint.XUp, FollowAxisConstraint.XDown))
                result.x = target.x;

            // Y Axis
            if (AllowAxis(delta.y, FollowAxisConstraint.YUp, FollowAxisConstraint.YDown))
                result.y = target.y;

            // Z Axis
            if (AllowAxis(delta.z, FollowAxisConstraint.ZUp, FollowAxisConstraint.ZDown))
                result.z = target.z;

            return result;
        }

        private bool AllowAxis(float delta, FollowAxisConstraint up, FollowAxisConstraint down)
        {
            bool allowUp = (m_AxisConstraint & up) != 0;
            bool allowDown = (m_AxisConstraint & down) != 0;

            // ❗ kalau axis ini tidak di-flag sama sekali → jangan gerak
            if (!allowUp && !allowDown)
                return false;

            if (delta > 0 && allowUp)
                return true;

            if (delta < 0 && allowDown)
                return true;

            return false;
        }
    }
}