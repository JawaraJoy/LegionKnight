

namespace Rush
{
    public interface IAvatarSpine
    {
        void PlayClip(AnimationClipConfig clipConfig);
        void FlipX(bool left);
        void PlayClipInterrupt(AnimationClipConfig clipConfig);
        void QueueClip(AnimationClipConfig clipConfig);
        void SetSkin(string skinName);
        void Pause();
        void Resume();
    }
}
