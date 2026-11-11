using UnityEngine;

namespace Rush
{
    public class RushPlayerUnitSpineAgent : MonoBehaviour
    {
        private AvatarSpine m_AvatarSpine;
        private AvatarSpine AvatarSpine
        {
            get
            {
                if (m_AvatarSpine == null)
                {
                    Unit playerUnit = RushPlayer.Instance.Unit;
                    if (playerUnit.HasBind(out AvatarSpine avatarSpine))
                    {
                        m_AvatarSpine = avatarSpine;
                    }
                }
                return m_AvatarSpine;
            }
        }
        public void PlayClip(AnimationClipConfig clipConfig)
        {
            AvatarSpine.PlayClip(clipConfig);
        }
        public void PlayClipInterrupt(AnimationClipConfig clipConfig)
        {
            AvatarSpine.PlayClipInterrupt(clipConfig);
        }
        public void FlipX(bool left)
        {
            AvatarSpine.FlipX(left);
        }
        public void Pause()
        {
            AvatarSpine.Pause();
        }

        public void Resume()
        {
            AvatarSpine.Resume();
        }
        public void SetSkin(string skinName)
        {
            AvatarSpine.SetSkin(skinName);
        }
        public void QueueClip(AnimationClipConfig clipConfig)
        {
            AvatarSpine.QueueClip(clipConfig);
        }
    }
}
