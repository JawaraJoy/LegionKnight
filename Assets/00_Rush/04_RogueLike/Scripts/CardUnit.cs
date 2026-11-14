using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class CardUnit
    {
        [SerializeField, MMReadOnly]
        private bool m_IsOwned;
        [SerializeField, MMReadOnly]
        private bool m_IsAdded;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private CardConfig m_CardConfig;

        public const int MaxUpgrade = 12;
        private const float c_BaseGrowthPercent = 15f;
        internal static float ExtraGrowthSteps(int rank) => Mathf.Clamp(rank, 0, MaxUpgrade) / c_BaseGrowthPercent;
        internal static float GrowthOf(ISkill skill) => skill is Skill runtime ? runtime.CardGrowthSteps : 0f;
        private int m_Upgrade;
        public int Upgrade => m_Upgrade;
        public bool IsMax => m_Upgrade >= MaxUpgrade;
        public int UpgradeCost => IsMax ? 0 : 1 << m_Upgrade;
        public bool CanUpgrade => IsOwned && !IsMax && m_Amount >= UpgradeCost && !m_CardConfig.IsBattlePoolOnly;
        public bool IsOwned => m_IsOwned;
        private string CollectionKey => "card.collection.v1." + m_CardConfig.BaseInfo.Id;

        [System.Serializable]
        private sealed class SavedCard
        {
            [SerializeField] internal int copies;
            [SerializeField] internal int upgrade;
            [SerializeField] internal bool heroGranted;
        }
        public bool IsAdded => m_IsAdded;
        public int Amount => m_Amount;
        public CardConfig CardConfig => m_CardConfig;

        private string AmountKey => m_CardConfig.BaseInfo.Id + "amount";
        private int m_DeckSlot = -1;
        public int DeckSlot => m_DeckSlot;
        public void AssignSlot(int index)
        {
            m_DeckSlot = index;
            if (m_IsAdded) UnityService.Instance.SaveData(UsedKey + ".slot", index);
        }

        private string UsedKey => m_CardConfig.BaseInfo.Id + "used";

        public CardUnit(CardConfig cardConfig)
        {
            m_CardConfig = cardConfig;
            m_IsOwned = false;
            m_IsAdded = false;
        }

        internal void ChangeCopies(int add, UnityEngine.Events.UnityAction changed)
        {
            if (add <= 0) return;
            var save = UnityService.Instance.LocalSave;
            save.ExecuteTransaction(() =>
            {
                var state = save.GetDataValue<SavedCard>(CollectionKey);
                bool owned = state != null;
                state ??= new SavedCard();
                state.copies = checked(state.copies + (owned ? add : add - 1));
                SaveInternal(state, changed);
            });
        }

        public void AddAmount(int add) => ChangeCopies(add, null);

        internal void UpgradeWithCopies(int expected, UnityEngine.Events.UnityAction changed)
        {
            var save = UnityService.Instance.LocalSave;
            save.ExecuteTransaction(() =>
            {
                var state = save.GetDataValue<SavedCard>(CollectionKey);
                if (state == null || state.upgrade != expected || expected < 0 || expected >= MaxUpgrade || m_CardConfig.IsBattlePoolOnly) return;
                int cost = 1 << expected;
                if (state.copies < cost) return;
                state.copies -= cost;
                state.upgrade++;
                SaveInternal(state, changed);
            });
        }

        private void SaveInternal(SavedCard state, UnityEngine.Events.UnityAction changed)
        {
            UnityService.Instance.LocalSave.SaveData(CollectionKey, state, callback: () =>
            {
                m_IsOwned = true;
                m_Amount = state.copies;
                m_Upgrade = state.upgrade;
                changed?.Invoke();
            });
        }

        internal void GrantHeroCard(UnityEngine.Events.UnityAction changed)
        {
            var save = UnityService.Instance.LocalSave;
            save.ExecuteTransaction(() =>
            {
                var state = save.GetDataValue<SavedCard>(CollectionKey);
                if (state?.heroGranted == true) return;
                if (state == null) state = new SavedCard();
                else state.copies = checked(state.copies + 1);
                state.heroGranted = true;
                SaveInternal(state, changed);
            });
        }

        public void SetIsAdded(bool set)
        {
            m_IsAdded = set;
            if (!set) m_DeckSlot = -1;
            UnityService.Instance.LocalSave.ExecuteTransaction(() =>
            {
                UnityService.Instance.SaveData(UsedKey, m_IsAdded);
                UnityService.Instance.SaveData(UsedKey + ".slot", m_DeckSlot);
            });
        }

        public void Init() => Init(false);
        internal void Init(bool heroEntitled)
        {
            var save = UnityService.Instance.LocalSave;
            var state = save.GetDataValue<SavedCard>(CollectionKey);
            int legacy = save.HasData(AmountKey) ? save.GetDataValue<int>(AmountKey) : m_Amount;
            if (state == null && (legacy > 0 || heroEntitled))
            {
                state = new SavedCard { copies = Mathf.Max(0, legacy - (heroEntitled ? 0 : 1)), heroGranted = heroEntitled };
                save.ExecuteTransaction(() => SaveInternal(state, null));
            }
            else if (heroEntitled && state != null && !state.heroGranted)
            {
                GrantHeroCard(null);
                state = save.GetDataValue<SavedCard>(CollectionKey);
            }
            m_IsOwned = state != null;
            m_Upgrade = Mathf.Clamp(state?.upgrade ?? 0, 0, MaxUpgrade);
            m_Amount = Mathf.Max(0, state?.copies ?? 0);

            if (UnityService.Instance.HasData(UsedKey))
                m_IsAdded = UnityService.Instance.GetData<bool>(UsedKey);

            m_DeckSlot = m_IsAdded && UnityService.Instance.HasData(UsedKey + ".slot")
                ? UnityService.Instance.GetData<int>(UsedKey + ".slot") : -1;
        }
    }
}
