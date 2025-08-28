using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class LootField
    {
        [SerializeField]
        private ScriptableObject m_Item;
        [SerializeField]
        private bool m_IsUnique;
        [SerializeField]
        private int m_Amount;
        [SerializeField, Range(0f, 1f)]
        private float m_Chance;

        public ScriptableObject Item => m_Item;
        public bool IsUnique => m_IsUnique;
        public int Amount => m_Amount;
        public float Chance => m_Chance;

        public LootField(ScriptableObject item, bool isUnique, int amount, float chance)
        {
            m_Item = item;
            m_IsUnique = isUnique;
            m_Amount = amount;
            m_Chance = chance;
        }

        public void SetAmount(int amount)
        {
            m_Amount = amount;
        }
        public void AddAmount(int amount)
        {
            m_Amount += amount;
        }
    }
}
