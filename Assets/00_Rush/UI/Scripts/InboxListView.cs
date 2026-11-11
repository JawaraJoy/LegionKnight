using System.Collections;
using System.Collections.Generic;
using LegionKnight;
using LegionKnight.Prototype;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Rush
{
    public abstract class InboxListView : PanelView
    {
        [SerializeField] private AssetReferenceGameObject m_MailSimpleViewAsset;
        [SerializeField] private Transform m_MailViewContainer;
        [SerializeField] private TMP_InputField m_RedeemInput;
        [SerializeField] private Button m_RedeemConfirmButton;
        [SerializeField] private MailSimpleProtView m_RowTemplate;
        [SerializeField] private TMP_Text m_UnreadCount;

        private const string c_UnreadFormat = "{0} new";
        private readonly Dictionary<MailDefinition, MailSimpleProtView> m_Rows = new();
        private readonly HashSet<MailDefinition> m_Loading = new();
        private PlayerMailBoxProt m_Mailbox;

        protected void Awake()
        {
            m_Mailbox = Player.Instance.MailBox;
            m_RedeemInput.onEndEdit.AddListener(m_Mailbox.SetRedeemCodeInput);
            m_RedeemConfirmButton.onClick.AddListener(ConfirmCodeInternal);
            RefreshListInternal();
        }

        public override void Show()
        {
            base.Show();
            RefreshListInternal();
        }

        public bool HasMailView(MailDefinition definition, out MailSimpleProtView view)
        {
            return m_Rows.TryGetValue(definition, out view);
        }

        private void RefreshListInternal()
        {
            foreach (MailField mail in m_Mailbox.Mails)
            {
                MailDefinition definition = mail.Definition;
                if (m_Rows.TryGetValue(definition, out MailSimpleProtView row))
                {
                    row.Init(definition);
                }
                else if (m_RowTemplate != null)
                {
                    RegisterRowInternal(definition, Instantiate(m_RowTemplate, m_MailViewContainer, false));
                }
                else if (m_Loading.Add(definition))
                {
                    StartCoroutine(LoadRowInternal(definition));
                }
            }

            RefreshUnreadInternal();
        }

        private void RegisterRowInternal(MailDefinition definition, MailSimpleProtView row)
        {
            m_Rows.Add(definition, row);
            row.StateChanged += RefreshUnreadInternal;
            row.Init(definition);
        }

        private IEnumerator LoadRowInternal(MailDefinition definition)
        {
            var handle = m_MailSimpleViewAsset.InstantiateAsync(m_MailViewContainer);
            yield return handle;
            m_Loading.Remove(definition);
            if (handle.Status == AsyncOperationStatus.Succeeded &&
                handle.Result.TryGetComponent(out MailSimpleProtView row))
            {
                RegisterRowInternal(definition, row);
            }
            else if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Addressables.ReleaseInstance(handle);
            }
            else
            {
                Addressables.Release(handle);
            }
        }

        private void RefreshUnreadInternal()
        {
            if (m_UnreadCount == null) return;

            int unread = 0;
            foreach (MailField mail in m_Mailbox.Mails)
            {
                if (mail.State == MailState.New) unread++;
            }
            m_UnreadCount.SetText(c_UnreadFormat, unread);
        }

        private void ConfirmCodeInternal()
        {
            m_Mailbox.SetRedeemCodeInput(m_RedeemInput.text);
            m_Mailbox.TryToRedeem();
        }
    }
}
