using UnityEngine;

namespace Rush
{
    public class GachaManager : GachaHandler
    {
        
    }

    // ini adalah singleton
    // kamu bisa call dengan cara RushPlayer.Instance.GachaManager
    public partial class RushPlayer
    {
        [SerializeField]
        private GachaManager m_GachaManager;

        public GachaManager GachaManager => m_GachaManager;
    }
}
