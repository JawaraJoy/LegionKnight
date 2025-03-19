using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class HeroView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_HeroNameText;
        [SerializeField]
        private Image m_HeroBigIcon;
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterSelected = new();
        public void SetCharacterSelected(CharacterDefinition defi)
        {
            m_HeroBigIcon.sprite = defi.Icon;
            m_HeroNameText.text = defi.name;
            OnCharacterSelectedInvoke(defi);
        }
        private void OnCharacterSelectedInvoke(CharacterDefinition defi)
        {
            m_OnCharacterSelected?.Invoke(defi);
        }
    }
    public partial class CharacterPanel
    {
        private HeroView GetHeroView()
        {
            return GetBinding<HeroView>();
        }

        public void SetCharacterSelected(CharacterDefinition defi)
        {
            GetHeroView().SetCharacterSelected(defi);
        }
    }
    public partial class GameManager
    {
        private CharacterPanel GetCharacterPanel()
        {
            return GetPanel<CharacterPanel>();
        }
        public void SetCharacterSelected(CharacterDefinition defi)
        {
            GetCharacterPanel().SetCharacterSelected(defi);
        }
    }
}
