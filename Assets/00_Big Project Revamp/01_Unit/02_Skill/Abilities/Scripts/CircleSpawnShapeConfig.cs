using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public enum SpawnPlane
    {
        Vertical,
        Horizontal
    }

    public enum SpawnFacingMode
    {
        KeepOriginRotation,
        FaceOutward,
        FaceInward
    }

    public enum CircleDistributionMode
    {
        Perimeter,
        RandomInside,
        RandomInsideNoOverlap
    }

    [CreateAssetMenu(fileName = "CircleSummonShape", menuName = "Rush/Combat/Shape/Circle", order = 0)]
    public class CircleSpawnShapeConfig : SpawnShapeConfig
    {
        [Header("Shape")]
        [SerializeField] private float m_Radius = 1.5f;
        [SerializeField] private SpawnPlane m_SpawnPlane = SpawnPlane.Horizontal;
        [SerializeField] private SpawnFacingMode m_FacingMode = SpawnFacingMode.FaceOutward;
        [SerializeField] private CircleDistributionMode m_DistributionMode = CircleDistributionMode.Perimeter;

        [Header("Angle")]
        [SerializeField] private float m_AngleOffsetDeg = 0f;
        [SerializeField] private float m_FacingAngleOffsetDeg = 0f;

        [Header("No Overlap")]
        [SerializeField] private float m_MinDistance = 0.75f;
        [SerializeField] private int m_MaxRandomAttempts = 20;
        [SerializeField] private bool m_FallbackToPerimeter = true;

        public override void GetSpawnTransform(
            Transform origin,
            int index,
            int totalCount,
            out Vector3 position,
            out Quaternion rotation)
        {
            if (origin == null)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return;
            }

            SpawnBatchTracker tracker = GetOrAddTracker(origin);

            if (index == 0)
            {
                tracker.ClearBatch();
            }

            GetPlaneAxes(origin, out Vector3 axisA, out Vector3 axisB);

            Vector3 offset = GetOffset(origin, tracker, axisA, axisB, index, totalCount);
            position = origin.position + offset;

            tracker.RegisterPosition(position);
            rotation = GetRotation(origin, position);
        }

        private SpawnBatchTracker GetOrAddTracker(Transform origin)
        {
            if (!origin.TryGetComponent(out SpawnBatchTracker tracker))
            {
                tracker = origin.gameObject.AddComponent<SpawnBatchTracker>();
            }

            return tracker;
        }

        private Vector3 GetOffset(
            Transform origin,
            SpawnBatchTracker tracker,
            Vector3 axisA,
            Vector3 axisB,
            int index,
            int totalCount)
        {
            switch (m_DistributionMode)
            {
                case CircleDistributionMode.RandomInside:
                    return GetRandomInsideOffset(axisA, axisB);

                case CircleDistributionMode.RandomInsideNoOverlap:
                    return GetRandomInsideNoOverlapOffset(origin, tracker.Positions, axisA, axisB, index, totalCount);

                case CircleDistributionMode.Perimeter:
                default:
                    return GetPerimeterOffset(axisA, axisB, index, totalCount);
            }
        }

        private Vector3 GetPerimeterOffset(Vector3 axisA, Vector3 axisB, int index, int totalCount)
        {
            int count = Mathf.Max(1, totalCount);
            float angleDeg = ((360f / count) * index) + m_AngleOffsetDeg;
            float rad = angleDeg * Mathf.Deg2Rad;

            return ((axisA * Mathf.Cos(rad)) + (axisB * Mathf.Sin(rad))) * m_Radius;
        }

        private Vector3 GetRandomInsideOffset(Vector3 axisA, Vector3 axisB)
        {
            float angleDeg = Random.Range(0f, 360f) + m_AngleOffsetDeg;
            float rad = angleDeg * Mathf.Deg2Rad;

            float radius = Mathf.Sqrt(Random.value) * m_Radius;
            return ((axisA * Mathf.Cos(rad)) + (axisB * Mathf.Sin(rad))) * radius;
        }

        private Vector3 GetRandomInsideNoOverlapOffset(
            Transform origin,
            IReadOnlyList<Vector3> usedPositions,
            Vector3 axisA,
            Vector3 axisB,
            int index,
            int totalCount)
        {
            int attempts = Mathf.Max(1, m_MaxRandomAttempts);

            for (int i = 0; i < attempts; i++)
            {
                Vector3 candidateOffset = GetRandomInsideOffset(axisA, axisB);
                Vector3 candidateWorldPos = origin.position + candidateOffset;

                if (IsFarEnough(candidateWorldPos, usedPositions, m_MinDistance))
                {
                    return candidateOffset;
                }
            }

            if (m_FallbackToPerimeter)
            {
                Vector3 perimeterOffset = GetPerimeterOffset(axisA, axisB, index, totalCount);
                Vector3 perimeterWorldPos = origin.position + perimeterOffset;

                if (IsFarEnough(perimeterWorldPos, usedPositions, m_MinDistance * 0.5f))
                {
                    return perimeterOffset;
                }
            }

            return GetBestEffortOffset(origin, usedPositions, axisA, axisB);
        }

        private Vector3 GetBestEffortOffset(
            Transform origin,
            IReadOnlyList<Vector3> usedPositions,
            Vector3 axisA,
            Vector3 axisB)
        {
            Vector3 bestOffset = Vector3.zero;
            float bestNearestDistance = -1f;

            int attempts = Mathf.Max(8, m_MaxRandomAttempts);

            for (int i = 0; i < attempts; i++)
            {
                Vector3 candidateOffset = GetRandomInsideOffset(axisA, axisB);
                Vector3 candidateWorldPos = origin.position + candidateOffset;

                float nearestDistance = GetNearestDistance(candidateWorldPos, usedPositions);
                if (nearestDistance > bestNearestDistance)
                {
                    bestNearestDistance = nearestDistance;
                    bestOffset = candidateOffset;
                }
            }

            return bestOffset;
        }

        private bool IsFarEnough(Vector3 candidate, IReadOnlyList<Vector3> usedPositions, float minDistance)
        {
            float minDistanceSqr = minDistance * minDistance;

            for (int i = 0; i < usedPositions.Count; i++)
            {
                if ((candidate - usedPositions[i]).sqrMagnitude < minDistanceSqr)
                {
                    return false;
                }
            }

            return true;
        }

        private float GetNearestDistance(Vector3 candidate, IReadOnlyList<Vector3> usedPositions)
        {
            if (usedPositions.Count == 0)
            {
                return float.MaxValue;
            }

            float nearestSqr = float.MaxValue;

            for (int i = 0; i < usedPositions.Count; i++)
            {
                float sqrDistance = (candidate - usedPositions[i]).sqrMagnitude;
                if (sqrDistance < nearestSqr)
                {
                    nearestSqr = sqrDistance;
                }
            }

            return Mathf.Sqrt(nearestSqr);
        }

        private void GetPlaneAxes(Transform origin, out Vector3 axisA, out Vector3 axisB)
        {
            switch (m_SpawnPlane)
            {
                case SpawnPlane.Vertical:
                    axisA = origin.right;
                    axisB = origin.up;
                    break;

                case SpawnPlane.Horizontal:
                default:
                    axisA = origin.right;
                    axisB = origin.forward;
                    break;
            }

            axisA.Normalize();
            axisB.Normalize();
        }

        private Quaternion GetRotation(Transform origin, Vector3 spawnPosition)
        {
            switch (m_FacingMode)
            {
                case SpawnFacingMode.KeepOriginRotation:
                    return ApplyFacingOffset(origin.rotation, origin);

                case SpawnFacingMode.FaceInward:
                    {
                        Vector3 dir = origin.position - spawnPosition;
                        return CreateLookRotation(origin, dir);
                    }

                case SpawnFacingMode.FaceOutward:
                default:
                    {
                        Vector3 dir = spawnPosition - origin.position;
                        return CreateLookRotation(origin, dir);
                    }
            }
        }

        private Quaternion CreateLookRotation(Transform origin, Vector3 dir)
        {
            if (dir.sqrMagnitude <= 0.0001f)
            {
                return ApplyFacingOffset(origin.rotation, origin);
            }

            dir.Normalize();

            switch (m_SpawnPlane)
            {
                case SpawnPlane.Vertical:
                    {
                        Vector3 forward = origin.forward.sqrMagnitude > 0.0001f
                            ? origin.forward.normalized
                            : Vector3.forward;

                        Quaternion rot = Quaternion.LookRotation(forward, dir);
                        return ApplyFacingOffset(rot, origin);
                    }

                case SpawnPlane.Horizontal:
                default:
                    {
                        Vector3 up = origin.up.sqrMagnitude > 0.0001f
                            ? origin.up.normalized
                            : Vector3.up;

                        Vector3 planarDir = Vector3.ProjectOnPlane(dir, up).normalized;
                        if (planarDir.sqrMagnitude <= 0.0001f)
                        {
                            return ApplyFacingOffset(origin.rotation, origin);
                        }

                        Quaternion rot = Quaternion.LookRotation(planarDir, up);
                        return ApplyFacingOffset(rot, origin);
                    }
            }
        }

        private Quaternion ApplyFacingOffset(Quaternion baseRotation, Transform origin)
        {
            if (Mathf.Abs(m_FacingAngleOffsetDeg) <= 0.0001f)
            {
                return baseRotation;
            }

            Vector3 offsetAxis = GetFacingOffsetAxis(origin);
            return Quaternion.AngleAxis(m_FacingAngleOffsetDeg, offsetAxis) * baseRotation;
        }

        private Vector3 GetFacingOffsetAxis(Transform origin)
        {
            switch (m_SpawnPlane)
            {
                case SpawnPlane.Vertical:
                    return origin.forward.sqrMagnitude > 0.0001f
                        ? origin.forward.normalized
                        : Vector3.forward;

                case SpawnPlane.Horizontal:
                default:
                    return origin.up.sqrMagnitude > 0.0001f
                        ? origin.up.normalized
                        : Vector3.up;
            }
        }
    }
}