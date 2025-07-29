using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public class SpineEvent
    {
        [SerializeField]
        private SpineAnimDefinition m_Definition;
        [SerializeField]
        private UnityEvent m_OnStart = new();
        [SerializeField]
        private UnityEvent m_OnEnd = new();
        public SpineAnimDefinition Definition => m_Definition;
        public UnityEvent OnStart => m_OnStart;
        public UnityEvent OnEnd => m_OnEnd;
    }
}
