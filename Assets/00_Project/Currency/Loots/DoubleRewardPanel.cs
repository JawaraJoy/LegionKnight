using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class DoubleRewardPanel : PanelView
    {
        [SerializeField]
        private LootMonitor m_MainLoot;
        [SerializeField]
        private LootMonitor m_MirrorLoot;
        private LootStorageManager m_LootStorageManager;

        [SerializeField]
        private Button m_CloseButton;
        private LootStorageManager LootStorageManager
        {
            get
            {
                if (m_LootStorageManager == null)
                {
                    m_LootStorageManager = GameManager.Instance.GetLootStorageManager();
                }
                return m_LootStorageManager;
            }
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            

            TransferLootToMain();
        }
        public void AddMainLoot(LootField loot)
        {
            m_MainLoot.AddLootView(loot);
        }
        public void AddMirrorLoot(LootField loot)
        {
            m_MirrorLoot.AddLootView(loot);
        }
        public void CopyMirrorFromLooted()
        {
            LootStorageManager.CopyMirrorFromLooted();
        }
        override protected void HideInternal()
        {
            base.HideInternal();
            m_MainLoot.ClearAllLootViews();
            m_MirrorLoot.ClearAllLootViews();
        }

        private void TransferLootToMain()
        {
            List<LootField> mainLoots = new(LootStorageManager.Looteds);
            List<LootField> mirrorLoots = new(LootStorageManager.MirrorLoots);
            m_MainLoot.AddLootsView(mainLoots);
            m_MirrorLoot.AddLootsView(mirrorLoots);
            m_CloseButton.interactable = false;
            //yield return StartCoroutine(LootStorageManager.TransferMirrorToLooteds());
            m_CloseButton.interactable = true;
        }
    }
}
