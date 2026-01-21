using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

namespace LegionKnight
{
    public partial class AuthenticationManager : Authentication
    {
        
    }

    public partial class UnityService
    {
        [SerializeField]
        private AuthenticationManager m_AuthenticationManager;
        public string PlayerId => m_AuthenticationManager.PlayerId;
        public string PlayerName => m_AuthenticationManager.Playername;
        public void StartSinginWithUnity()
        {
            m_AuthenticationManager.StartSinginWithUnity();
        }
        public void SignInAnonymously()
        {
            m_AuthenticationManager.SignInAnonymously();
        }
        public void SignOut()
        {
            m_AuthenticationManager.SignOut();
        }
    }
}
