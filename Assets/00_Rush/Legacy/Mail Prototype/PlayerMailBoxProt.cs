using LegionKnight.Prototype;
using UnityEngine;

namespace LegionKnight
{
    public class PlayerMailBoxProt : MailBoxProt
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerMailBoxProt m_MailBox;
        public PlayerMailBoxProt MailBox => m_MailBox;
    }
}
