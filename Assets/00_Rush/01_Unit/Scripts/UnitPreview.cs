using LegionKnight;
using TMPro;
using UnityEngine;

namespace Rush
{
    public class UnitPreview : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_UnitNameText;
        [SerializeField]
        private AvatarSpineUI m_UISpine;

        [SerializeField]
        private AbilityView[] m_AbilityViews;

        [SerializeField]
        private StatView m_Attack;
        [SerializeField]
        private StatView m_Defense;
        [SerializeField]
        private StatView m_Health;

        public void SetPreview(Unit unit)
        {
            m_UnitNameText.text = unit.Config.BaseInfo.Name;
            m_UISpine.Init(unit);
            SkillConfig[] skills = unit.Config.Skills;
            foreach (AbilityView ability in m_AbilityViews)
            {
                ability.Hide();
            }
            unit.HasBind(out SkillController controller);
            for (int i = 0; i < Mathf.Min(skills.Length, m_AbilityViews.Length); i++)
            {
                var config = skills[i];
                if (config == null) continue;
                Skill runtime = null;
                if (controller != null)
                {
                    foreach (var learned in controller.Skills)
                        if (learned != null && learned.SkillConfig == config) { runtime = learned; break; }
                }
                if (runtime != null) m_AbilityViews[i].InitRuntime(runtime, unit.Config);
                else m_AbilityViews[i].InitPreview(config, unit.Config);
                m_AbilityViews[i].Show();
            }
            StatsField stats = unit.Config.MainStats;
            int level = unit.Progression.Level;
            StatField stat = stats.GetFinalStat(level);
            m_Attack.SetCurrentValue(stat.Attack);
            m_Defense.SetCurrentValue(stat.Defense);
            m_Health.SetCurrentValue(stat.Health);
        }
    }
}