using Firebase.Analytics;
using UnityEngine;

namespace Rush
{
    public partial class AnalyticService : Singleton<AnalyticService>
    {
        [SerializeField]
        private FirebaseAnalytic m_FirebaseAnalytic;
        // wrap other analytic component here
        // misalnya kaya tenjin dll

        protected override void Awake()
        {
            base.Awake();
            m_FirebaseAnalytic.Init();
            // init other analytic here
            // setelah buat componentnya, init di sini
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

        // kalo bisa pake ini saja, karena global bisa tulis apa saja
        public void CustomEvent(string eventName, params Parameter[] parameters)
        {
            AnalyticWrapper.LogEvent(eventName, parameters);
        }

        // klo function kurang bisa tambah lagi disini
    }
}
