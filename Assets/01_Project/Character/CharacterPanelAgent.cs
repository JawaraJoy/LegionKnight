using UnityEngine;

namespace LegionKnight
{
    public class CharacterPanelAgent : MonoBehaviour
    {
        private CharacterPanel m_CharacterPanel;

        private CharacterPanel GetCharacterPanel()
        {
            if (m_CharacterPanel == null)
            {
                m_CharacterPanel = CanvasManager.Instance.GetPanel<CharacterPanel>();
            }
            return m_CharacterPanel;
        }
        public void Refresh()
        {
            CharacterPanel panel = GetCharacterPanel();
            HeroView heroView = panel.GetBinding<HeroView>();
            heroView.Refresh();
        }
    }
}
