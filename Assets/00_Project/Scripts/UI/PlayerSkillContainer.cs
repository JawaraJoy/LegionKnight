using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerSkillContainer : SkillContainer
    {
        
    }
    public partial class GameplayPanel
    {
        public void SetFill(string skillName, float fill)
        {
            GetBinding<PlayerSkillContainer>().SetFill(skillName, fill);
        }
        public void Active(string skillName)
        {
            GetBinding<PlayerSkillContainer>().Active(skillName);
        }
        public void InitCharacterSkill(CharacterDefinition definition)
        {
            GetBinding<PlayerSkillContainer>().Init(definition);
        }
        public void ActivePlayerSkillViews(bool set)
        {
            PlayerSkillContainer view = GetBinding<PlayerSkillContainer>();
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
        public void InitCharacterSkill(CharacterDefinition definition)
        {
            GameplayPanel panel = GetPanel<GameplayPanel>();
            panel.InitCharacterSkill(definition);
        }
        public void ActivePlayerSkillViews(bool set)
        {
            GameplayPanel panel = GetPanel<GameplayPanel>();
            panel.ActivePlayerSkillViews(set);
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
        public void InitCharacterSkill(CharacterDefinition definition)
        {
            GameManager.Instance.InitCharacterSkill(definition);
        }
        public void ActivePlayerSkillViews(bool set)
        {
            GameManager.Instance.ActivePlayerSkillViews(set);
        }
    }
}
