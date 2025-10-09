using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight.Prototype
{
    public class MailBoxProtPanel : PanelView
    {
        [SerializeField]
        private AssetReferenceGameObject m_MailSimpleViewAsset;
        [SerializeField]
        private Transform m_MailViewContainer;

        private readonly List<MailSimpleProtView> m_MailViews = new();

        private PlayerMailBoxProt m_Mail;
        private PlayerMailBoxProt GetMailBox()
        {
            if (m_Mail == null)
            {
                m_Mail = Player.Instance.MailBox;
            }
            return m_Mail;
        }
        public override void Show()
        {
            base.Show();
            MailField[] mails = GetMailBox().Mails;
            foreach (MailField mail in mails)
            {
                if (HasMailViewInternal(mail.Definition, out MailSimpleProtView view))
                {
                    view.Init(mail.Definition);
                }
                else
                {
                    StartCoroutine(SpawMailSimpleView(mail.Definition));
                }
            }
        }
        private MailSimpleProtView GetMailView(MailDefinition defi)
        {
            foreach (var item in m_MailViews)
            {
                if (item.Definition == defi)
                {
                    return item;
                }
            }
            return null;
        }

        private bool HasMailViewInternal(MailDefinition defi, out MailSimpleProtView view)
        {
            view = GetMailView(defi);
            return view != null;
        }
        public bool HasMailView(MailDefinition defi, out MailSimpleProtView view)
        {
            return HasMailViewInternal(defi, out view);
        }
        private IEnumerator SpawMailSimpleView(MailDefinition defi)
        {
            AsyncOperationHandle<GameObject> handle = m_MailSimpleViewAsset.InstantiateAsync(m_MailViewContainer, false);
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out MailSimpleProtView view))
                {
                    view.Init(defi);
                    m_MailViews.Add(view);
                }
            }
        }
    }
}
