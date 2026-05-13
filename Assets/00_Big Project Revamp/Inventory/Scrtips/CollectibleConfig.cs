
using LegionKnight;
using UnityEngine;

namespace Rush
{
    // the base configuration for all collectible items, such as cards, equipments, materials, etc. it contains the basic info of the collectible item, such as id, name and description, and the collectible field which defines the type of the collectible item
    public abstract partial class CollectibleConfig : Configuration
    {
        [SerializeField]
        private CollectibleField m_CollectibleField;
        public CollectibleField CollectibleField => m_CollectibleField;

        public void OnCollect(string source, int amount)
        {
            TenjinManager.Instance.SendEvent("Collected",$"{m_BaseInfo.Id}x{amount} from {source}");
        }
    }
}
