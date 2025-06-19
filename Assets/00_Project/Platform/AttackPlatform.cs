using UnityEngine;

namespace LegionKnight
{
    public class AttackPlatform : Platform
    {
        [SerializeField]
        private float m_OffsiteOveride = 100f; // Override the offsite value for the attack platform
        [SerializeField]
        private SpriteRenderer m_Image;

        public float OffsiteOveride => m_OffsiteOveride;
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
            if (IsRight())
            {
                m_OffsiteOveride *= 1;
            }
            else
            {
                m_OffsiteOveride *= -1 ;
            }
            Vector2 finalDesti = new Vector2(set.x + m_OffsiteOveride, set.y);
            base.SetDestination(finalDesti);
            //m_Destination = GetContactPosition();
        }
    }
}
