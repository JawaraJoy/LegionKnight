using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

namespace LegionKnight
{
    public class MailAction : MonoBehaviour
    {
        // Save the given list as an array JSON to Cloud Save
        private async Task SaveInboxListAsync(List<Mail> mails)
        {
            // JsonUtility produces {"mails":[...]} so extract only the array part:
            MailListWrapper wrapper = new(mails);
            string wrapperJson = JsonUtility.ToJson(wrapper);
            int startIndex = wrapperJson.IndexOf('[');
            int endIndex = wrapperJson.LastIndexOf(']');
            string arrayJson = wrapperJson.Substring(startIndex, endIndex - startIndex + 1);

            Dictionary<string, object> toSave = new()
            {
                { "inbox", arrayJson }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(toSave);
        }

        // Mark a mail as read and save
        public async Task<bool> MarkMailAsRead(long mailId)
        {
            Mailbox mailbox = new Mailbox();
            List<Mail> mails = await mailbox.LoadInbox();

            Mail mailToUpdate = mails.Find(m => m.Id == mailId);
            if (mailToUpdate == null)
            {
                return false;
            }

            mailToUpdate.SetIsRead(true);
            await SaveInboxListAsync(mails);
            return true;
        }
    }
}
