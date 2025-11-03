using Unity.Services.Authentication;
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
        public string PlayerName => m_PlayerName;

        [SerializeField]
        private UnityEvent<string> m_OnNameChanged = new();

        private PlayerInfoPanel m_CustomProfilePanel;
        private PlayerInfoPanel GetProfileInfoPanel()
        {
            if (m_CustomProfilePanel == null)
            {
                m_CustomProfilePanel = GameManager.Instance.GetPanel<PlayerInfoPanel>();
            }
            return m_CustomProfilePanel;
        }
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

            if (UnityService.Instance.HasData(m_NameSupplyDefinition.Id))
            {
                m_PlayerName = UnityService.Instance.GetData<string>(m_NameSupplyDefinition.Id);
            }
            else
            {
                m_PlayerName = m_NameSupplyDefinition.GetRandomName();
                UnityService.Instance.SaveData(m_NameSupplyDefinition.Id, m_PlayerName);
            }
            Debug.Log($"Player name: {m_PlayerName}");
            GameManager.Instance.SetPlayerNameView(m_PlayerName);
        }
        public void SetPlayerName(string playerName)
        {
            m_PlayerName = playerName;
            UnityService.Instance.SaveData(m_NameSupplyDefinition.Id, m_PlayerName);
            m_OnNameChanged?.Invoke(m_PlayerName);
            GetProfileInfoPanel().GetBinding<LevelView>().SetNameText(m_PlayerName);
            SetPlayerNameAsync(m_PlayerName);
        }

        private async void SetPlayerNameAsync(string playerName)
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(m_PlayerName);
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
        public Stat GetFinalStat(int star, int level)
        {
            return m_CharacterDefinition.FinalStat(star, level);
        }
    }
}
