
using LegionKnight;
using UnityEngine;

namespace Rush
{
    // the base configuration for all collectible items, such as cards, equipments, materials, etc. it contains the basic info of the collectible item, such as id, name and description, and the collectible field which defines the type of the collectible item
    public abstract partial class CollectibleConfig : Configuration
    {
        [SerializeField]
        protected CollectibleField m_CollectibleField;
        [SerializeField]
        private Currency m_SellConfig;
        public CollectibleField CollectibleField => m_CollectibleField;

        public void OnCollect(string source, int amount)
        {
            TenjinManager.Instance.SendEvent("Collected",$"{m_BaseInfo.Id}x{amount} from {source}");

            AnalyticService.Instance.ItemCollected(source, m_BaseInfo.Name, amount);
        }
        public Currency GetSellValue(int amount)
        {
            float rarityRate = m_CollectibleField.RarityConfig.ValueRate;
            int sellValue = m_SellConfig.Amount;

            int totaValue = Mathf.RoundToInt(sellValue * rarityRate);
            totaValue *= amount;

            Currency finalSellValue = new Currency(m_SellConfig.ItemConfig, totaValue);
            return finalSellValue;
        }
    }
}
