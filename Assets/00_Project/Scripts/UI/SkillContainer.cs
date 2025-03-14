using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class SkillContainer : UIView
    {
        [SerializeField]
        private List<SkillView> m_SkillViews = new();

        private SkillView GetSkillView(string skillName)
        {
            SkillView match = m_SkillViews.Find(x => x.SkillName == skillName);
            return match;
        }
        public void SetFill(string skillName, float fill)
        {
            GetSkillView(skillName).SetFill(fill);
        }
        public void Active(string skillName)
        {
            GetSkillView(skillName).Active();
        }
    }
    public partial class GameplayPanel
    {
        public void SetFill(string skillName, float fill)
        {
            GetBinding<SkillContainer>().SetFill(skillName, fill);
        }
        public void Active(string skillName)
        {
            GetBinding<SkillContainer>().Active(skillName);
        }
    }

    public partial class GameManager
    {
        public void SetSkillViewFill(string skillName, float fill)
        {
            GameplayPanel panel = GetPanel<GameplayPanel>();
            panel.SetFill(skillName, fill);
        }
        public void Active(string skillName)
        {
            GameplayPanel panel = GetPanel<GameplayPanel>();
            panel.Active(skillName);
        }
    }
    public partial class GameplayPanelAgent
    {
        public void SetSkillViewFill(string skillName, float fill)
        {
            GameManager.Instance.SetSkillViewFill(skillName, fill);
        }
        public void Active(string skillName)
        {
            GameManager.Instance.Active(skillName);
        }
    }
}
