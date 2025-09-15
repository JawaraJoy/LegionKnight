using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight.Dialogue
{
    [System.Serializable]
    public class Dialogue
    {
        
        [SerializeField]
        private string m_OwnerName;
        [SerializeField]
        private bool m_IsOver;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private UnityEvent m_OnDialogueStart;
        [SerializeField]
        private UnityEvent m_OnDialogueEnd;

        public string OwnerName => m_OwnerName;
        public bool IsOver => m_IsOver;
        public string Description => m_Description;
        public UnityEvent OnDialogueStart => m_OnDialogueStart;
        public UnityEvent OnDialogueEnd => m_OnDialogueEnd;
    }
}
