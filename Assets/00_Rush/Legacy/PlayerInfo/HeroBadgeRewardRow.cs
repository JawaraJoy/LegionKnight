using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class HeroBadgeRewardRow : MonoBehaviour
    {
        [System.Serializable]
        private sealed class RewardField
        {
            [SerializeField] private Image m_Icon;
            [SerializeField] private Image m_Rim;
            [SerializeField] private TMP_Text m_Name;
            [SerializeField] private TMP_Text m_Requirement;
            [SerializeField] private TMP_Text m_Status;
            public Image Icon => m_Icon;
            public Image Rim => m_Rim;
            public TMP_Text Name => m_Name;
            public TMP_Text Requirement => m_Requirement;
            public TMP_Text Status => m_Status;
        }

        [SerializeField] private RewardField m_Reward;
        [SerializeField] private Color m_ReadyColor;
        [SerializeField] private Color m_OtherColor;
        [SerializeField] private GameObject m_Divider;
        [SerializeField] private GameObject m_ClaimedMark;
        [SerializeField] private GameObject m_ReadyMark;
        public string RewardName => m_Reward.Name.text;

        public void Display(HeroProfileBadgeConfig badge, int index, int claimed, int earned, bool canClaim)
        {
            m_Divider.SetActive(index < badge.Upgrade.Length - 1);
            var tier = badge.Upgrade[index];
            var item = tier.RewardOnUnlock[0].ItemLoot;
            var art = item.CollectibleField;
            var hero = badge.BaseInfo.Name;
            bool photo = item is CustomImageDefinition image && image.Type == CustomImageType.Icon;
            m_Reward.Name.text = photo ? hero + " Photo" : item.BaseInfo.Name;
            m_Reward.Icon.sprite = art.Icon;
            m_Reward.Rim.color = art.RarityConfig != null ? art.RarityConfig.Color : Color.white;
            int stage = tier.Target - 1;
            m_Reward.Requirement.text = stage == 0 ? "Recruit " + hero :
                $"Reach Evolve {stage} with {hero}";
            bool ready = canClaim && index == claimed;
            m_ClaimedMark.SetActive(index < claimed);
            m_ReadyMark.SetActive(ready);
            m_Reward.Status.color = ready ? m_ReadyColor : m_OtherColor;
            m_Reward.Status.text = index < claimed ? "Claimed" : ready ? "Ready to claim" :
                index < earned ? "Claim previous reward first" : "Locked";
        }
    }
}

