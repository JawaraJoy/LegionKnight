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
        public BGMPlayer BGMPlayer => m_BGMPlayer;

        
    }
}
