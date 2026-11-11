using UnityEngine;

namespace Rush
{
    public class RigidBodyModifier : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D m_Rb;

private void FreezeConstrain(RigidbodyConstraints2D constraints)
        {
            if (!TryResolveRigidbody())
            {
                return;
            }

            m_Rb.constraints = constraints;
        }

private bool TryResolveRigidbody()
        {
            if (m_Rb != null)
            {
                return true;
            }

            if (TryGetComponent(out m_Rb))
            {
                return true;
            }

            Debug.LogWarning($"Rigidbody2D component not found on {gameObject.name}", this);
            return false;
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
            if (!TryResolveRigidbody())
            {
                return;
            }

            m_Rb.freezeRotation = set;
        }
    }
}
