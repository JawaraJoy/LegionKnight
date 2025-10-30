using UnityEngine;

namespace LegionKnight.Prototype
{
    public class MailBoxChannel{  }

    public partial class MailDefinition
    {
        private static PlayerMailBoxProt m_MailBox;
        private static MailBoxProtPanel m_MailBoxProtPanel;
        private static MailBoxReadProtPanel m_MailBoxReadProtPanel;
        private static PlayerMailBoxProt GetMailBox()
        {
            if (m_MailBox == null)
            {
                m_MailBox = Player.Instance.MailBox;
            }
            return m_MailBox;
        }
        private static MailBoxProtPanel GetBoxProtPanel()
        {
            if (m_MailBoxProtPanel == null)
            {
                m_MailBoxProtPanel = GameManager.Instance.GetPanel<MailBoxProtPanel>();
            }
            return m_MailBoxProtPanel;
        }
        private static MailBoxReadProtPanel GetBoxReadProtPanel()
        {
            if (m_MailBoxReadProtPanel == null)
            {
                m_MailBoxReadProtPanel = GameManager.Instance.GetPanel<MailBoxReadProtPanel>();
            }
            return m_MailBoxReadProtPanel;
        }
        public string StateClaimRewardText()
        {
            string hasClaimText = "Claimed";
            string hasntClaimText = "Claimable";
            string noRewardText = "No Reward!";
            if (GetMailBox().HasMail(m_Id, out MailField mail))
            {
                if (mail.Definition.HasRewards())
                {
                    if (mail.HasClaim)
                    {
                        return hasClaimText;
                    }
                    else
                    {
                        return hasntClaimText;
                    }
                }
                else
                {
                    return noRewardText;
                }
            }
            return noRewardText;
        }
        public bool HasClaim()
        {
            if (GetMailBox().HasMail(m_Id, out MailField mail))
            {
                if (mail.Definition.HasRewards())
                {
                    return mail.HasClaim;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public MailState GetMailState()
        {
            if (GetMailBox().HasMail(m_Id, out MailField mail))
            {
                return mail.State;
            }
            return default;
        }
        public void Init()
        {
            GetMailBox().Init();
        }
        private void InitSelfInternal()
        {
            if (GetMailBox().HasMail(m_Id, out MailField mail))
            {
                mail.Init();
            }
            if (GetBoxProtPanel().HasMailView(this, out MailSimpleProtView view))
            {
                view.Init(this);
            }
            GetBoxReadProtPanel().Refresh();
        }
        public void NewMail()
        {
            if (GetMailBox().HasMail(m_Id, out MailField mail))
            {
                mail.NewMail();
                InitSelfInternal();
            }
        }
        public void HideMail()
        {
            if (GetMailBox().HasMail(m_Id, out MailField mail))
            {
                mail.HideMail();
                InitSelfInternal();
            }
        }
        public void ReadMail()
        {
            if (GetMailBox().HasMail(m_Id, out MailField mail))
            {
                mail.ReadMail();
                GetBoxReadProtPanel().ReadMail(this);
                InitSelfInternal();
            }
        }
        public void ClaimReward()
        {
            if (GetMailBox().HasMail(m_Id, out MailField mail))
            {
                mail.ClaimReward();
                InitSelfInternal();
            }
        }
        public void DeleteMail()
        {
            if (GetMailBox().HasMail(m_Id, out MailField mail))
            {
                mail.DeleteMail();
                InitSelfInternal();
            }
        }
    }
}
