using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    /// <summary>
    /// Section yang menampilkan detail khusus card:
    /// daftar skill, card purpose (Activation / SkillUp), category modifications.
    ///
    /// Hanya tampil ketika entry yang di-bind adalah CardInventoryEntry.
    /// </summary>
    public class CardDetailSection : CollectibleDetailSection
    {
        [Header("Skill list")]
        [SerializeField] private Transform  m_SkillListContainer;
        [SerializeField] private GameObject m_SkillRowPrefab;   // prefab dengan TextMeshProUGUI

        [Header("Ownership badge")]
        [SerializeField] private GameObject         m_LockedOverlay;
        [SerializeField] private TextMeshProUGUI    m_OwnershipLabel;

        // ── CollectibleDetailSection ──────────────────────────────────
        public override bool IsRelevantFor(ICollectibleEntry entry)
        {
            return entry is CardInventoryEntry;
        }

        protected override void OnBind(ICollectibleEntry entry)
        {
            if (entry is not CardInventoryEntry cardEntry) return;

            BindSkills(cardEntry);
            BindOwnership(cardEntry);
        }

        // ── Skills ────────────────────────────────────────────────────
        private void BindSkills(CardInventoryEntry entry)
        {
            if (!m_SkillListContainer || !m_SkillRowPrefab) return;

            foreach (Transform child in m_SkillListContainer)
                Destroy(child.gameObject);

            CardSkillField[] skills = entry.Config.SkillConfigs;
            if (skills == null) return;

            foreach (CardSkillField skill in skills)
            {
                if (skill == null) continue;

                GameObject row    = Instantiate(m_SkillRowPrefab, m_SkillListContainer);
                var        label  = row.GetComponentInChildren<TextMeshProUGUI>();
                if (label)
                {
                    string purposeLabel = skill.CardPurpose == CardPurpose.Activation
                        ? "[Activate]"
                        : "[Skill Up]";
                    label.text = $"{purposeLabel} {skill.SkillConfig?.BaseInfo?.Name ?? "Unknown Skill"}";
                }
            }
        }

        // ── Ownership ─────────────────────────────────────────────────
        private void BindOwnership(CardInventoryEntry entry)
        {
            if (m_LockedOverlay)  m_LockedOverlay.SetActive(!entry.IsOwned);
            if (m_OwnershipLabel) m_OwnershipLabel.text = entry.IsOwned ? "Owned" : "Locked";
        }
    }
}
