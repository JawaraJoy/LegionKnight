using UnityEngine;

namespace LegionKnight
{
    public class CharacterPanelAgent : MonoBehaviour
    {
        private HeroPanel m_CharacterPanel;

        private HeroPanel GetCharacterPanel()
        {
            if (m_CharacterPanel == null)
            {
                m_CharacterPanel = CanvasManager.Instance.GetPanel<HeroPanel>();
            }
            return m_CharacterPanel;
        }
        public void Refresh()
        {
            HeroPanel panel = GetCharacterPanel();
            HeroView heroView = panel.GetBinding<HeroView>();
            heroView.Refresh();
        }
    }
}
