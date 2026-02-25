using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class SpinWheelMonitor : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_SpinAmountText;
        [SerializeField]
        private TextMeshProUGUI m_FreeDrawWatchAmountText;

        [SerializeField]
        private LootMonitor m_LootMonitor;

        [SerializeField]
        private SpinRewardView[] m_Rewards;
        [SerializeField]
        private SpinRewardView m_SelectedRewardView;
        private SpinWheelManager m_SpinWheelManager;


        [SerializeField]
        private Button m_SpinButton;
        [SerializeField]
        private Button m_FreeWatchSpinButton;
        [SerializeField]
        private Button m_CloseButton;
        private void Start()
        {
            m_SpinButton.onClick.RemoveAllListeners();
            m_SpinButton.onClick.AddListener(Spin);

            m_FreeWatchSpinButton.onClick.RemoveAllListeners();
            m_FreeWatchSpinButton.onClick.AddListener(FreeWatchSpin);
        }
        private SpinRewardView GetSpinRewardView(string id)
        {
            foreach (var reward in m_Rewards) 
            {
                if (reward.Definition.Id == id)
                {
                    return reward;
                }
            }
            return null;
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            UpdateFreeWatchDrawTextInternal();
        }
        private SpinWheelManager GetSpinWheelManagerInternal()
        {
            if (m_SpinWheelManager == null)
            {
                m_SpinWheelManager = Player.Instance.SpinWheelManager;
            }
            return m_SpinWheelManager;
        }
        public void SetSelected(SpinRewardDefinition selectedDefi)
        {
            foreach (var item in m_Rewards)
            {
                item.SetSelected(selectedDefi);
            }
            m_SelectedRewardView.Init(selectedDefi);
        }
        public void Claim()
        {
            SpinWheelManager manager = GetSpinWheelManagerInternal();
            ShowClaimedDailyReward(manager.SelectedReward.Rewards);
        }
        private void ShowClaimedDailyReward(LootChestDefinition loot)
        {
            m_LootMonitor.ClearAllLootViews();
            m_LootMonitor.AddLootsView(loot.LootFields.ToList());
            m_LootMonitor.Show();
        }
        private void Spin()
        {
            GetSpinWheelManagerInternal().Spin(UpdateFreeWatchDrawTextInternal);
        }
        private void FreeWatchSpin()
        {
            GetSpinWheelManagerInternal().FreeWatchSpin(UpdateFreeWatchDrawTextInternal);
        }

        private void UpdateFreeWatchDrawTextInternal()
        {
            SpinWheelManager manager = GetSpinWheelManagerInternal();
            int spinDrawAmount = manager.SpinDraw.Amount;

            int freeWatchDrawAmount = manager.FreeDrawWatch;
            int maxFreeWatchDrawAmount = manager.Definition.FreeDrawWatchAmount;

            string SpinDrawText = $"x{spinDrawAmount}";
            string freeWatchDrawText = $"{freeWatchDrawAmount}/{maxFreeWatchDrawAmount}";

            m_SpinAmountText.text = SpinDrawText;
            m_FreeDrawWatchAmountText.text = freeWatchDrawText;
        }

        public void BusyButtons(bool active)
        {
            m_FreeWatchSpinButton.interactable = active;
            m_SpinButton.interactable = active;
            m_CloseButton.interactable = active;
        }
    }
}
