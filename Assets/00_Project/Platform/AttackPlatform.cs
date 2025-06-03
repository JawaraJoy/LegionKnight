using UnityEngine;

namespace LegionKnight
{
    public class AttackPlatform : Platform
    {
        [SerializeField]
        private SpriteRenderer m_Image;

        protected override bool IsReachedInternal()
        {
            return false; // AttackPlatform does not reach a destination like other platforms
        }
        protected override Vector2 GetContactPosition()
        {
            return GameManager.Instance.GetPlatformDestination().position;
        }
        private bool IsRight()
        {
            return GameManager.Instance.GetOffsideDestination() > 0f;
        }

        public override void SetStartPosition(Transform set)
        {
            base.SetStartPosition(set);
            if (m_Image != null)
            {
                m_Image.flipX = IsRight();
            }
            else
            {
                Debug.LogWarning("Image is not assigned in AttackPlatform.");

            }
        }
        public override void SetDestination(Vector2 set)
        {
            base.SetDestination(set);
            m_Destination = GetContactPosition();
        }
    }
}
