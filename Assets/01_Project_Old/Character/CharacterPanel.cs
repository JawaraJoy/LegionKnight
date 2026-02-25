using Rush;
using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string CharacterPanelId = "Character";
    }
    public partial class HeroPanel : PanelView
    {
        public override string UniqueId => PanelId.CharacterPanelId;

        [SerializeField]
        private SelectCharacterMode m_SelectCharacterMode = SelectCharacterMode.Character;
        public SelectCharacterMode SelectCharacterMode => m_SelectCharacterMode;

        [SerializeField]
        private CharacterSelectionView m_CharacterSelectionView;
        [SerializeField]
        private PlatformSelectionView m_PlatformSelectionView;
        public void SetSelectMode(int index)
        {
            m_SelectCharacterMode = (SelectCharacterMode)index;
            Adjust();
        }

        private void Adjust()
        {
            if (m_SelectCharacterMode == SelectCharacterMode.Character)
            {
                m_PlatformSelectionView.HideAllPlatforms();
            }
            else
            {
                m_CharacterSelectionView.HideAll();
            }
        }
        public void ShowRarity(RarityConfig rarityConfig)
        { 
            m_CharacterSelectionView.ShowRarity(rarityConfig);
            m_PlatformSelectionView.ShowRarity(rarityConfig);
            Adjust();
        }
        public void ShowAll()
        {
            m_CharacterSelectionView.ShowAll();
            m_PlatformSelectionView.ShowAllPlatforms();
            Adjust();
        }
    }

    public enum SelectCharacterMode
    {
        Character = 0,
        Platform = 1
    }
}
