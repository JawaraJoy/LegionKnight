using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class CanvasWorldCameraSetter : MonoBehaviour
    {
        [SerializeField]
        private Canvas m_Canvas;

        private Camera m_Camera;

        private void Start()
        {
            m_Camera = PlayerCamera.Instance.Camera;
            if (m_Canvas.renderMode == RenderMode.WorldSpace)
            {
                m_Canvas.worldCamera = m_Camera;
            }
        }
    }
}
