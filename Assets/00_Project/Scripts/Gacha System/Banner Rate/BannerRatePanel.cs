using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class BannerRatePanel : PanelView
    {
        [SerializeField, MMReadOnly]
        private GachaManager m_GachaManager;

        [SerializeField]
        private Transform m_NotesSpawner;
        [SerializeField]
        private GachaItemNoteView m_ItemNoteView;

        [SerializeField, MMReadOnly]
        private List<GachaItemNoteView> m_SpawnedItems = new ();

        [SerializeField, MMReadOnly]
        private int m_SpawnedItemsCount = 0;
        [SerializeField, MMReadOnly]
        private int m_GachaItemsCount = 0;

        private GachaItemNoteView GetSpawnedItem(GachaReward reward)
        {
            GachaItemNoteView r = m_SpawnedItems.Find(x => x.Item == reward);
            if (r == null)
            {
                return null;
            }
            return r;
        }
        private GachaManager GetGachaManagerInternal()
        {
            if (m_GachaManager == null)
            {
                m_GachaManager = GameManager.Instance.GachaMananger;
            }
            return m_GachaManager;
        }
        protected override void ShowInternal()
        {
            base.ShowInternal();
            foreach (var item in m_SpawnedItems)
            {
                item.Hide();
            }
            BannerDefinition defi = GetGachaManagerInternal().GetSelectedBanner().Definition;
            if (defi == null) return;

            StartCoroutine(SettingUpRateNote(defi));
        }

        private IEnumerator SettingUpRateNote(BannerDefinition defi)
        {
            List<GachaReward> gr = new(defi.GachaRewards);
            m_SpawnedItemsCount = m_SpawnedItems.Count;
            m_GachaItemsCount = gr.Count;
            bool morethanSpawned = m_GachaItemsCount > m_SpawnedItemsCount;
            if (morethanSpawned)
            {
                int selisih = m_GachaItemsCount - m_SpawnedItemsCount;
                for (int i = 0; i < selisih; i++)
                {
                    yield return StartCoroutine(SpawnItemNote());
                }
            }
            yield return new WaitForEndOfFrame();

            for (int i = 0; i < m_GachaItemsCount; i++)
            {
                m_SpawnedItems[i].Init(gr[i]);
            }
        }

        private IEnumerator SpawnItemNote()
        {
            var a = InstantiateAsync(m_ItemNoteView, m_NotesSpawner);
            yield return a;
            if (a.isDone)
            {
                GachaItemNoteView view = a.Result[0];
                if (view != null)
                {
                    m_SpawnedItems.Add(view);
                }
            }
        }
    }
}
