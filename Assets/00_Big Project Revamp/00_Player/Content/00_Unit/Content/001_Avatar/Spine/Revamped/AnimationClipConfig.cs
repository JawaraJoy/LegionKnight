using LegionKnight;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Animation Clip", menuName = "Rush/Spine/Animation")]
    public partial class AnimationClipConfig : Configuration
    {
        [SerializeField]
        private bool m_Loop;
        [SerializeField]
        private AnimationClipConfig m_NextAnimation;
        public bool Loop => m_Loop;
        public AnimationClipConfig NextAnimation => m_NextAnimation;
    }
}
