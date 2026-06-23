using LegionKnight;
using LegionKnight.Prototype;
using TMPro;
using UnityEngine;
namespace Rush
{
    public partial class PatchNotePanel : PanelView
    {
        [SerializeField]
        private TextMeshProUGUI m_TitlePatchText;
        [SerializeField]
        private TextMeshProUGUI m_DescriptionPatchText;

        private MailDefinition m_CurrentPatchNote;

        private MailDefinition CurrentPatchNoteInternal
        {
            get
            {
                if (m_CurrentPatchNote == null)
                {
                    m_CurrentPatchNote = GameManager.Instance.PatchNoteManager.CurrentPatchNote;
                }
                return m_CurrentPatchNote;
            }
        }
        private void ShowPatchNoteInternal(MailDefinition patchNote)
        {
            m_TitlePatchText.text = patchNote.Label;
            m_DescriptionPatchText.text = patchNote.Description;
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            ShowPatchNoteInternal(CurrentPatchNoteInternal);
        }

        public void Refresh()
        {
            ShowPatchNoteInternal(CurrentPatchNoteInternal);
        }
    }
}
