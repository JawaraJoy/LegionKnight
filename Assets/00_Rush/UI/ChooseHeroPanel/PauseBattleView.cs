using System.Collections.Generic;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class PauseBattleView : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Sheet;
        [SerializeField] private TMP_Text m_Stage;
        [SerializeField] private ScrollRect m_Scroll;
        [SerializeField] private LootItemView m_Template;
        [SerializeField] private GameObject m_Empty;

        private readonly List<LootItemView> m_Items = new();
        private LootStorageManager m_Storage;

        private void OnEnable()
        {
            FitSheet();
            var stage = RushGameManager.Instance?.StageManager?.UsedStageConfig;
            m_Stage.text = stage != null ? stage.BaseInfo.Name : string.Empty;
            m_Storage = GameManager.Instance?.LootStorageManager;
            if (m_Storage != null) m_Storage.OnLootedsChangedEvent.AddListener(OnLootChanged);
            DisplayLoot(m_Storage?.Looteds);
            m_Scroll.StopMovement();
            m_Scroll.horizontalNormalizedPosition = 0;
        }

        private void OnDisable()
        {
            if (m_Storage != null) m_Storage.OnLootedsChangedEvent.RemoveListener(OnLootChanged);
            m_Storage = null;
        }

        private void OnRectTransformDimensionsChange() => FitSheet();

        private void FitSheet()
        {
            if (m_Sheet == null) return;
            var area = ((RectTransform)transform).rect.size;
            var size = m_Sheet.sizeDelta;
            float scale = Mathf.Min(area.x / size.x, area.y / size.y);
            if (scale > 0) m_Sheet.localScale = Vector3.one * scale;
        }

        private void OnLootChanged(List<LootField> loot) => DisplayLoot(loot);

        private void DisplayLoot(IReadOnlyList<LootField> loot)
        {
            int used = 0;
            if (loot != null)
            {
                foreach (var item in loot)
                {
                    if (item?.ItemLoot == null || item.Amount <= 0) continue;
                    if (used == m_Items.Count) m_Items.Add(Instantiate(m_Template, m_Scroll.content, false));
                    var view = m_Items[used++];
                    view.gameObject.SetActive(true);
                    view.Init(item);
                }
            }
            for (int i = used; i < m_Items.Count; i++) m_Items[i].gameObject.SetActive(false);
            m_Empty.SetActive(used == 0);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Scroll.content);
        }
    }
}
