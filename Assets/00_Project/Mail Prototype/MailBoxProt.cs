using MoreMountains.Tools;
using UnityEngine;

namespace LegionKnight.Prototype
{
    public class MailBoxProt : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private MailDefinition m_SelectedMail;
        [SerializeField]
        private MailField[] m_Mails;
        public MailField[] Mails => m_Mails;
        public MailDefinition SelectedMail => m_SelectedMail;

        public MailField GetSelectedMail()
        {
            if (HasMailInternal(m_SelectedMail.Id, out MailField mail))
            {
                return mail;
            }
            return null;
        }
        public void Init()
        {
            foreach (MailField m in m_Mails)
            {
                m.Init();
            }
        }
        private MailField GetMail(string id)
        {
            foreach (MailField m in m_Mails)
            {
                if (m.Definition.Id == id)
                {
                    return m;
                }
            }
            return null;
        }
        private bool HasMailInternal(string id, out MailField mail)
        {
            mail = GetMail(id);
            return mail != null;
        }
        public bool HasMail(string id, out MailField mail)
        {
            return HasMailInternal(id, out mail);
        }

        public void SetSelectedMail(MailDefinition defi)
        {
            if (HasMailInternal(defi.Id, out MailField mail))
            {
                m_SelectedMail = mail.Definition;
            }
        }
    }

    [System.Serializable]
    public class MailField
    {
        [SerializeField]
        private MailDefinition m_Definition;
        [SerializeField]
        private bool m_HasClaim = false;
        [SerializeField, MMReadOnly]
        private MailState m_State;
        public MailDefinition Definition => m_Definition;
        public bool HasClaim => m_HasClaim;
        public MailState State => m_State;

        private string STATEKEY => $"state{m_Definition.Id}";
        private string CLAIMKEY => $"claim{m_Definition.Id}";
        public void Init()
        {
            m_State = m_Definition.StartingState;
            bool hasStateData = UnityService.Instance.HasData(STATEKEY);
            bool hasClaimData = UnityService.Instance.HasData(CLAIMKEY);
            if (hasStateData) 
            {
                m_State = (MailState)UnityService.Instance.GetData<int>(STATEKEY);
            }
            if (hasClaimData)
            {
                m_HasClaim = UnityService.Instance.GetData<bool>(CLAIMKEY);
            }
        }
        public void NewMail()
        {
            m_State = MailState.New;
            UpdateState();
        }
        public void HideMail()
        {
            m_State = MailState.Hide;
            UpdateState();
        }
        public void ReadMail()
        {
            m_State = MailState.Read;
            UpdateState();
        }

        private LootedPanel m_LootPanel;
        private LootedPanel GetLootedPanel()
        {
            if (m_LootPanel == null)
            {
                m_LootPanel = GameManager.Instance.GetPanel<LootedPanel>();
            }
            return m_LootPanel;
        }
        public void ClaimReward()
        {
            if (m_HasClaim == true) return;
            m_HasClaim = true;
            foreach (LootField loot in m_Definition.Rewards)
            {
                loot.DirectTakeLoot();
            }
            GetLootedPanel().ShowLoot(m_Definition.Rewards);
        }
        public void DeleteMail()
        {
            m_State = MailState.Delete;
            UpdateState();
        }

        private void UpdateState()
        {
            UnityService.Instance.SaveData(STATEKEY, (int)m_State);
        }
    }
}
