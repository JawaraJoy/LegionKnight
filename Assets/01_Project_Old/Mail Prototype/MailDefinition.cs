using Rush;
using System;
using UnityEngine;

namespace LegionKnight.Prototype
{
    [CreateAssetMenu(fileName = "Mail", menuName = "Legion Knight/Mail")]
    public partial class MailDefinition : ScriptableObject, IDescriptable
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField]
        private bool m_IsSaidPlayerNameInDescription = false;
        [SerializeField, TextArea]
        private string m_Description;
        [SerializeField]
        private MailState m_StartingState = MailState.Hide;
        [SerializeField]
        private LootField[] m_Rewards;

        [Header("Redeem Code")]
        [SerializeField]
        private bool m_HasExpiredDate = false;
        [SerializeField]
        private string m_ExpiredDate = "10/10/2025";
        [SerializeField]
        private bool m_ForSpecificPlayer = false;
        [SerializeField, TextArea]
        private string m_PlayerId = "Input player id here, ask the player to share their id";
        public string Id => m_Id;
        public string Label => m_Label;
        public string Description
        {
            get
            {
                if (m_IsSaidPlayerNameInDescription)
                {
                    string formatName = $"{Player.Instance.PlayerName}";
                    return string.Format(m_Description, formatName);
                }
                else
                {
                    return m_Description;
                }
            }
        }
        public MailState StartingState => m_StartingState;
        public LootField[] Rewards => m_Rewards;
        public bool ForSpecificPlayer => m_ForSpecificPlayer;
        public bool HasExpiredDate => m_HasExpiredDate;
        public DateTime ExpiredDate => DateTime.ParseExact(m_ExpiredDate, "dd/mm/yyyy", null);
        public bool HasRewards()
        {
            return m_Rewards.Length > -1;
        }
    }

    public enum MailState
    {
        Hide = 0,
        New = 1,
        Read = 2,
        Delete = 3,
    }
}
