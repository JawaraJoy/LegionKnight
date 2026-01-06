using MoreMountains.Tools;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight.Prototype
{
    public partial class MailBoxProt : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private MailDefinition m_SelectedMail;
        [SerializeField, MMReadOnly]
        private string m_RedeemCodeInput;
        [SerializeField]
        private MailField[] m_Mails;
        public MailField[] Mails => m_Mails;
        public MailDefinition SelectedMail => m_SelectedMail;

        [SerializeField]
        private UnityEvent<MailField> m_OnRedeemCodeSucced = new();
        [SerializeField]
        private UnityEvent<MailField> m_OnReeemCodeFailed = new();
        [SerializeField]
        private UnityEvent<MailField> m_OnHasNewMail = new();

        private TextPopUpPanel m_PopUpPanel;

        private HomePanel m_HomePanel;
        private CommonUIView m_NotifView;

        private HomePanel GetHomePanel()
        {
            if (m_HomePanel == null)
            {
                m_HomePanel = CanvasManager.Instance.GetPanel<HomePanel>();
            }
            return m_HomePanel;
        }
        private CommonUIView GetNotifView()
        {
            if (m_NotifView == null)
            {
                m_NotifView = GetHomePanel().GetBinding<CommonUIView>();
            }
            return m_NotifView;
        }
        private TextPopUpPanel GetPopUpPanel()
        {
            if (m_PopUpPanel == null)
            {
                m_PopUpPanel = CanvasManager.Instance.GetPanel<TextPopUpPanel>();
            }
            return m_PopUpPanel;
        }
        public void SetRedeemCodeInput(string set)
        {
            m_RedeemCodeInput = set;
        }
        private void OnRedeemCodeSuccedInvoke(MailField mail)
        {
            m_OnRedeemCodeSucced?.Invoke(mail);
            if (mail.State == MailState.Hide)
            {
                GetPopUpPanel().ShowText("Redeem Code was Success");
            }
            else
            {
                GetPopUpPanel().ShowText("Redeem Code already use");
            }
        }
        private void OnRedeemCodeFailedInvoke(MailField mail)
        {
            m_OnReeemCodeFailed?.Invoke(mail);
            GetPopUpPanel().ShowText("Redeem Code is Invalid or Expired");
        }
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
                if (m.State == MailState.New)
                {
                    OnHasNewEmail(m);
                }
            }
        }

        private void OnHasNewEmail(MailField mail)
        {
            m_OnHasNewMail?.Invoke(mail);
            GetPopUpPanel().ShowText("You have new Mail!");
            GetNotifView().Show();
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
        public void TryToRedeem()
        {
            if (HasMailInternal(m_RedeemCodeInput, out MailField mail))
            {
                mail.TryToRedeem(m_RedeemCodeInput, OnRedeemCodeSuccedInvoke, OnRedeemCodeFailedInvoke);
            }
        }
    }

    [System.Serializable]
    public class MailField
    {
        [SerializeField]
        private MailDefinition m_Definition;
        [SerializeField, MMReadOnly]
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
        public void TryToRedeem(string code, UnityAction<MailField> onSuccess, UnityAction<MailField> onFail)
        {
            bool codeIsEqual = code == GetReedemCode();
            if (codeIsEqual)
            {
                onSuccess.Invoke(this);
                if (m_State == MailState.Hide && !HasExpired())
                {
                    NewMailInternal();
                }
            }
            else
            {
                onFail.Invoke(this);
            }
        }

        private string GetReedemCode()
        {
            bool hasSpecific = m_Definition.ForSpecificPlayer;
            if (hasSpecific)
            {
                return $"{m_Definition.Id}{UnityService.Instance.PlayerId}";
            }
            else
            {
                return m_Definition.Id;
            }
        }
        private bool HasExpired()
        {
            bool hasExpired = DateTime.Now >= m_Definition.ExpiredDate && m_Definition.HasExpiredDate;
            return hasExpired;
        }
        public void NewMail()
        {
            m_State = MailState.New;
            UpdateState();
            m_Definition.Init();
        }
        private void NewMailInternal()
        {
            m_State = MailState.New;
            m_Definition.NewMail();
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
                m_LootPanel = CanvasManager.Instance.GetPanel<LootedPanel>();
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
            UnityService.Instance.SaveData(CLAIMKEY, m_HasClaim);
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
