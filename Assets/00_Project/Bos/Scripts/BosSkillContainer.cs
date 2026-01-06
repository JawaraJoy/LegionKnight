using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class BosSkillContainer : SkillContainer
    {
    }
    public partial class GameplayPanel
    {
        public void SetBosSkillFill(string skillName, float fill)
        {
            GetBinding<BosSkillContainer>().SetFill(skillName, fill);
        }
        public void ActiveBosSkill(string skillName)
        {
            GetBinding<BosSkillContainer>().Active(skillName);
        }
        public void ActiveBosSkillViews(bool set)
        {
            BosSkillContainer view = GetBinding<BosSkillContainer>();
            if (set)
            {
                view.Show();
            }
            else
            {
                view.Hide();
            }
        }
    }
    public partial class CanvasManager
    {
        public void SetBosSkillViewFill(string skillName, float fill)
        {
            GameplayPanel panel = GetPanel<GameplayPanel>();
            panel.SetBosSkillFill(skillName, fill);
        }
        public void ActiveBosSkill(string skillName)
        {
            GameplayPanel panel = GetPanel<GameplayPanel>();
            panel.ActiveBosSkill(skillName);
        }
        public void ActiveBosSkillViews(bool set)
        {
            GameplayPanel panel = GetPanel<GameplayPanel>();
            panel.ActiveBosSkillViews(set);
        }
    }
    public partial class GameplayPanelAgent
    {
        public void SetBosSkillViewFill(string skillName, float fill)
        {
            CanvasManager.Instance.SetBosSkillViewFill(skillName, fill);
        }
        public void ActiveBosSkill(string skillName)
        {
            CanvasManager.Instance.ActiveBosSkill(skillName);
        }
        public void ActiveBosSkillViews(bool set)
        {
            CanvasManager.Instance.ActiveBosSkillViews(set);
        }
    }
}
