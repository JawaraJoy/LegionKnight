using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class InputNamePanel : PanelView
    {
        [SerializeField] private TMP_InputField m_InputName;
        [SerializeField] private Button m_ConfirmButton;
        [SerializeField] private Button m_CancelButton;
        [SerializeField] private TMP_Text m_Status;
        private bool m_Submitting;

        private void Awake()
        {
            m_InputName.characterLimit = Player.MaxPlayerNameLength;
            m_InputName.onValueChanged.AddListener(RefreshValidation);
            m_ConfirmButton.onClick.AddListener(ConfirmName);
            m_CancelButton.onClick.AddListener(HideInternal);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            m_InputName.SetTextWithoutNotify(Player.Instance.PlayerName ?? "");
            RefreshValidation(m_InputName.text);
        }

        private void RefreshValidation(string text)
        {
            bool valid = Player.IsValidPlayerName(text?.Trim());
            m_ConfirmButton.interactable = !m_Submitting && valid;
            if (m_Status != null) m_Status.text = valid ? "" : "Enter 1–20 characters without spaces.";
        }

        private async void ConfirmName()
        {
            if (m_Submitting || !m_ConfirmButton.interactable) return;
            m_Submitting = true;
            m_InputName.interactable = m_CancelButton.interactable = m_ConfirmButton.interactable = false;
            if (m_Status != null) m_Status.text = "Saving…";
            string error = null;
            try { await Player.Instance.RenamePlayerAsync(m_InputName.text); }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Profile rename failed: {ex.Message}");
                error = "Couldn’t save your name. Check your connection or try another name.";
            }
            if (!this) return;
            m_Submitting = false;
            m_InputName.interactable = m_CancelButton.interactable = true;
            RefreshValidation(m_InputName.text);
            if (error == null) HideInternal();
            else if (m_Status != null) m_Status.text = error;
        }
    }
}
