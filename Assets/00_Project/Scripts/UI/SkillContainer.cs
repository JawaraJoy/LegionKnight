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
    }
    public partial class GameplayPanel
    {
        public void SetFill(string skillName, float fill)
        {
            GetBinding<SkillContainer>().SetFill(skillName, fill);
        }
    }

    public partial class GameManager
    {
        public void SetSkillViewFill(string skillName, float fill)
        {
            GameplayPanel panel = GetPanel<GameplayPanel>();
            panel.SetFill(skillName, fill);
        }
    }
}
