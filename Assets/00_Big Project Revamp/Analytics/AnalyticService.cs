using UnityEngine;

namespace Rush
{
    public class AnalyticService : Singleton<AnalyticService>
    {
        [SerializeField]
        private FirebaseAnalytic m_FirebaseAnalytic;
        // wrap other analytic component here

        protected override void Awake()
        {
            base.Awake();
            m_FirebaseAnalytic.Init();
            // init other analytic here
        }
        public void HeroDefeated(string heroName, string killer)
        {
            AnalyticWrapper.LogEvent("hero_defeated", heroName, killer);
        }
        public void ItemCollected(string source, string itemName, int amount)
        {
            string nameEvent = $"{source}_{itemName}";
            AnalyticWrapper.LogEvent("item_collected", nameEvent, amount);
        }
        public void BossDefeated(string bossName, string heroAgainstName)
        {
            AnalyticWrapper.LogEvent("boss_defeated", bossName, heroAgainstName);
        }
        public void MissionCompleted(string missionName, string missionGroup)
        {
            AnalyticWrapper.LogEvent("mission_completed", missionName, missionGroup);
        }
        public void WatchAds(string platform, double revenue)
        {
            AnalyticWrapper.LogEvent("watch_ads", platform, revenue);
        }
    }
}
