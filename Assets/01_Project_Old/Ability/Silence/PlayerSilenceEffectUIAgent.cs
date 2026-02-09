using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class PlayerSilenceEffectUIAgent : MonoBehaviour
    {
        private GameplayPanel m_Panel;

        private PlayerSkillContainer m_SkillContainer;
        private GameplayPanel Panel
        {
            get
            {
                if (m_Panel == null)
                {
                    m_Panel = CanvasManager.Instance.GetPanel<GameplayPanel>();
                }
                return m_Panel;
            }
        }
        private PlayerSkillContainer SkillContainer
        {
            get
            {
                if (m_SkillContainer == null)
                {
                    m_SkillContainer = Panel.GetBinding<PlayerSkillContainer>();
                }
                return m_SkillContainer;
            }
        }

        public void SetActiveSilenceUI(bool active)
        {
            foreach(SkillView skills in SkillContainer.SkillViews)
            {
                SilenceEffectUI silenceEffect = skills.GetBinding<SilenceEffectUI>();
                if (silenceEffect != null)
                {
                    if (active)
                    {
                        silenceEffect.Show();
                    }
                    else
                    {
                        silenceEffect.Hide();
                    }
                }
            }
        }
    }
}
