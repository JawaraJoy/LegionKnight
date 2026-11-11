using System;
using LegionKnight;
using LegionKnight.Prototype;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Rush
{
    public abstract class InboxMailRowView : UIView
    {
        [Serializable]
        private sealed class PreviewField
        {
            [SerializeField] private LootItemView[] m_Slots;
            [SerializeField] private GameObject m_ExtraBadge;
            [SerializeField] private TMP_Text m_ExtraCount;

            public LootItemView[] Slots => m_Slots;
            public GameObject ExtraBadge => m_ExtraBadge;
            public TMP_Text ExtraCount => m_ExtraCount;
        }

        [Serializable]
        private sealed class StateField
        {
            [SerializeField] private GameObject m_Background;
            [SerializeField] private Color m_StatusColor = Color.white;

            public GameObject Background => m_Background;
            public Color StatusColor => m_StatusColor;
        }

        private const string c_OverflowFormat = "+{0}";
        private const string c_NoRewards = "No rewards";
        [SerializeField] private AssetReferenceGameObject m_MailItemViewAsset;
        [SerializeField] private TextMeshProUGUI m_LabelText;
        [SerializeField] private TextMeshProUGUI m_StatusText;
        [SerializeField] private TextMeshProUGUI m_RewardStateText;
        [SerializeField] private Transform m_ItemViewContainer;
        [SerializeField] private PreviewField m_Preview = new();
        [SerializeField] private StateField m_New = new();
        [SerializeField] private StateField m_Read = new();

        private readonly InboxRewardItems m_LegacyItems = new();
        private MailDefinition m_Definition;
        private MailState? m_LastState;
        public MailDefinition Definition => m_Definition;
        public event Action StateChanged;

        public void Init(MailDefinition definition)
        {
            BindInternal(definition);
        }

        public void ReadMail()
        {
            m_Definition.ReadMail();
        }

        private void BindInternal(MailDefinition definition)
        {
            m_Definition = definition;
            MailState state = definition.GetMailState();
            bool changed = m_LastState != state;
            m_LastState = state;
            if (state == MailState.Hide || state == MailState.Delete)
            {
                HideInternal();
            }
            else
            {
                var rewards = definition.Rewards ?? Array.Empty<LootField>();
                m_LabelText.text = definition.Label;
                m_StatusText.text = state.ToString();
                m_RewardStateText.text = rewards.Length == 0 ? c_NoRewards : definition.StateClaimRewardText();
                ShowInternal();
                if (m_Preview.Slots != null && m_Preview.Slots.Length > 0)
                {
                    SetPreviewInternal(rewards, state == MailState.New);
                }
                else
                {
                    m_LegacyItems.SetItems(rewards, m_ItemViewContainer, null, m_MailItemViewAsset);
                }
            }

            if (changed) StateChanged?.Invoke();
        }

        private void SetPreviewInternal(LootField[] rewards, bool unread)
        {
            m_New.Background.SetActive(unread);
            m_Read.Background.SetActive(!unread);
            m_StatusText.color = unread ? m_New.StatusColor : m_Read.StatusColor;
            var slots = m_Preview.Slots;
            for (int i = 0; i < slots.Length; i++)
            {
                bool visible = i < rewards.Length;
                slots[i].gameObject.SetActive(visible);
                if (visible) slots[i].Init(rewards[i]);
            }
            int extra = rewards.Length - slots.Length;
            m_Preview.ExtraBadge.SetActive(extra > 0);
            if (extra > 0) m_Preview.ExtraCount.SetText(c_OverflowFormat, extra);
        }
    }
}
