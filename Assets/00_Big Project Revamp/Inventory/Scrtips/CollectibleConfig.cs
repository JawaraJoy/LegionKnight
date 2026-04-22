
using LegionKnight;
using UnityEngine;

namespace Rush
{
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
