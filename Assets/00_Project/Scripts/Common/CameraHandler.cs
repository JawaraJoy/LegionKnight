using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace LegionKnight
{
    public partial class CameraHandler : MonoBehaviour
    {
        private Transform m_Camera;
        [SerializeField]
        private Vector3 m_Offset;
        [Range(1, 10)]
        public float m_SmoothFactor;

        private void Start()
        {
            m_Camera = Camera.main.transform;
        }
        public void SetCamera()
        {
            m_Camera = Camera.main.transform;
        }
        public void RemoveCamera()
        {
            m_Camera = null;
        }
        private void LateUpdate()
        {
            Follow();
        }
        private void Follow()
        {
            if (m_Camera == null) return;
            Vector3 targetPosition = transform.position + m_Offset;
            Vector3 smoothPosition = Vector3.Lerp(m_Camera.position, targetPosition, m_SmoothFactor * Time.deltaTime);
            m_Camera.position = smoothPosition;
        }
        public void SetOffsite(Vector3 set)
        {
            m_Offset = set;
        }
    }
}
