using TMPro;
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
        private Image m_UnitIcon;
        [SerializeField]
        private GameObject m_LockIcon;
        [SerializeField]
        private Button m_SelectButton;
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterSelected = new();
        public CharacterDefinition Definition => m_Definition;

        [SerializeField]
        private Image m_Frame;
        [SerializeField]
        private TextMeshProUGUI m_LevelText;

        [SerializeField]
        private StarGroupView m_StarGroupView;
        private void OnEnable()
        {
            InitInternal();
        }
        private void SelectCharacterInternal()
        {
            Player.Instance.SetSelectedCharacter(m_Definition);
            OnCharacterSelectedInvoke();
        }
        public void SelectCharacter()
        {
            SelectCharacterInternal();
        }
        private void InitInternal()
        {
            CharacterUnit character = Player.Instance.GetCharacterUnit(m_Definition);
            InitInternal(character);
            
        }
        private void InitInternal(CharacterUnit unit)
        {
            m_Definition = unit.Definition;
            m_LockIcon.SetActive(!unit.Owned);
            m_SelectButton.interactable = unit.Owned;
            m_UnitIcon.sprite = unit.SmallIcon;
            m_SelectButton.onClick.RemoveAllListeners();
            m_SelectButton.onClick.AddListener(SelectCharacterInternal);

            m_StarGroupView.Init(m_Definition);
            m_Frame.color = unit.Definition.ColorRarity;
            m_LevelText.text = $"Lv {unit.Level}";
        }
        public void Init(CharacterUnit unit)
        {
            InitInternal(unit);
        }

        private void OnCharacterSelectedInvoke()
        {
            m_OnCharacterSelected?.Invoke(m_Definition);
        }
    }
}
