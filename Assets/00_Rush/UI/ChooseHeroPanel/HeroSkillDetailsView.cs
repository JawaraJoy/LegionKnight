using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class HeroSkillDetailsView : MonoBehaviour
    {
        [SerializeField] private RectTransform m_Sheet;
        [SerializeField] private Image m_Icon, m_Rim;
        [SerializeField] private TMP_Text m_Title, m_Category, m_Rarity, m_Level, m_OwnerName;
        [SerializeField] private GameObject m_OwnerRule, m_RarityBadge;
        [SerializeField] private ScrollRect m_Scroll;
        [SerializeField] private TMP_Text m_Description, m_DamageLabel, m_Damage, m_Charge, m_Range;
        [SerializeField] private GameObject m_DamageRow, m_ChargeColumn, m_RangeColumn, m_ColumnRule, m_Metrics;

        public void Close() => GetComponentInParent<SkillDescriptionPanel>().Hide();

        private void OnEnable() => FitSheet();

        private void OnRectTransformDimensionsChange() => FitSheet();

        private void FitSheet()
        {
            if (m_Sheet == null) return;
            var bounds = ((RectTransform)transform).rect.size;
            var design = m_Sheet.sizeDelta;
            float scale = Mathf.Min(bounds.x / design.x, bounds.y / design.y);
            if (scale > 0) m_Sheet.localScale = Vector3.one * scale;
        }

        public void Display(SkillConfig config, UnitConfig owner, int level, GameplayDescription.SkillDetails effects, bool resetScroll)
        {
            var art = config.CollectibleField;
            var rarity = art.RarityConfig;
            m_Icon.sprite = art.Icon;
            m_Icon.enabled = art.Icon != null;
            m_Rim.color = rarity != null ? rarity.Color : Color.white;
            m_Rarity.text = rarity != null ? rarity.BaseInfo.Name : string.Empty;
            m_RarityBadge.SetActive(rarity != null);
            m_Title.text = config.BaseInfo.Name;
            m_Category.text = config.Category != null ? config.Category.BaseInfo.Name.ToUpperInvariant() : string.Empty;
            m_Level.text = $"Skill Lv. {level}";
            m_OwnerName.text = owner != null ? owner.BaseInfo.Name : string.Empty;
            m_OwnerRule.SetActive(owner != null);

            string summary = config.DetailSummary;
            string description = effects.Description;
            if (!string.IsNullOrWhiteSpace(summary))
                description = string.IsNullOrWhiteSpace(description) ? summary : summary + "\n\n" + description;
            if (string.IsNullOrWhiteSpace(description) && effects.Damage == null && effects.Charge == null && effects.Range == null)
                description = config.BaseInfo.Description;
            m_Description.text = description;
            m_Description.gameObject.SetActive(!string.IsNullOrWhiteSpace(description));
            m_DamageLabel.text = effects.DamageLabel;
            m_Damage.text = effects.Damage;
            m_DamageRow.SetActive(effects.Damage != null);
            m_Charge.text = effects.Charge;
            m_Range.text = effects.Range;
            m_ChargeColumn.SetActive(effects.Charge != null);
            m_RangeColumn.SetActive(effects.Range != null);
            m_ColumnRule.SetActive(effects.Range != null && effects.Charge != null);
            m_Metrics.SetActive(effects.Range != null || effects.Charge != null);

            if (!isActiveAndEnabled) return;
            FitSheet();
            float position = resetScroll ? 1f : m_Scroll.verticalNormalizedPosition;
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_Scroll.content);
            Canvas.ForceUpdateCanvases();
            if (resetScroll) m_Scroll.StopMovement();
            m_Scroll.verticalNormalizedPosition = position;
        }
    }
}
