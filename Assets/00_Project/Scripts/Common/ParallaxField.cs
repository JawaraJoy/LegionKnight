using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public partial class ParallaxField
    {
        [Header("Parallax Settings")]
        [Tooltip("The speed at which the background moves relative to the camera.")]
        [Range(0f, 1f)] public float parallaxMultiplier = 0.5f;

        [Tooltip("Enable parallax movement on the X-axis.")]
        public bool moveOnXAxis = true;

        [Tooltip("Enable parallax movement on the Y-axis.")]
        public bool moveOnYAxis = false;

        [SerializeField]
        private Transform m_Target;
        private Transform cam;

        private Vector3 previousCamPosition;

        public void Start()
        {
            cam = Camera.main.transform;
            previousCamPosition = cam.position;
        }

        public void LateUpdate()
        {
            Vector3 deltaMovement = cam.position - previousCamPosition;

            float xMovement = moveOnXAxis ? deltaMovement.x * parallaxMultiplier : 0f;
            float yMovement = moveOnYAxis ? deltaMovement.y * parallaxMultiplier : 0f;

            m_Target.position += new Vector3(xMovement, yMovement, 0);
            previousCamPosition = cam.position;
        }
    }
}
