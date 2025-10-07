using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class Mail
    {
        [SerializeField]
        private long m_Id;
        [SerializeField]
        private string m_Title;
        [SerializeField]
        private string m_Message;
        [SerializeField]
        private string m_RewardId;
        [SerializeField]
        private string m_Timestamp;
        [SerializeField]
        private bool m_IsRead;

        public long Id => m_Id;
        public string Title => m_Title;
        public string Message => m_Message;
        public string RewardId => m_RewardId;
        public string Timestamp => m_Timestamp;
        public bool IsRead => m_IsRead;

        public void SetIsRead(bool isRead)
        {
            m_IsRead = isRead;
        }
    }

    [System.Serializable]
    public class MailListWrapper
    {
        [SerializeField]
        private List<Mail> m_Mails = new();
        public List<Mail> Mails => m_Mails;

        public MailListWrapper(List<Mail> mails)
        {
            m_Mails = mails;
        }
    }

    public class Mailbox : MonoBehaviour
    {
        async void Start()
        {
            var mails = await LoadInbox();
            Debug.Log($"Inbox count: {mails.Count}");
            foreach (var m in mails)
                Debug.Log($"[{m.Id}] {m.Title} - {m.Message} (read:{m.IsRead})");
        }

        public async Task<List<Mail>> LoadInbox()
        {
            HashSet<string> keys = new HashSet<string>() { "inbox" };

            Dictionary<string, Item> result =
                await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (result.TryGetValue("inbox", out Item inboxItem))
            {
                // Option A (preferred if Cloud Code saved a real array): deserialize directly
                try
                {
                    List<Mail> directList = inboxItem.Value.GetAs<List<Mail>>();
                    if (directList != null)
                    {
                        return directList;
                    }
                }
                catch
                {
                    // ignore and fall back to string parsing
                }

                // Option B (fallback): read as string and parse with JsonUtility
                string jsonArray = inboxItem.Value.GetAsString(); // e.g. "[{...}, {...}]"
                string wrapped = "{\"mails\":" + jsonArray + "}";
                MailListWrapper wrapper = JsonUtility.FromJson<MailListWrapper>(wrapped);
                if (wrapper != null && wrapper.Mails != null)
                {
                    return wrapper.Mails;
                }
            }

            return new List<Mail>();
        }
    }
}
