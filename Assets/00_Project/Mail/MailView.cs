using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class MailView : MonoBehaviour
    {
        public TextMeshProUGUI m_TitleText;
        public TextMeshProUGUI m_MessageText;
        public Button m_ClaimButton;

        private long m_MailId;

        public void Setup(Mail mail, Action<long> onClaim)
        {
            m_MailId = mail.Id;
            m_TitleText.text = mail.Title;
            m_MessageText.text = mail.Message;

            if (mail.IsRead)
            {
                m_ClaimButton.interactable = false;
                m_ClaimButton.GetComponentInChildren<TextMeshProUGUI>().text = "Read";
            }
            else
            {
                m_ClaimButton.interactable = true;
                m_ClaimButton.GetComponentInChildren<TextMeshProUGUI>().text = "Claim";
            }

            m_ClaimButton.onClick.RemoveAllListeners();
            m_ClaimButton.onClick.AddListener(() => onClaim(m_MailId));
        }
    }
}
