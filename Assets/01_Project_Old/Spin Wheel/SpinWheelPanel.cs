// SpinWheelPanel.cs  (replaces SpinWheelMonitor — inherits PanelView like the rest of your UI)
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class SpinWheelPanel : PanelView
    {
        // ── HUD ──────────────────────────────────────────────────────────────────

        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI m_SpinAmountText;
        [SerializeField] private TextMeshProUGUI m_FreeWatchAmountText;

        // ── Reward grid ───────────────────────────────────────────────────────────

        [Header("Reward grid")]
        [SerializeField] private SpinRewardView[] m_RewardViews;   // assign 8 in Inspector
        [SerializeField] private SpinRewardView m_SelectedRewardPreview;

        // ── Loot result ───────────────────────────────────────────────────────────

        [Header("Loot result")]
        [SerializeField] private LootMonitor m_LootMonitor;

        // ── Buttons ───────────────────────────────────────────────────────────────

        [Header("Buttons")]
        [SerializeField] private Button m_SpinButton;
        [SerializeField] private Button m_FreeWatchButton;
        [SerializeField] private Button m_CloseButton;

        // ── Internal ──────────────────────────────────────────────────────────────

        private SpinWheelManager Manager => Player.Instance.SpinWheelManager;

        // ── Lifecycle ─────────────────────────────────────────────────────────────

        private void Start()
        {
            m_SpinButton.onClick.AddListener(OnSpinClicked);
            m_FreeWatchButton.onClick.AddListener(OnFreeWatchClicked);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
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

        // ── SpinWheel event receivers (wire these up in Inspector) ────────────────

        /// <summary>Wire to SpinWheel.m_OnStepChanged</summary>
        public void OnStepChanged(SpinRewardDefinition reward)
        {
            HighlightReward(reward);
            m_SelectedRewardPreview.Init(reward);
        }

        /// <summary>Wire to SpinWheel.m_OnSpinEnd</summary>
        public void OnSpinEnd()
        {
            SetButtonsBusy(false);
            RefreshButtonState();
        }

        /// <summary>Wire to SpinWheel.m_OnClaim</summary>
        public void OnClaim(SpinRewardDefinition reward)
        {
            ShowLootResult(reward);
            RefreshHUD();
            RefreshButtonState();
        }

        // ── Reward highlighting ───────────────────────────────────────────────────

        private void HighlightReward(SpinRewardDefinition reward)
        {
            foreach (var view in m_RewardViews)
                view.SetSelected(view.Definition == reward ? reward : null);
        }

        // ── Loot result ───────────────────────────────────────────────────────────

        private void ShowLootResult(SpinRewardDefinition reward)
        {
            m_LootMonitor.ClearAllLootViews();
            m_LootMonitor.Show();
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
            bool idle = !Manager.IsBusy && Manager.SelectedReward == null;
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