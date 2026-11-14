using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    // Implement on a boss unit when a sustained, interruptible ability owns a channel clock.
    public interface IBossChannelProgress
    {
        bool IsChanneling { get; }
        string AbilityName { get; }
        float Remaining { get; }
        float Duration { get; }
        int InterruptDamage { get; }
        int InterruptTarget { get; }
    }

    public sealed class BattleBossHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_BossNameLabel, m_Percent, m_Ability, m_Resist;
        [SerializeField] private Image m_Health, m_Cast, m_Interrupt;
        [SerializeField] private GameObject m_Row, m_InterruptTrack;
        [SerializeField] private CanvasGroup m_Surface;
        [SerializeField] private TMP_Text m_LevelLabel, m_UltimateName, m_UltimateCharge;
        [SerializeField] private GameObject m_UltimateRow;
        [SerializeField] private Image m_UltimateIcon, m_UltimateRim, m_UltimateFill;

        private int m_Level = -1, m_CurrentCharge = -1, m_MaxCharge = -1;
        private SkillConfig m_UltimateConfig;
        private Unit m_Owner;
        private Damageable m_HP;
        private SkillController m_Skills;
        private BossWorldBars m_WorldBars;
        private IBossChannelProgress m_Channel;
        private bool m_WasChannel;
        private string m_LastTitle;
        private Skill m_LastCast;
        private int m_LastPercent = -1, m_Tenths, m_LastDamage;
        private void OnDisable()
        {
            ReleaseWorldBarsInternal();
            m_Owner = null; m_HP = null; m_Skills = null; m_LastCast = null; m_LastPercent = -1;
            m_Row.SetActive(false);
        }
        private void ReleaseWorldBarsInternal()
        {
            if (m_WorldBars != null) m_WorldBars.SetHudVisible(false);
            m_WorldBars = null;
        }
        private void LateUpdate()
        {
            var game = RushGameManager.Instance;
            var waves = game != null && game.StageManager != null ? game.StageManager.EnemyWaveHandler : null;
            var owner = waves != null ? waves.BossUnitExisten : null;
            if (owner == null || !owner.gameObject.activeInHierarchy || owner.Config == null)
            { m_Surface.alpha = 0; m_Surface.blocksRaycasts = false; ReleaseWorldBarsInternal(); m_Owner = null; m_Row.SetActive(false); return; }
            if (m_Owner != owner)
            {
                ReleaseWorldBarsInternal();
                m_Owner = owner; owner.HasBind(out m_HP); owner.HasBind(out m_Skills);
                m_WorldBars = owner.GetComponent<BossWorldBars>();
                m_Channel = owner.GetComponent<IBossChannelProgress>();
                if (m_WorldBars != null && m_HP != null) m_WorldBars.SetHudVisible(true);
                m_LastPercent = -1; m_LastCast = null; m_BossNameLabel.text = owner.Config.BaseInfo.Name.Trim();
            }
            bool alive = m_HP != null && m_HP.IsAlive;
            m_Surface.alpha = alive ? 1 : 0; m_Surface.blocksRaycasts = alive;
            if (!alive) { m_Row.SetActive(false); return; }
            UpdateBossInfoInternal(owner);
            float hp = m_HP.MaxHealth > 0 ? Mathf.Clamp01((float)m_HP.Health/m_HP.MaxHealth) : 0;
            m_Health.fillAmount = hp;
            int percent = (int)(((long)m_HP.Health * 100 + m_HP.MaxHealth - 1) / Mathf.Max(1,m_HP.MaxHealth));
            if (m_LastPercent != percent) { m_LastPercent = percent; m_Percent.SetText("{0}%",percent); }
            Skill cast = m_Skills != null ? m_Skills.CurrentCastingSkill : null;
            bool channel = m_Channel != null && m_Channel.IsChanneling && m_Channel.Duration > 0;
            bool visible = channel || (cast != null && cast.IsCasting && cast.SkillConfig != null);
            if (m_Row.activeSelf != visible) m_Row.SetActive(visible);
            if (!visible) { m_LastCast = null; m_LastTitle = null; return; }
            float remaining = Mathf.Max(0,channel ? m_Channel.Remaining : cast.RemainingCastTime);
            float duration = channel ? m_Channel.Duration : cast.SkillConfig.Casting.CastDuration;
            m_Cast.fillAmount = Mathf.Clamp01(remaining / Mathf.Max(.001f,duration));
            string title = channel ? m_Channel.AbilityName : cast.SkillConfig.BaseInfo.Name;
            bool changed = m_WasChannel != channel || m_LastCast != cast || m_LastTitle != title;
            if (changed) m_Cast.color = channel ? (Color)new Color32(185,164,255,255) : (Color)new Color32(255,204,118,255);
            int tenths = Mathf.CeilToInt(remaining*10);
            if (changed || tenths != m_Tenths)
            { m_Tenths = tenths; m_Ability.text = $"{(channel ? "Channeling" : "Casting")}  ·  {title?.Trim()}  ·  {tenths*.1f:0.0}s"; }
            int resist = channel ? m_Channel.InterruptTarget : cast.MaxInterruptCount;
            if (m_InterruptTrack.activeSelf != (resist > 0)) m_InterruptTrack.SetActive(resist > 0);
            if (resist > 0)
            {
                int damage = Mathf.Clamp(channel ? m_Channel.InterruptDamage : cast.CurrentInterruptCount,0,resist);
                m_Interrupt.fillAmount = (float)damage/resist;
                if (changed || m_LastDamage != damage) { m_LastDamage = damage; m_Resist.SetText("Interrupt  {0} / {1}",(float)damage,(float)resist); }
            }
            m_LastCast = cast; m_WasChannel = channel; m_LastTitle = title;
        }

        private void UpdateBossInfoInternal(Unit owner)
        {
            int level = owner.Progression.Level;
            if (m_Level != level)
            {
                m_Level = level;
                m_LevelLabel.SetText("Lv. {0}", level);
            }

            Skill ultimate = m_WorldBars != null ? m_WorldBars.QueuedUltimate : null;
            SkillConfig config = ultimate != null ? ultimate.SkillConfig : null;
            bool show = config != null;
            if (m_UltimateRow.activeSelf != show) m_UltimateRow.SetActive(show);
            if (!show)
            {
                m_UltimateConfig = null;
                return;
            }

            if (m_UltimateConfig != config)
            {
                m_UltimateConfig = config;
                m_UltimateName.text = config.BaseInfo.Name.Trim();
                m_UltimateIcon.sprite = config.CollectibleField.Icon;
                m_UltimateIcon.enabled = m_UltimateIcon.sprite != null;
                var rarity = config.CollectibleField.RarityConfig;
                m_UltimateRim.color = rarity != null ? rarity.Color : Color.white;
                m_CurrentCharge = -1;
            }

            int maximum = Mathf.RoundToInt(config.Activation.Charge);
            int current = Mathf.Clamp(Mathf.RoundToInt(ultimate.RemainingCharge), 0, maximum);
            m_UltimateFill.fillAmount = maximum > 0 ? Mathf.Clamp01(ultimate.RemainingCharge / maximum) : 0f;
            if (m_CurrentCharge == current && m_MaxCharge == maximum) return;
            m_CurrentCharge = current;
            m_MaxCharge = maximum;
            if (maximum > 0) m_UltimateCharge.SetText("{0}/{1}", (float)current, (float)maximum);
            else m_UltimateCharge.text = string.Empty;
        }
    }
}
