using UnityEngine;
using LegionKnight;
using UnityEngine.UI;
using TMPro;

namespace Rush
{
    public class FakeReviewPanel : PanelView
    {
        [SerializeField]
        private Button m_ConfirmationButton;
        [SerializeField]
        private TMP_InputField m_InputReviewField;

        [SerializeField]
        private FakeStarReviewButton[] m_StarReviewButtons;
        public FakeStarReviewButton[] StarReviewButtons => m_StarReviewButtons;
        private void Start()
        {
            m_ConfirmationButton.onClick.AddListener(ApplyReview);
        }
        public void OpenReview()
        {
            foreach (var button in m_StarReviewButtons)
            {
                button.ActivateStar(false);
            }
            ShowInternal();
            SetConfirmationButtonInternal(false);
        }
        private void SetConfirmationButtonInternal(bool active)
        {
            m_ConfirmationButton.interactable = active;
        }
        public void SetConfirmationButton(bool active)
        {
            SetConfirmationButtonInternal(active);
        }
        private void ApplyReview()
        {
            RushGameManager.Instance.FakeReview.ApplyReview(m_InputReviewField.text);
            HideInternal();
        }
    }
}
