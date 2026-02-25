using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rush;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Loot", menuName = "Legion Knight/Loot", order = 0)]
    public class LootChestDefinition : CollectibleConfig
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private LootField[] m_LootFields;

        public string Id => m_Id;
        public LootField[] LootFields => m_LootFields;

        private LootedPanel m_Panel;
        private LootedPanel GetPanel()
        {
            if (m_Panel == null)
            {
                m_Panel = CanvasManager.Instance.GetPanel<LootedPanel>();
            }
            return m_Panel;
        }
        private List<LootField> GetRandomLootsInternal()
        {
            List<LootField> loots = new List<LootField>();
            for (int i = 0; i < m_LootFields.Length; i++)
            {
                if (Random.value <= m_LootFields[i].Chance)
                {
                    Debug.Log($"Looted: {m_LootFields[i].ItemLoot.BaseInfo.Name} x{m_LootFields[i].Amount}");
                    loots.Add(m_LootFields[i]);
                }
            }
            return loots;
        }
        public List<LootField> GetRandomLoots()
        {
            return GetRandomLootsInternal();
        }
        public LootField GetRandomOneLoot()
        {
            var loots = GetRandomLootsInternal();
            if (loots.Count > 0)
            {
                return loots[Random.Range(0, loots.Count)];
            }
            return null;
        }

        public void DirectTakeLoots()
        {
            foreach (var loot in m_LootFields)
            {
                loot.DirectTakeLoot();
            }
        }
        public void DirectTakeLootsShow()
        {
            foreach (var loot in m_LootFields)
            {
                loot.DirectTakeLoot();
            }
            GetPanel().ShowLoot(m_LootFields);
        }
    }
    
}
