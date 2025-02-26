using UnityEngine;

namespace LegionKnight
{
    public partial class BGMPlayer : AudioPlayer
    {
        
    }

    public partial class AudioHandler
    {
        [SerializeField]
        private BGMPlayer m_BGMPlayer;

        private void Play(bool loop)
        {
            m_BGMPlayer.Play(loop);
        }
        private void Play(AudioClip clip, bool loop)
        {
            m_BGMPlayer.Play(clip, loop);
        }
        public void PlayBGM()
        {
            Play(true);
        }
        public void PlayBGM(AudioClip clip)
        {
            Play(clip, true);
        }
    }
}
