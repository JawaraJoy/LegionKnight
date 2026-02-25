using System.Linq;
using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class DailyRewardPanel : PanelView
    {
        [SerializeField]
        private LootMonitor m_PreviewLootMonitor;

        [SerializeField]
        private DailyRewardView[] m_DailyRewardViews;
        private DailyReward m_DailyReward;

        [SerializeField]
        private TextMeshProUGUI m_RestTimerText;

        private DailyReward DailyRewardInternal
        {
            get
            {
                if (m_DailyReward == null)
                {
                    m_DailyReward = GameManager.Instance.DailyRewardManager;
                }
                return m_DailyReward;
            }
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            DailyRewardInternal.Refresh();
            foreach (var view in m_DailyRewardViews)
            {
                view.Show();
            }
            TimerDefinition defi = DailyRewardInternal.Timer;
            m_RestTimerText.text = defi.GetRemainingTimeToReset();
        }
        public void ShowClaimedDailyReward(LootChestDefinition loot)
        {
            m_PreviewLootMonitor.ClearAllLootViews();
            m_PreviewLootMonitor.AddLootsView(loot.LootFields.ToList());
            m_PreviewLootMonitor.Show();
        }
    }

    public partial class CanvasManager
    {
        public DailyRewardPanel GetDailyRewardPanel()
        {
            return GetPanel<DailyRewardPanel>();
        }
    }
}
