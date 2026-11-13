using System;
using System.Linq;
using System.Threading.Tasks;
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
                int getrandomTag = UnityEngine.Random.Range(0, RandomTagNumber);
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
            var cached = UnityService.Instance.GetData<string>(PlayerNameKey);
            ApplyPlayerName(string.IsNullOrWhiteSpace(cached) ? PlayerDefaultName : cached);
            m_NameInitialization = RestoreAccountNameAsync();
        }

        public const int MaxPlayerNameLength = 20;
        private Task m_NameInitialization = Task.CompletedTask;
        private bool m_IsRenaming;

        public static bool IsValidPlayerName(string name) =>
            !string.IsNullOrWhiteSpace(name) && name.Length <= MaxPlayerNameLength &&
            !name.Any(c => char.IsWhiteSpace(c) || char.IsControl(c) || c == '<' || c == '>');

        private static string DisplayName(string accountName)
        {
            int tag = accountName.LastIndexOf('#');
            return tag > 0 && accountName.Length - tag == 5 && accountName.Substring(tag + 1).All(char.IsDigit)
                ? accountName.Substring(0, tag) : accountName;
        }

        private async Task RestoreAccountNameAsync()
        {
            try
            {
                var service = AuthenticationService.Instance;
                if (!service.IsAuthorized) return;
                var name = await service.GetPlayerNameAsync(false);
                if (!this || !service.IsAuthorized) return;
                if (string.IsNullOrEmpty(name) && IsValidPlayerName(m_PlayerName))
                    name = await service.UpdatePlayerNameAsync(m_PlayerName);
                if (this && !string.IsNullOrEmpty(name)) ApplyPlayerName(DisplayName(name));
            }
            catch (Exception ex) { Debug.LogWarning($"Player name sync unavailable: {ex.Message}"); }
        }

        public async void SetPlayerName(string playerName)
        {
            try { await RenamePlayerAsync(playerName); }
            catch (Exception ex) { Debug.LogWarning($"Rename failed: {ex.Message}"); }
        }

        public Task RenamePlayerAsync(string name) => RenamePlayerAsync(name, candidate =>
        {
            var service = AuthenticationService.Instance;
            if (!service.IsAuthorized) throw new InvalidOperationException("Sign in to change your name.");
            return service.UpdatePlayerNameAsync(candidate);
        });

        internal async Task RenamePlayerAsync(string name, Func<string, Task<string>> submit)
        {
            name = name?.Trim();
            if (!IsValidPlayerName(name)) throw new ArgumentException("Enter 1–20 characters without spaces.");
            if (m_IsRenaming) throw new InvalidOperationException("A rename is already in progress.");
            m_IsRenaming = true;
            try
            {
                await m_NameInitialization;
                var confirmed = await submit(name);
                if (string.IsNullOrWhiteSpace(confirmed)) throw new InvalidOperationException("Name was not confirmed.");
                if (this) ApplyPlayerName(DisplayName(confirmed));
            }
            finally { m_IsRenaming = false; }
        }

        private void ApplyPlayerName(string name)
        {
            m_PlayerName = name;
            UnityService.Instance.SaveData(PlayerNameKey, name);
            GetProfileInfoPanel()?.GetBinding<LevelView>()?.SetNameText(name);
            CanvasManager.Instance.SetPlayerNameView(name);
            m_OnNameChanged?.Invoke(name);
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
