using UnityEngine;

namespace LegionKnight
{
    public class ParrralaxInfinite : MonoBehaviour
    {
        public float parallaxMultiplierX = 0.5f; // Speed for X-axis
        public float parallaxMultiplierY = 0.5f; // Speed for Y-axis
        private Transform cam;
        private Vector3 lastCamPosition;
        private float textureUnitSizeX, textureUnitSizeY;

        void Start()
        {
            cam = Camera.main.transform;
            lastCamPosition = cam.position;

            // Get sprite width & height for repositioning
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            textureUnitSizeX = spriteRenderer.bounds.size.x;
            textureUnitSizeY = spriteRenderer.bounds.size.y;
        }

        void LateUpdate()
        {
            // Move background based on camera movement
            Vector3 deltaMovement = cam.position - lastCamPosition;
            transform.position += new Vector3(deltaMovement.x * parallaxMultiplierX, deltaMovement.y * parallaxMultiplierY, 0);
            lastCamPosition = cam.position;

            // Check if the background moved out of view and reposition
            float camLeftEdge = cam.position.x - Camera.main.orthographicSize * Camera.main.aspect;
            float camRightEdge = cam.position.x + Camera.main.orthographicSize * Camera.main.aspect;
            float camBottomEdge = cam.position.y - Camera.main.orthographicSize;
            float camTopEdge = cam.position.y + Camera.main.orthographicSize;

            // Reposition horizontally (X)
            if (transform.position.x + textureUnitSizeX / 2 < camLeftEdge)
            {
                transform.position += new Vector3(textureUnitSizeX * 2, 0, 0);
            }
            else if (transform.position.x - textureUnitSizeX / 2 > camRightEdge)
            {
                transform.position -= new Vector3(textureUnitSizeX * 2, 0, 0);
            }

            // Reposition vertically (Y)
            if (transform.position.y + textureUnitSizeY / 2 < camBottomEdge)
            {
                transform.position += new Vector3(0, textureUnitSizeY * 2, 0);
            }
            else if (transform.position.y - textureUnitSizeY / 2 > camTopEdge)
            {
                transform.position -= new Vector3(0, textureUnitSizeY * 2, 0);
            }
        }
    }
}
