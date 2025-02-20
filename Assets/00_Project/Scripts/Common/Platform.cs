using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    [System.Serializable]
    public class AssetReferencePlatform : AssetReferenceT<Platform>
    {
        public AssetReferencePlatform(string guid) : base(guid)
        {
        }
    }
    public partial class Platform : MonoBehaviour
    {
        private bool m_CanMove;
        [SerializeField]
        private float m_MinSpeed;
        [SerializeField]
        private float m_MaxSpeed;
        private float m_SpeedMultiplier;

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            if (!m_CanMove) return;
            transform.Translate(Vector3.right * GetFinalSpeed() * Time.deltaTime, Space.Self);
        }

        private float GetFinalSpeed()
        {
            float speed = Random.Range(m_MinSpeed, m_MaxSpeed);
            return speed * m_SpeedMultiplier;
        }

        public void StartMove(float direction)
        {
            m_SpeedMultiplier = direction;
            m_CanMove = true;
        }
        public void StopMove()
        {
            m_CanMove = false;
        }
    }
}
