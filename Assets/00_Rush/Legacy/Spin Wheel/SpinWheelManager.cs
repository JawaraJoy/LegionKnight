// SpinWheelManager.cs
using UnityEngine;

namespace LegionKnight
{
    /// <summary>
    /// Thin subclass — lets Player hold a typed reference.
    /// Add game-specific overrides here if needed later.
    /// </summary>
    public class SpinWheelManager : SpinWheel { }

    public partial class Player
    {
        [SerializeField]
        private SpinWheelManager m_SpinWheelManager;
        public SpinWheelManager SpinWheelManager => m_SpinWheelManager;
    }
}