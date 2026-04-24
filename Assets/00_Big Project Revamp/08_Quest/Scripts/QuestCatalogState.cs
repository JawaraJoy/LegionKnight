using System;

namespace Rush
{
    public class QuestCatalogState
    {
        private readonly QuestCatalogConfig m_Config;
        private readonly DateTime m_NextResetTime;

        public QuestCatalogConfig Config => m_Config;
        public DateTime NextResetTime => m_NextResetTime;

        public double SecondsUntilReset =>
            Math.Max(0, (m_NextResetTime - DateTime.Now).TotalSeconds);

        public QuestCatalogState(QuestCatalogConfig config, DateTime nextResetTime)
        {
            m_Config = config;
            m_NextResetTime = nextResetTime;
        }
    }
}