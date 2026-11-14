using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class DailySignInRewardsPopup : MonoBehaviour
    {
        [System.Serializable]
        private sealed class ListField
        {
            [SerializeField] private ScrollRect m_Scroll;
            [SerializeField] private DailySignInRewardCard m_RowTemplate;
            private readonly List<DailySignInRewardCard> m_Rows = new();

            internal void Bind(IReadOnlyList<CollectibleResultEntry> entries)
            {
                bool multiple = entries.Count > 1;
                m_Scroll.gameObject.SetActive(multiple);
                if (!multiple) return;

                for (int i = 0; i < entries.Count; i++)
                {
                    if (i == m_Rows.Count)
                        m_Rows.Add(Object.Instantiate(m_RowTemplate, m_Scroll.content, false));

                    var row = m_Rows[i];
                    row.Bind(entries[i]);
                    row.gameObject.SetActive(true);
                }
                for (int i = entries.Count; i < m_Rows.Count; i++)
                    m_Rows[i].gameObject.SetActive(false);

                LayoutRebuilder.ForceRebuildLayoutImmediate(m_Scroll.content);
                m_Scroll.StopMovement();
                m_Scroll.verticalNormalizedPosition = 1f;
            }
        }

        [System.Serializable]
        private sealed class DialogField
        {
            [SerializeField] private RectTransform m_Surface;
            [SerializeField] private TMP_Text m_Day;
            [SerializeField] private DailySignInRewardCard m_Featured;
            internal void Bind(string caption, IReadOnlyList<CollectibleResultEntry> entries)
            {
                m_Day.text = caption;
                bool single = entries.Count == 1;
                m_Featured.gameObject.SetActive(single);
                if (single) m_Featured.Bind(entries[0]);
            }

            internal void Fit(RectTransform stage)
            {
                if (stage == null || m_Surface == null) return;
                const float margin = 24f;
                float scale = Mathf.Min(1f, stage.rect.width / (m_Surface.rect.width + margin),
                    stage.rect.height / (m_Surface.rect.height + margin));
                if (scale > 0f) m_Surface.localScale = new Vector3(scale, scale, 1f);
            }
        }

        [SerializeField] private DialogField m_Dialog;
        [SerializeField] private ListField m_List;
        [SerializeField] private Button m_Continue;

        internal void Present(DailySignInClaimResult result) => Present(result.Entries, $"DAY {result.DayNumber}");

        internal void Present(IReadOnlyList<CollectibleResultEntry> entries, string caption)
        {
            if (entries.Count == 0) return;
            gameObject.SetActive(true);
            m_Dialog.Fit(transform as RectTransform);
            m_Dialog.Bind(caption, entries);
            m_List.Bind(entries);
        }

        private void OnEnable() => m_Continue.onClick.AddListener(Dismiss);
        private void OnDisable() => m_Continue.onClick.RemoveListener(Dismiss);
        private void OnRectTransformDimensionsChange() => m_Dialog?.Fit(transform as RectTransform);
        internal void Dismiss() => gameObject.SetActive(false);
    }
}
