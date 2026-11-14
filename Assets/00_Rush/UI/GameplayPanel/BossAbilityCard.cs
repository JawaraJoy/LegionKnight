using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class BossAbilityCard : MonoBehaviour
    {
        [SerializeField] private Button m_OpenDetails;
        [SerializeField] private Image m_Icon, m_Frame;
        [SerializeField] private TMP_Text m_Category, m_Title, m_Description;
        [SerializeField] private TMP_Text m_Cast, m_Charge, m_InterruptText;
        [SerializeField] private RectTransform m_Metrics, m_Interrupt;
        [SerializeField] private LayoutElement m_Height;
        private SkillConfig m_Config;
        private Skill m_Runtime;
        private UnitConfig m_Owner;
        private SkillDescriptionPanel m_Detail;

        private void Awake() => m_OpenDetails.onClick.AddListener(OpenDetails);
        private void OnDestroy() => m_OpenDetails.onClick.RemoveListener(OpenDetails);

        public void Display(SkillConfig config, Skill runtime, SkillController controller, UnitConfig owner,
            int maxHealth, SkillDescriptionPanel detail)
        {
            m_Config = config;
            m_Runtime = runtime;
            m_Owner = owner;
            m_Detail = detail;
            m_Title.text = config.BaseInfo.Name.Trim();
            m_Category.text = config.Category != null ? config.Category.BaseInfo.Name.Trim().ToUpperInvariant() : string.Empty;
            m_Icon.sprite = config.CollectibleField.Icon;
            m_Icon.enabled = m_Icon.sprite != null;
            m_Frame.color = config.CollectibleField.RarityConfig?.Color ?? Color.white;
            int level = runtime != null ? runtime.Progression.Level : config.LevelSet?.Level ?? 1;
            m_Description.text = string.IsNullOrWhiteSpace(config.DetailSummary)
                ? GameplayDescription.SkillEffects(config, level, controller) : config.DetailSummary;
            float cast = config.Casting?.CastDuration ?? 0f;
            m_Cast.text = cast > 0 ? cast.ToString("0.0", CultureInfo.InvariantCulture) + " s" : "Instant";
            var activation = config.Activation;
            m_Charge.text = activation != null && activation.ForceActiveState == ForceActiveState.OnChargeFull
                ? activation.Charge.ToString("0.##", CultureInfo.InvariantCulture) : "\u2014";

            float descriptionWidth = m_Description.rectTransform.rect.width;
            float textHeight = Mathf.Ceil(m_Description.GetPreferredValues(m_Description.text, descriptionWidth, Mathf.Infinity).y);
            m_Description.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight);
            float metricsY = 104f + textHeight;
            m_Metrics.anchoredPosition = new Vector2(16, -metricsY);
            float bottom = metricsY + 63f;
            m_Interrupt.gameObject.SetActive(cast > 0);
            if (cast > 0)
            {
                int target = Mathf.RoundToInt(maxHealth * config.Casting.MaxInteruptResist);
                m_InterruptText.text = target > 0
                    ? "Deal " + target.ToString("N0", CultureInfo.InvariantCulture) + " damage during the cast to cancel it."
                    : "Any damage that reaches the boss during the cast interrupts it.";
                m_Interrupt.anchoredPosition = new Vector2(13, -bottom);
                bottom += m_Interrupt.rect.height + 4f;
            }
            m_Height.preferredHeight = bottom + 6f;
        }

        private void OpenDetails()
        {
            if (m_Detail == null || m_Config == null) return;
            if (m_Runtime != null) m_Detail.ShowRuntimeDetail(m_Runtime);
            else m_Detail.ShowPreview(m_Config, m_Owner);
        }
    }
}
