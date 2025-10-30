using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

namespace LegionKnight
{
    public class MailBoxMonitor : MonoBehaviour
    {
        [SerializeField]
        private Transform m_ContentParent;       // ScrollView content
        [SerializeField]
        private MailView m_MailItemPrefab;     // MailItemPrefab

        private AdminMailSender m_AdminSender;

        private async void Init()
        {
            m_AdminSender = gameObject.AddComponent<AdminMailSender>();
            await PopulateInbox();
        }

        public async Task PopulateInbox()
        {
            Mailbox mailbox = gameObject.AddComponent<Mailbox>();
            List<Mail> mails = await mailbox.LoadInbox();

            // Clear existing
            for (int i = m_ContentParent.childCount - 1; i >= 0; i--)
            {
                Destroy(m_ContentParent.GetChild(i).gameObject);
            }

            for (int i = 0; i < mails.Count; i++)
            {
                Mail mail = mails[i];
                MailView item = Instantiate(m_MailItemPrefab, m_ContentParent);
                item.Setup(mail, async (long mailId) =>
                {
                    // Call Cloud Code to mark read (server-side)
                    await m_AdminSender.MarkMailReadViaCloudCode(AuthenticationService.Instance.PlayerId, mailId);

                    // Refresh UI
                    await PopulateInbox();
                });
            }
        }
    }
}
