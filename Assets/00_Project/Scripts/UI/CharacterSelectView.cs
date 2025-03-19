using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class CharacterSelectView : UIView
    {
        [SerializeField]
        private CharacterDefinition m_Definition;
        [SerializeField]
        private GameObject m_LockIcon;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterSelected = new();
        private void SelectCharacterInternal()
        {
            Player.Instance.SetSelectedCharacter(m_Definition);
            OnCharacterSelectedInvoke();
        }
        public void SelectCharacter()
        {
            SelectCharacterInternal();
        }
        public void Ini(CharacterUnit unit)
        {
            m_Definition = unit.Definition;
            m_LockIcon.SetActive(!unit.Owned);
            m_SelectButton.interactable = unit.Owned;
            m_SelectButton.onClick.RemoveAllListeners();
            m_SelectButton.onClick.AddListener(SelectCharacterInternal);
        }

        private void OnCharacterSelectedInvoke()
        {
            m_OnCharacterSelected?.Invoke(m_Definition);
        }
    }
}
