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

        public void ShowLoots(LootField[] loots)
        {
            LootedPanelInternal.ShowLoot(loots);
        }
    }
}
