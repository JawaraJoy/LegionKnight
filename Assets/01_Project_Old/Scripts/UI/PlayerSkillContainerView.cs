using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerSkillContainerView : SkillContainerView
    {
        
    }
    public partial class GameplayPanel
    {
        public void SetFill(string skillName, float fill)
        {
            GetBinding<PlayerSkillContainerView>().SetFill(skillName, fill);
        }
        public void ChargeAmount(string skillName, int amount)
        {
            GetBinding<PlayerSkillContainerView>().ChargeAmount(skillName, amount);
        }
        public void Active(string skillName)
        {
            GetBinding<PlayerSkillContainerView>().Active(skillName);
        }
        public void InitCharacterSkill(HeroUnitConfig heroConfig)
        {
            GetBinding<PlayerSkillContainerView>().Init(heroConfig);
        }
        public void ActivePlayerSkillViews(bool set)
        {
            PlayerSkillContainerView view = GetBinding<PlayerSkillContainerView>();
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

    public partial class GameplayPanelAgent
    {
        private GameplayPanel m_Panel;
        private GameplayPanel GetPanelInternal()
        {
            if (m_Panel == null)
            {
                m_Panel = CanvasManager.Instance.GetPanel<GameplayPanel>();
            }
            return m_Panel;
        }
        public void SetSkillViewFill(string skillName, float fill)
        {
            GetPanelInternal().SetFill(skillName, fill);
        }
        public void ChargeAmount(string skillName, int amount)
        {
            GetPanelInternal().ChargeAmount(skillName, amount);
        }
        public void Active(string skillName)
        {
            GetPanelInternal().Active(skillName);
        }
        public void InitCharacterSkill(HeroUnitConfig heroConfig)
        {
            GetPanelInternal().InitCharacterSkill(heroConfig);
        }
        public void ActivePlayerSkillViews(bool set)
        {
            GetPanelInternal().ActivePlayerSkillViews(set);
        }
    }
}
