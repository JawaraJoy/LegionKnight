using LegionKnight;
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

        private PatchNoteConfig m_CurrentPatchNote;

        private PatchNoteConfig CurrentPatchNoteInternal
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
        private void ShowPatchNoteInternal(PatchNoteConfig patchNote)
        {
            m_TitlePatchText.text = patchNote.BaseInfo.Name;
            m_DescriptionPatchText.text = patchNote.BaseInfo.Description;
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
