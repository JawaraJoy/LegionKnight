using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "HookQuest_", menuName = "Rush/Quest/HookQuest", order = 0)]
    public class HookQuest : ScriptableObject
    {
        [SerializeField]
        private QuestTaskConfig[] m_QuestTasks;

        public void AddTaskCount(int amount)
        {
            foreach (QuestTaskConfig questTask in m_QuestTasks)
            {
                questTask.AddTaskCount(amount);
            }
        }

    }
}
