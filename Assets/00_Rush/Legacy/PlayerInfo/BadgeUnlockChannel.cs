using System;
using LegionKnight;
using UnityEngine;

namespace Rush
{
    public sealed class BadgeUnlockChannel : ScriptableObject
    {
        public event Action<BadgeContent, int> Earned;

        public void NotifyEarned(BadgeContent badge, int tier) => Earned?.Invoke(badge, tier);
    }
}
