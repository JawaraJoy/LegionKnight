using LegionKnight.Prototype;
using UnityEngine;

namespace Rush
{ 
    public partial class PatchNoteManager : MonoBehaviour
    {
        [SerializeField]
        private MailDefinition m_CurrentPatchNote;
        public MailDefinition CurrentPatchNote => m_CurrentPatchNote;
    }
}

namespace LegionKnight
{
    using Rush;
    public partial class GameManager
    {
        [SerializeField]
        private PatchNoteManager m_PatchNoteManager;
        public PatchNoteManager PatchNoteManager => m_PatchNoteManager;
    }
}