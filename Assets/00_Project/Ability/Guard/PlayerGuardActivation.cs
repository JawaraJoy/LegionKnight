using UnityEngine;

namespace LegionKnight
{
    public class PlayerGuardActivation : GuardActivation
    {
        override protected void GuardActive()
        {
            Player.Instance.SetCanActive(true);
            // Implement player-specific guard activation logic here
            Player.Instance.ActiveGuard(m_GuardSprite, m_GuardDuration);
            // Example: Apply guard effect, play animation, etc.
        }
    }
}
