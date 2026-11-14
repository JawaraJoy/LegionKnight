using System;
using System.Collections.Generic;
using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Tutorial Flashes", menuName = "Rush/Tutorial/Flash Channel")]
    public sealed class TutorialFlashChannel : ScriptableObject
    {
        [NonSerialized] private readonly List<TutorFlash> m_Targets = new();
        public IReadOnlyList<TutorFlash> Targets => m_Targets;
        public event Action<TutorFlash> TargetAdded;
        public event Action<TutorFlash> TargetRemoved;
        public void Register(TutorFlash target)
        {
            if (target == null || m_Targets.Contains(target)) return;
            m_Targets.Add(target);
            TargetAdded?.Invoke(target);
        }
        public void Unregister(TutorFlash target)
        {
            if (!m_Targets.Remove(target)) return;
            TargetRemoved?.Invoke(target);
        }
    }
}
