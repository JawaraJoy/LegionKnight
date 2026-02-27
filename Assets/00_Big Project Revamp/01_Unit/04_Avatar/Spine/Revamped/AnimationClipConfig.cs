using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Animation Clip", menuName = "Rush/Spine/Animation")]
    public partial class AnimationClipConfig : Configuration
    {
        [SerializeField]
        private int m_Track = 0;
        [SerializeField]
        private float m_Delay = 0f;
        [SerializeField]
        private float m_SpeedRate = 1f;
        [SerializeField]
        private bool m_Loop;
        [SerializeField]
        private AnimationClipConfig m_NextAnimation;
        public int Track => m_Track;
        public float Delay => m_Delay;
        public float SpeedRate => m_SpeedRate;
        public bool Loop => m_Loop;
        public AnimationClipConfig NextAnimation => m_NextAnimation;
    }
}
