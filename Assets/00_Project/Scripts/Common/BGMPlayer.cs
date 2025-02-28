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

        public void PlayBGM()
        {
            m_BGMPlayer.Play(true);
        }
        public void PlayBGM(AudioClip clip)
        {
            m_BGMPlayer.Play(clip, true);
        }
        public void StopBGM()
        {
            m_BGMPlayer.Stop();
        }
    }
}
