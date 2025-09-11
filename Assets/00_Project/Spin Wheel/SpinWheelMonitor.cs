using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class SpinWheelMonitor : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_FreeDrawAmountText;
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
        private Button m_FreeSpinButton;
        [SerializeField]
        private Button m_FreeWatchSpinButton;
        private void Start()
        {
            m_FreeSpinButton.onClick.RemoveAllListeners();
            m_FreeSpinButton.onClick.AddListener(FreeSpin);

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
        private void ShowClaimedDailyReward(LootDefinition loot)
        {
            m_LootMonitor.ClearAllLootViews();
            m_LootMonitor.AddLootsView(loot.LootFields.ToList());
            m_LootMonitor.Show();
        }
        private void Spin()
        {
            GetSpinWheelManagerInternal().Spin();
        }
        private void FreeSpin()
        {
            GetSpinWheelManagerInternal().FreeSpin(UpdateFreeWatchDrawTextInternal);
        }
        private void FreeWatchSpin()
        {
            GetSpinWheelManagerInternal().FreeWatchSpin(UpdateFreeWatchDrawTextInternal);
        }

        private void UpdateFreeWatchDrawTextInternal()
        {
            SpinWheelManager manager = GetSpinWheelManagerInternal();
            int freeDrawAmount = manager.FreeDraw;
            int maxFreeDrawAmount = manager.Definition.FreeDrawAmount;

            int freeWatchDrawAmount = manager.FreeDrawWatch;
            int maxFreeWatchDrawAmount = manager.Definition.FreeDrawWatchAmount;

            string freeDrawText = $"{freeDrawAmount}/{maxFreeDrawAmount}";
            string freeWatchDrawText = $"{freeWatchDrawAmount}/{maxFreeWatchDrawAmount}";

            m_FreeDrawAmountText.text = freeDrawText;
            m_FreeDrawWatchAmountText.text = freeWatchDrawText;
        }
    }
}
