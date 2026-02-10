using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Beam", menuName = "Rush/Combat/Ammo/Beam")]
    public class BeamConfig : AmmoConfig
    {
        [Header("Beam Shape")]
        [SerializeField] private float m_MaxLength = 10f;
        [SerializeField] private float m_Width = 0.2f;

        [Header("Beam Animation")]
        [SerializeField] private bool m_AnimateWidth = true;
        [SerializeField] private float m_ExpandTime = 0.05f;
        [SerializeField] private float m_ShrinkTime = 0.05f;

        [Header("Beam Collision")]
        [SerializeField] private bool m_Piercing = false;
        [SerializeField] private float m_DamageInterval = 0.2f;

        public float MaxLength => m_MaxLength;
        public float Width => m_Width;
        public bool AnimateWidth => m_AnimateWidth;
        public float ExpandTime => m_ExpandTime;
        public float ShrinkTime => m_ShrinkTime;
        public bool Piercing => m_Piercing;
        public float DamageInterval => m_DamageInterval;
    }
}
