using UnityEngine;

namespace LegionKnight
{
    public class SpinWheelManager : SpinWheel
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private SpinWheelManager m_SpinWheelManager;
        public SpinWheelManager SpinWheelManager => m_SpinWheelManager;
    }
}
