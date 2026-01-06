using UnityEngine;

namespace LegionKnight
{
    public class RigidBodyModifier : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D m_Rb;

        private void FreezeConstrain(RigidbodyConstraints2D constraints)
        {
            if (m_Rb == null)
            {
                m_Rb = GetComponent<Rigidbody2D>();
            }
            if (m_Rb != null)
            {
                m_Rb.constraints = constraints;
            }
            else
            {
                Debug.LogWarning("Rigidbody2D component not found on " + gameObject.name);
            }
        }

        public void FreezePositionX()
        {
            FreezeConstrain(RigidbodyConstraints2D.FreezePositionX);
        }
        public void FreezePositionY()
        {
            FreezeConstrain(RigidbodyConstraints2D.FreezePositionY);
        }
        public void FreezePosition()
        {
            FreezeConstrain(RigidbodyConstraints2D.FreezePosition);
        }
        public void FreezeRotation(bool set)
        {
            if (m_Rb != null)
            {
                m_Rb.freezeRotation = set;
            }
            else
            {
                Debug.LogWarning("Rigidbody2D component not found on " + gameObject.name);
            }
        }
    }
}
