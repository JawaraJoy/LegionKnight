using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class LootStorageManagerAgent : MonoBehaviour
    {
        private LootedPanel m_LootPanel;
        private LootedPanel LootedPanelInternal
        {
            get
            {
                if (m_LootPanel == null)
                {
                    m_LootPanel = CanvasManager.Instance.GetPanel<LootedPanel>();
                }
                return m_LootPanel;
            }
        }
        public void TakeLoots()
        {
            GameManager.Instance.LootStorageManager.TakeLooteds();
        }
        public void AddLoots(LootField[] loots)
        {
            GameManager.Instance.LootStorageManager.AddLoots(loots);
        }
        public void AddLoot(LootField loot)
        {
            GameManager.Instance.LootStorageManager.AddLoot(loot);
        }
        public void ClearLoots()
        {
            GameManager.Instance.LootStorageManager.ClearLoots();
        }

        public void ShowLoots(LootField[] loots)
        {
            LootedPanelInternal.ShowLoot(loots);
        }
    }
}
