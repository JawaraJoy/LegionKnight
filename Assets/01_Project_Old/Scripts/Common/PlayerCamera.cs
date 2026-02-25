using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Rush;

namespace LegionKnight
{
    [System.Serializable]
    public partial class CameraPostSet
    {
        [SerializeField]
        private string m_PostName;
        [SerializeField]
        private float m_TransitionDuration = 0.1f;
        [SerializeField]
        private Vector3 m_Post;
        public string PostName => m_PostName;
        public Vector3 Post => m_Post;
        [SerializeField]
        private UnityEvent m_OnPostStartSet = new();
        [SerializeField]
        private UnityEvent m_OnPostEndSet = new();
        public float TransitionDuration => m_TransitionDuration;
        public void OnPostStartSetInvoke()
        {
            m_OnPostStartSet?.Invoke();
        }
        public void OnPostEndSetInvoke()
        {
            m_OnPostEndSet?.Invoke();
        }
    }
    public partial class PlayerCamera : Singleton<PlayerCamera>
    {
        [SerializeField]
        private CinemachineCamera m_CinemachineCamera;
        [SerializeField]
        private List<CameraPostSet> m_CameraPostSets = new();
        private bool m_StayFollow;
        [SerializeField]
        private UnityEvent<bool> m_OnSetStayFollow = new();
        [SerializeField]
        private UnityEvent<Vector3> m_OnSetOffsite = new();

        private CameraPostSet m_CurrentPostSet;
        private void Start()
        {
            m_CinemachineCamera.Target.TrackingTarget = RushPlayer.Instance.transform;
        }
        private CameraPostSet GetCameraPostSet(string nam)
        {
            CameraPostSet match = m_CameraPostSets.Find(x => x.PostName == nam);
            return match;
        }
        public void SetStayFollow(bool set)
        {
            m_StayFollow = set;
            OnSetStayFollowInvoke();
        }
        private void OnSetStayFollowInvoke()
        {
            m_OnSetStayFollow?.Invoke(m_StayFollow);
        }
        private void OnSetOffsite(Vector3 set)
        {
            m_OnSetOffsite?.Invoke(set);
        }
        public void SetOffsite(Vector3 set)
        {
            m_CinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset = set;
            OnSetOffsite(set);
        }
        public void SetOffSite(string postName)
        {
            StartCoroutine(SetOffsiteSmooth(postName));
        }
        private IEnumerator SetOffsiteSmooth(string postName)
        {
            CameraPostSet newPostSet = GetCameraPostSet(postName);
            newPostSet.OnPostStartSetInvoke();
            Vector3 newPost = newPostSet.Post;
            float duration = newPostSet.TransitionDuration;
            Vector3 currentPost = m_CinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset;
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                m_CinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset = Vector3.Lerp(currentPost, newPost, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            m_CinemachineCamera.GetComponent<CinemachineFollow>().FollowOffset = newPost;
            OnSetOffsite(newPost);
            m_CurrentPostSet = newPostSet;
            newPostSet.OnPostEndSetInvoke();
        }
    }
}
