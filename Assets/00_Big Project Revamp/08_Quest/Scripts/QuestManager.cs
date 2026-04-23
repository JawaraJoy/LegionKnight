using UnityEngine;

namespace Rush
{
    public class QuestManager : QuestHandler { }

    public partial class RushPlayer
    {
        [SerializeField] private QuestManager m_QuestManager;
        public QuestManager QuestManager => m_QuestManager;
    }
}