using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight.Dialogue
{
    public class DialogueDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_OwnerName;
        [SerializeField]
        private string m_Description;
        [SerializeField]
        private UnityEvent m_Action;

        public string OwnerName => m_OwnerName;
        public string Description => m_Description;
        public UnityEvent Action => m_Action;
    }
}
