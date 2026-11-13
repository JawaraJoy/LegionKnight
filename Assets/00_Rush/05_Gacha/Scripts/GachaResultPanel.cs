using System;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class GachaResultPanel : CollectibleResultPanel
    {
        [Serializable]
        private sealed class SummaryField
        {
            [SerializeField] private TMP_Text m_Banner;
            [SerializeField] private TMP_Text m_Count;
            [SerializeField] private GachaDrawResultCard m_Single;
            [SerializeField] private ScrollRect m_Grid;
            [SerializeField] private Button m_Backdrop;
            [SerializeField] private MMF_Player m_Entrance;

            public Button Backdrop => m_Backdrop;
            public GachaDrawResultCard Single => m_Single;

            public void PlayEntrance(bool instant)
            {
                if (m_Entrance.IsPlaying) m_Entrance.StopFeedbacks();
                if (instant) m_Entrance.transform.localScale = Vector3.one;
                else m_Entrance.PlayFeedbacks();
            }

            public void Prepare(GachaBannerConfig banner, int count)
            {
                m_Banner.text = banner != null ? banner.BaseInfo.Name : string.Empty;
                m_Count.text = $"Draw {count}";
                m_Single.gameObject.SetActive(false);
                m_Grid.gameObject.SetActive(count > 1);
                m_Grid.StopMovement();
                m_Grid.content.anchoredPosition = Vector2.zero;
            }
        }

        [SerializeField] private SummaryField m_Summary;
        private bool m_IsSingle;
        private GachaBannerConfig m_Banner;

        public override void Show(CollectibleResultData result) => OpenInternal(result, false);
        internal void ShowAll(CollectibleResultData result) => OpenInternal(result, true);

        private void OpenInternal(CollectibleResultData result, bool showAllAtOnce)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            m_Banner = RushPlayer.Instance.GachaManager.ResolveResultBanner(result.Source);
            m_IsSingle = result.Entries.Count == 1;
            if (m_Summary?.Single != null) m_Summary.Prepare(m_Banner, result.Entries.Count);
            ShowResultInternal(result, showAllAtOnce);
        }

        private void Awake()
        {
            m_OnResultDone.AddListener(EnableCloseInternal);
            if (m_Summary?.Backdrop != null) m_Summary.Backdrop.onClick.AddListener(HideInternal);
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            SetCloseEnabledInternal(false);
            if (IsShown && m_Summary?.Single != null) m_Summary.PlayEntrance(ShowAllAtOnce);
        }

        private void EnableCloseInternal() => SetCloseEnabledInternal(true);

        private void SetCloseEnabledInternal(bool enabled)
        {
            m_CloseButton.interactable = enabled;
            if (m_Summary?.Backdrop != null) m_Summary.Backdrop.interactable = enabled;
        }

        protected override void SpawnEntryInternal(CollectibleResultEntry entry)
        {
            if (m_Summary?.Single == null || !m_IsSingle)
            {
                base.SpawnEntryInternal(entry);
                return;
            }
            var card = m_Summary.Single;
            card.gameObject.SetActive(true);
            card.Setup(entry, !ShowAllAtOnce);
        }

        protected override bool IsSpecialEntryInternal(CollectibleResultEntry entry)
        {
            if (m_Banner == null) return false;
            return ContainsReward(m_Banner.FinalPityGuarantees, entry.Collectible)
                || ContainsReward(m_Banner.SmallPityGuarantees, entry.Collectible)
                || ContainsReward(m_Banner.FirstDrawGuarantees, entry.Collectible);
        }

        private static bool ContainsReward(GachaCollectableConfig[] rewards, CollectibleConfig target)
        {
            if (rewards == null) return false;
            foreach (var reward in rewards)
                if (reward != null && reward.Collect == target) return true;
            return false;
        }
    }
}
