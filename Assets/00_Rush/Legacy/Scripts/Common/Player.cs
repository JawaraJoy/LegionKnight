using MoreMountains.Tools;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    // use rushplayer in the future
    // migrate all fields, and related component to Rushplayer, then delete this after all finished
    [System.Obsolete("Use RushPlayer in the Future")]
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

        private string PlayerNameKey => "playerName";

        private string PlayerDefaultName
        {
            get
            {
                int getrandomTag = Random.Range(0, RandomTagNumber);
                string defaultName = $"User[{getrandomTag}]";
                return defaultName ;
            }
        }
        private int RandomTagNumber => 1000000;
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
            bool hasEditedName = UnityService.Instance.HasData(PlayerNameKey);
            string getName = PlayerDefaultName;
            if (hasEditedName)
            {
                getName = UnityService.Instance.GetData<string>(PlayerNameKey);
            }
            SetPlayerNameInternal(getName);
            Debug.Log($"Player name: {m_PlayerName}");
            CanvasManager.Instance.SetPlayerNameView(m_PlayerName);
        }
        public void SetPlayerName(string playerName)
        {
            
            //SetPlayerNameAsync(m_PlayerName);
            SetPlayerNameInternal(playerName);
        }
        private void SetPlayerNameInternal(string playerName)
        {
            m_PlayerName = playerName;
            m_OnNameChanged?.Invoke(m_PlayerName);
            UnityService.Instance.SaveData(PlayerNameKey, m_PlayerName);
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
