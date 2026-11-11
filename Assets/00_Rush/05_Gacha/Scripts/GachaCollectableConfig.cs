using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "GachaCollect_", menuName = "Rush/Gacha/Collectable")]
    public class GachaCollectableConfig : ScriptableObject
    {
        [SerializeField]
        private CollectibleConfig m_Collect;
        [SerializeField]
        private int m_Amount;
        [SerializeField, Range(0f, 1f)]
        private float m_Chance = 1f;

        public CollectibleConfig Collect => m_Collect;
        public int Amount => m_Amount;
        public float Chance => m_Chance;
    }
}
