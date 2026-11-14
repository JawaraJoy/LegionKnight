using System;
using System.Collections.Generic;
using LegionKnight;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Tutorial Targets", menuName = "Rush/Tutorial/Target Channel")]
    public sealed class TutorialTargetChannel : ScriptableObject
    {
        [NonSerialized] private readonly List<TutorTarget> m_Targets = new();
        public IReadOnlyList<TutorTarget> Targets => m_Targets;
        public event Action<TutorTarget> TargetAdded;
        public event Action<TutorTarget> TargetRemoved;
        public void Register(TutorTarget target)
        {
            if (target == null || m_Targets.Contains(target)) return;
            m_Targets.Add(target);
            TargetAdded?.Invoke(target);
        }
        public void Unregister(TutorTarget target)
        {
            if (!m_Targets.Remove(target)) return;
            TargetRemoved?.Invoke(target);
        }
    }
}
