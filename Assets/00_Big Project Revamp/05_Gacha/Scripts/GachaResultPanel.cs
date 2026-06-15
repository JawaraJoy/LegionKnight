namespace Rush
{
    // Subclass CollectibleResultPanel khusus gacha
    // Override IsSpecialEntryInternal untuk cek apakah entry berasal dari guarantee array
    public class GachaResultPanel : CollectibleResultPanel
    {
        private void Awake()
        {
            m_OnResultDone.AddListener(() => SetActiveCloseButton(true));
        }

        protected override void ShowInternal()
        {
            base.ShowInternal();
            SetActiveCloseButton(false);
        }
        private void SetActiveCloseButton(bool act)
        {
            m_CloseButton.interactable = act;
        }
        protected override bool IsSpecialEntryInternal(CollectibleResultEntry entry)
        {
            var banner = RushPlayer.Instance.GachaManager.ActiveBanner;
            if (banner == null) return false;

            return ContainsInArrayInternal(banner.FinalPityGuarantees, entry.Collectible)
                || ContainsInArrayInternal(banner.SmallPityGuarantees, entry.Collectible)
                || ContainsInArrayInternal(banner.FirstDrawGuarantees, entry.Collectible);
        }

        private static bool ContainsInArrayInternal(
            GachaCollectableConfig[] arr, CollectibleConfig target)
        {
            if (arr == null) return false;
            foreach (var c in arr)
                if (c.Collect == target) return true;
            return false;
        }
    }
}