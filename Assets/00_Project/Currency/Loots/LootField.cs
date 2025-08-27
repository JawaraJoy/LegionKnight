using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class LootField
    {
        [SerializeField]
        private ScriptableObject m_Item;
        [SerializeField]
        private bool m_Unique;
        [SerializeField]
        private int m_Amount;
        [SerializeField, Range(0f, 1f)]
        private float m_Chance;

        public ScriptableObject Item => m_Item;
        public bool IsUnique => m_Unique;
        public int Amount => m_Amount;
        public float Chance => m_Chance;
    }
}
