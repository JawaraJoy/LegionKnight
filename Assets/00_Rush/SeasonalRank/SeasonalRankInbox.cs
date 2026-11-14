using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class SeasonalRankInbox : MonoBehaviour
    {
        [Serializable] private sealed class PopupField
        {
            [SerializeField] internal RectTransform surface;
            [SerializeField] internal TMP_Text title, period, rank, caption, quantity, actionText, secondaryText;
            [SerializeField] internal Button action, secondary;
        }
        [Serializable] private sealed class LetterField
        {
            [SerializeField] internal GameObject group;
            [SerializeField] internal TMP_Text title, rank, trophies, quantity;
            [SerializeField] internal GameObject[] summary;
            internal void Bind(bool detail, RankReward reward)
            {
                group.SetActive(detail);
                foreach(var item in summary)item.SetActive(!detail);
                if(!detail)return;
                title.text=reward.label+" Rewards";
                rank.text="#"+reward.rank;
                trophies.text=reward.score.ToString("N0");
                quantity.text="×"+reward.amount.ToString("N0");
            }
        }
        [SerializeField] private LetterField m_Letter;
        [Serializable] private sealed class MailField
        {
            [SerializeField] internal Transform list;
            [SerializeField] internal Button template;
            [SerializeField] internal InboxListView inbox;
            [SerializeField] internal HomePanel home;
        }
        [SerializeField] private GameObject m_Overlay;
        [SerializeField] private PopupField m_Popup;
        [SerializeField] private AudioSource m_RankRewardSound;
        [SerializeField] private AudioClip m_LetterOpenClip;
        [SerializeField] private MailField m_Mail;
        private readonly Dictionary<string,Button> m_Rows=new();
        private SeasonalRanks m_Ranks;
        private string m_Owner,m_RewardId,m_Error;
        private RankReward[] m_Pending=Array.Empty<RankReward>();
        private bool m_Busy,m_Later,m_Detail,m_Refresh=true,m_InboxWasVisible;
        private void Awake()
        {
            m_Popup.action.onClick.AddListener(Confirm);
            m_Popup.secondary.onClick.AddListener(Close);
        }
        private void OnDestroy(){if(m_Ranks!=null)m_Ranks.Changed-=Changed;}
        private void Changed()=>m_Refresh=true;
        private void Update()
        {
            if(m_Ranks!=SeasonalRanks.Current)
            {
                if(m_Ranks!=null)m_Ranks.Changed-=Changed;
                m_Ranks=SeasonalRanks.Current;
                if(m_Ranks!=null)m_Ranks.Changed+=Changed;
                m_Refresh=true;
            }
            if(m_Ranks==null)return;
            if(m_Owner!=SeasonalRanks.PlayerId)
            {
                m_Owner=SeasonalRanks.PlayerId;m_RewardId=null;m_Later=false;m_Refresh=true;
                m_Overlay.SetActive(false);
                foreach(var b in m_Rows.Values)Destroy(b.gameObject);
                m_Rows.Clear();
            }
            bool inbox=m_Mail.inbox.Content.activeInHierarchy;
            if(inbox&&!m_InboxWasVisible){m_Ranks.RefreshMail();m_Refresh=true;}
            m_InboxWasVisible=inbox;
            if(m_Refresh){m_Refresh=false;Rebuild();}
            if(!m_Busy&&!m_Later&&!m_Overlay.activeSelf&&m_Pending.Length>0&&m_Mail.home.Content.activeInHierarchy&&!inbox)
                Open(null);
            if(m_Overlay.activeSelf)
            {
                var bounds=((RectTransform)m_Overlay.transform).rect;
                float scale=Mathf.Min(bounds.width/420f,bounds.height/590f);
                if(scale>0)m_Popup.surface.localScale=new Vector3(scale,scale,1);
            }
        }
        private void Rebuild()
        {
            var rewards=m_Ranks.Mail.OrderByDescending(m=>m.id.Substring(m.id.IndexOf('_')+1)).ToArray();
            m_Pending=rewards.Where(m=>m.state=="pending").ToArray();
            foreach(var reward in rewards.Where(m=>m.state!="pending"))
            {
                if(!m_Rows.TryGetValue(reward.id,out var row))
                {
                    string id=reward.id;
                    row=Instantiate(m_Mail.template,m_Mail.list,false);
                    row.onClick.AddListener(()=>Open(id));m_Rows[id]=row;
                    row.transform.SetAsFirstSibling();row.gameObject.SetActive(true);
                }
                row.GetComponentInChildren<TMP_Text>(true).text=reward.label+" Rewards\n"+
                    (reward.state=="claimed"?"Claimed":"Claimable")+"    •    "+reward.amount.ToString("N0")+" Diamonds";
            }
            m_Mail.inbox.RefreshRankUnread();
            if(m_Overlay.activeSelf&&!m_Busy)
            {
                if(!m_Detail&&m_Pending.Length==0)Close();else Render();
            }
        }
        private RankReward Detail=>m_Ranks.Mail.FirstOrDefault(m=>m.id==m_RewardId);
        private void Open(string id)
        {
            bool detail = id != null;
            bool opening = !m_Overlay.activeSelf || m_Detail != detail;
            m_Error = null;
            m_Detail = detail;
            m_RewardId = id;
            transform.SetAsLastSibling();
            m_Overlay.transform.SetAsLastSibling();
            m_Overlay.SetActive(true);
            Render();
            if (!opening || !m_Overlay.activeInHierarchy || m_RankRewardSound == null) return;
            m_RankRewardSound.Stop();
            if (!m_Detail) m_RankRewardSound.Play();
            else if (m_LetterOpenClip != null) m_RankRewardSound.PlayOneShot(m_LetterOpenClip);
        }
        private void Render()
        {
            var reward=m_Detail?Detail:m_Pending.FirstOrDefault();
            if(reward==null){Close();return;}
            m_Letter.Bind(m_Detail,reward);
            bool grouped=!m_Detail&&m_Pending.Length>1;
            m_Popup.title.text=m_Detail?"Inbox":"Rank Rewards";
            m_Popup.period.text=grouped?m_Pending.Length+" rank rewards":reward.label+(m_Detail?" Rewards":" complete");
            m_Popup.rank.text=grouped?"×"+m_Pending.Length:"#"+reward.rank;
            m_Popup.caption.text=grouped?string.Join("\n",m_Pending.Take(3).Select(m=>m.label+"   #"+m.rank+"   •   "+m.amount.ToString("N0")))+(m_Pending.Length>3?"\n+"+(m_Pending.Length-3)+" more":""):
                "Final rank   •   "+reward.score.ToString("N0")+" trophies";
            if(!string.IsNullOrEmpty(m_Error)) m_Popup.caption.text=m_Error;
            long amount=grouped?m_Pending.Sum(m=>(long)m.amount):reward.amount;
            m_Popup.quantity.text=amount.ToString("N0")+" Diamonds";
            m_Popup.actionText.text=m_Detail?(reward.state=="claimed"?"Claimed":string.IsNullOrEmpty(m_Error)?"Claim":"Try Again"):"Send to Inbox";
            m_Popup.secondaryText.text=m_Detail?"Close":"Later";
            m_Popup.action.interactable=!m_Busy&&(!m_Detail||reward.state=="inbox");
            m_Popup.secondary.interactable=!m_Busy;
        }
        private void Close()
        {
            if(m_Busy)return;
            if(!m_Detail)m_Later=true;
            m_Overlay.SetActive(false);
        }
        private async void Confirm()
        {
            if(m_Busy)return;
            var id=m_Owner;
            m_Error=null;m_Busy=true;m_Popup.action.interactable=false;m_Popup.secondary.interactable=false;
            try
            {
                if(m_Detail)await m_Ranks.Act("claim",m_RewardId);
                else foreach(var reward in m_Pending.ToArray())
                {
                    if(id!=SeasonalRanks.PlayerId)throw new OperationCanceledException();
                    await m_Ranks.Act("send",reward.id);
                }
                if(this==null||id!=SeasonalRanks.PlayerId)return;
                if(!m_Detail){m_Overlay.SetActive(false);m_Mail.inbox.Show();}
            }
            catch(Exception ex)
            {
                if(this!=null&&id==SeasonalRanks.PlayerId)m_Error="Connection interrupted. Tap to retry.";
                Debug.LogWarning("[Seasonal Rank] "+ex.Message);
            }
            finally
            {
                if(this!=null){m_Busy=false;m_Refresh=true;m_Popup.secondary.interactable=true;}
            }
        }
    }
}
