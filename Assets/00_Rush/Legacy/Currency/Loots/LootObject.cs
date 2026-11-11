using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LootObject : MonoBehaviour
    {
        [SerializeField]
        private LootChestDefinition m_LootDefinition;

        [SerializeField]
        private UnityEvent<LootField[]> m_OnLooted;

        public void Loots()
        {
            if (m_LootDefinition == null)
            {
                Debug.LogWarning("LootDefinition is not assigned.");
                return;
            }
            var loots = m_LootDefinition.GetRandomLoots();
            m_OnLooted?.Invoke(loots.ToArray());
        }
        public void LootOne()
        {
            if (m_LootDefinition == null)
            {
                Debug.LogWarning("LootDefinition is not assigned.");
                return;
            }
            var loots = m_LootDefinition.GetRandomLoots();
            if (loots.Count > 0)
            {
                m_OnLooted?.Invoke(new LootField[] { loots[0] });
            }
        }
    }
}
