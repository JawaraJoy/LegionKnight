using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Rush
{
    public interface IPlayGamesAccount
    {
        string PlayerId { get; }
        bool HasSession { get; }
        Task RestoreAsync();
        Task<bool> HasLinkAsync();
        Task LinkAsync(string code);
        Task SignInAsync(string code, bool create);
        Task VerifyOwnerAsync(string code, string owner);
    }

    public sealed class PlayGamesAccountFlow
    {
        public async Task<string> AuthenticateAsync(IPlayGamesAccount account, string savedOwner,
            bool hasLocalProgress, Func<Task<string>> getCode, Action<string> rememberOwner)
        {
            var owner = string.IsNullOrEmpty(savedOwner) ? account.PlayerId : savedOwner;
            if (string.IsNullOrEmpty(savedOwner) && !string.IsNullOrEmpty(owner)) rememberOwner(owner);
            if (account.HasSession)
            {
                await account.RestoreAsync();
                if (string.IsNullOrEmpty(account.PlayerId) || (!string.IsNullOrEmpty(owner) && owner != account.PlayerId))
                    throw new InvalidOperationException("Cached account mismatch.");
                owner = account.PlayerId;
                rememberOwner(owner);
                var code = await getCode();
                if (await account.HasLinkAsync())
                    await account.VerifyOwnerAsync(code, owner);
                else
                    await account.LinkAsync(code);
            }
            else if (!string.IsNullOrEmpty(owner))
            {
                // Verify in a separate SDK profile before replacing any default-session credentials.
                await account.VerifyOwnerAsync(await getCode(), owner);
                await account.SignInAsync(await getCode(), false);
            }
            else
            {
                if (hasLocalProgress) throw new InvalidOperationException("Guest credentials unavailable.");
                await account.SignInAsync(await getCode(), true);
                owner = account.PlayerId;
            }
            if (string.IsNullOrEmpty(owner) || owner != account.PlayerId)
                throw new InvalidOperationException("Unexpected player identity.");
            rememberOwner(owner);
            return owner;
        }
    }

    internal sealed class PlayGamesUnityAccount : IPlayGamesAccount
    {
        private const string c_VerificationProfile = "pgs-owner-check";
        private readonly IAuthenticationService m_Auth;
        private readonly string m_Environment;
        private IUnityServices m_Verifier;
        internal PlayGamesUnityAccount(string environment)
        {
            m_Auth = AuthenticationService.Instance;
            m_Environment = environment;
        }

        public string PlayerId => m_Auth.PlayerId;
        public bool HasSession => m_Auth.IsAuthorized || m_Auth.SessionTokenExists;
        public Task RestoreAsync() => m_Auth.IsAuthorized ? Task.CompletedTask :
            m_Auth.SignInAnonymouslyAsync(new SignInOptions { CreateAccount = false });
        public async Task<bool> HasLinkAsync() =>
            !string.IsNullOrEmpty((await m_Auth.GetPlayerInfoAsync()).GetGooglePlayGamesId());
        public Task LinkAsync(string code) => m_Auth.LinkWithGooglePlayGamesAsync(code);
        public Task SignInAsync(string code, bool create) =>
            m_Auth.SignInWithGooglePlayGamesAsync(code, new SignInOptions { CreateAccount = create });

        public async Task VerifyOwnerAsync(string code, string owner)
        {
            if (m_Verifier == null && !UnityServices.Services.TryGetValue(c_VerificationProfile, out m_Verifier))
                m_Verifier = UnityServices.CreateServices(c_VerificationProfile);
            if (m_Verifier.State != ServicesInitializationState.Initialized)
                await m_Verifier.InitializeAsync(new InitializationOptions().SetEnvironmentName(m_Environment).SetProfile(c_VerificationProfile));
            var verifier = m_Verifier.GetAuthenticationService();
            try
            {
                await verifier.SignInWithGooglePlayGamesAsync(code, new SignInOptions { CreateAccount = false });
                if (verifier.PlayerId != owner) throw new InvalidOperationException("Different Play Games account.");
            }
            finally { verifier.SignOut(true); }
        }
    }
}
