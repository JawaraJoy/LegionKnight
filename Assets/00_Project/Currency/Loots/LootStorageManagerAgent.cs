using UnityEngine;

namespace LegionKnight
{
    public class LootStorageManagerAgent : MonoBehaviour
    {
        public void TakeLoots()
        {
            GameManager.Instance.TakeLooteds();
        }
        public void AddLoots(LootField[] loots)
        {
            GameManager.Instance.AddLoots(loots);
        }
        public void AddLoot(LootField loot)
        {
            GameManager.Instance.AddLoot(loot);
        }
        public void ClearLoots()
        {
            GameManager.Instance.ClearLoots();
        }
    }
}
