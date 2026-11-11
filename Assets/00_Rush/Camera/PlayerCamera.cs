using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Rush;

namespace LegionKnight
{
    public partial class PlayerCamera : Singleton<PlayerCamera>
    {
        [SerializeField]
        private Camera m_Camera;

        [SerializeField]
        private CinemachineCamera m_CinemachineCamera;

        [SerializeField]
        private CameraPostSetField[] m_CameraPostSets;

        [SerializeField]
        private UnityEvent<bool> m_OnSetStayFollow = new();

        [SerializeField]
        private UnityEvent<Vector3> m_OnSetOffsite = new();

        private CinemachineFollow m_CinemachineFollow;
        private CameraPostSetField m_CurrentPostSet;

        private Coroutine m_OffsetRoutine;
        private Vector3 m_CurrentOffset;

        private bool m_StayFollow;

        public Camera Camera => m_Camera;

        protected override void Awake()
        {
            base.Awake();
            m_CinemachineFollow = m_CinemachineCamera.GetComponent<CinemachineFollow>();
            m_CurrentOffset = m_CinemachineFollow.FollowOffset;
        }

        private void Start()
        {
            m_CinemachineCamera.Target.TrackingTarget = RushPlayer.Instance.transform;
        }

        private CameraPostSetField GetCameraPostSet(string name)
        {
            for (int i = 0; i < m_CameraPostSets.Length; i++)
            {
                if (m_CameraPostSets[i].Config.PostName == name)
                    return m_CameraPostSets[i];
            }

            return null;
        }

        public void SetStayFollow(bool set)
        {
            m_StayFollow = set;

            m_OnSetStayFollow?.Invoke(m_StayFollow);

            m_CinemachineCamera.enabled = set;
        }

        public void SetOffsite(Vector3 offset)
        {
            if (m_OffsetRoutine != null)
            {
                StopCoroutine(m_OffsetRoutine);
                m_OffsetRoutine = null;
            }

            m_CurrentOffset = offset;

            m_CinemachineFollow.FollowOffset = offset;

            m_OnSetOffsite?.Invoke(offset);
        }

        public void SetOffSite(CameraPostSetConfig config)
        {
            if (m_OffsetRoutine != null)
            {
                StopCoroutine(m_OffsetRoutine);
            }

            m_OffsetRoutine = StartCoroutine(SetOffsiteSmooth(config.PostName));
        }

        private IEnumerator SetOffsiteSmooth(string postName)
        {
            CameraPostSetField newPostSet = GetCameraPostSet(postName);

            if (newPostSet == null)
                yield break;

            newPostSet.OnPostStartSetInvoke();

            Vector3 startOffset = m_CurrentOffset;
            Vector3 targetOffset = newPostSet.Config.Post;

            float duration = newPostSet.Config.TransitionDuration;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                Vector3 newOffset = Vector3.Lerp(
                    startOffset,
                    targetOffset,
                    elapsed / duration
                );

                m_CinemachineFollow.FollowOffset = newOffset;
                m_CurrentOffset = newOffset;

                yield return null;
            }

            m_CurrentOffset = targetOffset;
            m_CinemachineFollow.FollowOffset = targetOffset;

            m_OnSetOffsite?.Invoke(targetOffset);

            m_CurrentPostSet = newPostSet;

            newPostSet.OnPostEndSetInvoke();

            m_OffsetRoutine = null;
        }
    }
}