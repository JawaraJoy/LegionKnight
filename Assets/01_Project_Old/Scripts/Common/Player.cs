using MoreMountains.Tools;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Player : Singleton<Player>
    {
        [SerializeField, MMReadOnly]
        private string m_PlayerName;

        [SerializeField]
        private UnityEvent m_OnStart = new();

        [SerializeField, MMReadOnly]
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
                m_CustomProfilePanel = CanvasManager.Instance.GetPanel<PlayerInfoPanel>();
            }
            return m_CustomProfilePanel;
        }
        public void SetCanUseResurrectionAds(bool set)
        {
            m_CanUseResurrectionAds = set;
        }
        public void Init()
        {
            
            OnInitInvoke();
        }
        private void OnInitInvoke()
        {
            m_OnStart?.Invoke();
            m_PlayerName = UnityService.Instance.PlayerName;
            Debug.Log($"Player name: {m_PlayerName}");
            CanvasManager.Instance.SetPlayerNameView(m_PlayerName);
        }
        public void SetPlayerName(string playerName)
        {
            m_PlayerName = playerName;
            m_OnNameChanged?.Invoke(m_PlayerName);
            GetProfileInfoPanel().GetBinding<LevelView>().SetNameText(m_PlayerName);
            SetPlayerNameAsync(m_PlayerName);
        }

        private async void SetPlayerNameAsync(string playerName)
        {
            await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        }
        public void AddOnStart(UnityAction action)
        {
            m_OnStart.AddListener(action);
        }
        public void RemoveOnStart(UnityAction action)
        {
            m_OnStart.RemoveListener(action);
        }
    }
}
