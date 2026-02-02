using UnityEngine;

namespace Rush
{ 
    public partial class PatchNoteManager : MonoBehaviour
    {
        [SerializeField]
        private PatchNoteConfig m_CurrentPatchNote;
        public PatchNoteConfig CurrentPatchNote => m_CurrentPatchNote;
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