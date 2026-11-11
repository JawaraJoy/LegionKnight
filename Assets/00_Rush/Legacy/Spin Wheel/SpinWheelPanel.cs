// SpinWheelPanel.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class SpinWheelPanel : PanelView
    {
        // ── HUD ───────────────────────────────────────────────────────────────────

        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI m_SpinAmountText;
        [SerializeField] private TextMeshProUGUI m_FreeWatchAmountText;

        // ── Reward grid ───────────────────────────────────────────────────────────
        // Urutan array ini HARUS sama dengan urutan m_Definition.Rewards di SpinWheelDefinition.
        // Index 0 di sini = index 0 di Rewards[], dst.

        [Header("Reward grid")]
        [SerializeField] private SpinRewardView[] m_RewardViews;
        [SerializeField] private SpinRewardView m_SelectedRewardPreview;

        // ── Buttons ───────────────────────────────────────────────────────────────

        [Header("Buttons")]
        [SerializeField] private Button m_SpinButton;
        [SerializeField] private Button m_FreeWatchButton;
        [SerializeField] private Button m_CloseButton;

        // ── Internal ──────────────────────────────────────────────────────────────

        private SpinWheelManager Manager => Player.Instance.SpinWheelManager;

        private int m_CurrentHighlightIndex = -1;

        // ── Lifecycle ─────────────────────────────────────────────────────────────

        private void Start()
        {
            m_SpinButton.onClick.AddListener(OnSpinClicked);
            m_FreeWatchButton.onClick.AddListener(OnFreeWatchClicked);
            Manager.OnStepChanged.AddListener(OnStepChanged);
            Manager.OnSpinEnd.AddListener(OnSpinEnd);
            Manager.OnClaim.AddListener(OnClaim);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            ClearHighlight();
            RefreshHUD();
            RefreshButtonState();
        }

        // ── Button handlers ───────────────────────────────────────────────────────

        private void OnSpinClicked()
        {
            Manager.TrySpin(OnAfterSpinConsumed);
        }

        private void OnFreeWatchClicked()
        {
            Manager.TryFreeWatchSpin(OnAfterSpinConsumed);
        }

        private void OnAfterSpinConsumed()
        {
            RefreshHUD();
            SetButtonsBusy(true);
        }

        // ── SpinWheel event receivers (wire di Inspector) ─────────────────────────

        /// <summary>
        /// Wire ke SpinWheel.m_OnStepChanged (int index, SpinRewardDefinition reward).
        /// Highlight view di index tersebut, matikan sisanya.
        /// </summary>
        private void OnStepChanged(int index, SpinRewardDefinition reward)
        {
            // Matikan highlight sebelumnya
            if (m_CurrentHighlightIndex >= 0 && m_CurrentHighlightIndex < m_RewardViews.Length)
                m_RewardViews[m_CurrentHighlightIndex].SetHighlight(false);

            // Nyalakan highlight di index baru
            m_CurrentHighlightIndex = index;
            if (m_CurrentHighlightIndex >= 0 && m_CurrentHighlightIndex < m_RewardViews.Length)
                m_RewardViews[m_CurrentHighlightIndex].SetHighlight(true);

            // Update preview tengah
            if (m_SelectedRewardPreview != null)
                m_SelectedRewardPreview.Init(reward);
        }

        /// <summary>Wire ke SpinWheel.m_OnSpinEnd</summary>
        private void OnSpinEnd()
        {
            SetButtonsBusy(false);
            RefreshButtonState();
        }

        /// <summary>Wire ke SpinWheel.m_OnClaim</summary>
        private void OnClaim(SpinRewardDefinition reward)
        {
            ClearHighlight();
            RefreshHUD();
            RefreshButtonState();
            
            Debug.Log($"Claimed reward: {reward.DisplayName} x{reward.Amount}");
        }

        // ── Highlight helpers ─────────────────────────────────────────────────────

        private void ClearHighlight()
        {
            foreach (var view in m_RewardViews)
                view.SetHighlight(false);
            m_CurrentHighlightIndex = -1;
        }

        // ── HUD & button state ────────────────────────────────────────────────────

        private void RefreshHUD()
        {
            int tickets = Manager.SpinDraw.Amount;
            int freeWatch = Manager.FreeDrawWatch;
            int maxFreeWatch = Manager.Definition.FreeDrawWatchAmount;

            m_SpinAmountText.text = $"x{tickets}";
            m_FreeWatchAmountText.text = $"{freeWatch}/{maxFreeWatch}";
        }

        private void RefreshButtonState()
        {
            bool idle = !Manager.IsBusy;
            m_SpinButton.interactable = idle && Manager.SpinDraw.Amount > 0;
            m_FreeWatchButton.interactable = idle && Manager.FreeDrawWatch > 0;
            m_CloseButton.interactable = idle;
        }

        private void SetButtonsBusy(bool busy)
        {
            m_SpinButton.interactable = !busy;
            m_FreeWatchButton.interactable = !busy;
            m_CloseButton.interactable = !busy;
        }
    }
}