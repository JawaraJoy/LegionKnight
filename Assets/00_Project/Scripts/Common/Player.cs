using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Player : Singleton<Player>
    {
        [SerializeField]
        private NameSupplyDefinition m_NameSupplyDefinition;
        [SerializeField]
        private string m_PlayerName;
        [SerializeField]
        private CharacterDefinition m_CharacterDefinition;

        public CharacterDefinition CharacterDefinition => m_CharacterDefinition;
        [SerializeField]
        private UnityEvent m_OnStart = new();
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnSetCharacterDefinition = new();

        private bool m_CanUseResurrectionAds = true;
        public bool CanUseResurrectionAds => m_CanUseResurrectionAds;
        public void SetCanUseResurrectionAds(bool set)
        {
            m_CanUseResurrectionAds = set;
        }
        public void Init()
        {
            
            OnStartInvoke();
        }
        public void SetCharacterDefinition(CharacterDefinition definition)
        {
            m_CharacterDefinition = definition;
            OnSetCharacterDefinitionInvoke(definition);
        }
        private void OnStartInvoke()
        {
            m_OnStart?.Invoke();

            if (UnityService.Instance.HasData("PlayerName"))
            {
                m_PlayerName = UnityService.Instance.GetData<string>("PlayerName");
            }
            else
            {
                m_PlayerName = m_NameSupplyDefinition.GetRandomName();
                UnityService.Instance.SaveData("PlayerName", m_PlayerName);
            }
            Debug.Log($"Player name: {m_PlayerName}");
            GameManager.Instance.SetPlayerNameView(m_PlayerName);
        }
        public void AddOnStart(UnityAction action)
        {
            m_OnStart.AddListener(action);
        }
        public void RemoveOnStart(UnityAction action)
        {
            m_OnStart.RemoveListener(action);
        }
        private void OnSetCharacterDefinitionInvoke(CharacterDefinition definition)
        {
            m_OnSetCharacterDefinition?.Invoke(definition);

        }
        public int Attack => m_CharacterDefinition.Attack;
    }
}
