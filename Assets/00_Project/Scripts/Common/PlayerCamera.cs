using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class CameraPostSet
    {
        [SerializeField]
        private string m_PostName;
        [SerializeField]
        private Vector3 m_Post;
        public string PostName => m_PostName;
        public Vector3 Post => m_Post;
    }
    public partial class PlayerCamera : Singleton<PlayerCamera>
    {
        [SerializeField]
        private List<CameraPostSet> m_CameraPostSets = new();
        private bool m_StayFollow;
        [SerializeField]
        private UnityEvent<bool> m_OnSetStayFollow = new();
        [SerializeField]
        private UnityEvent<Vector3> m_OnSetOffsite = new();
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
            OnSetOffsite(set);
        }
        public void SetOffSite(string postName)
        {
            Vector3 post = GetCameraPostSet(postName).Post;
            OnSetOffsite(post);
        }
    }
}
