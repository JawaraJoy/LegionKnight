using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class ExplodeSetupField
    {
        [SerializeField]
        private bool m_ExplodeOnHit = false;
        [SerializeField]
        private float m_ExplosionRadius = 5f;
        public bool ExplodeOnHit => m_ExplodeOnHit;
        public float ExplosionRadius => m_ExplosionRadius;
    }
    public interface IExplodeable
    {
        bool ExplodeOnHit { get; }
        float ExplosionRadius { get; }
    }
}
