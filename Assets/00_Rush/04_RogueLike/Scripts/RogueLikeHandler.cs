
using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class RogueLikeHandler : MonoBehaviour, IReseter
    {
        [SerializeField]
        private RogueLikeConfig m_Config;
        [SerializeField, MMReadOnly]
        private int m_ForPlayerCurrentLevel = 1;
        [SerializeField, MMReadOnly]
        private int m_ForPlayerCurrentExperience;
        [SerializeField, MMReadOnly]
        private HeroUnitConfig m_CurrentHero;
        [SerializeField, MMReadOnly]
        private List<CardConfig> m_CustomCards = new();
        [SerializeField, MMReadOnly]
        private List<CardConfig> m_CollectedCards = new();

        [SerializeField]
        private UnityEvent<int, int> m_OnForPlayerExperienceAdded;
        [SerializeField]
        private UnityEvent<int> m_OnForPlayerLevelUp;
        [SerializeField]
        private UnityEvent<int> m_OnPlayerLevelChanged;

        [SerializeField]
        private UnityEvent<int, int> m_OnForBossExperienceAdded;

        [SerializeField]
        private UnityEvent<int> m_OnForBossLevelUp;
        [SerializeField]
        private UnityEvent<CardConfig> m_OnCardCollected;
        private int m_PendingCardChoices;
        private bool m_IsCheckingExperience;
        public int PendingCardChoices => m_PendingCardChoices;
        public RogueLikeConfig Config => m_Config;
        public int ForPlayerCurrentExperience => m_ForPlayerCurrentExperience;
        public int ForPlayerCurrentLevel => m_ForPlayerCurrentLevel;
        public UnityEvent<int, int> OnForPlayerExperienceAdded => m_OnForPlayerExperienceAdded;
        public UnityEvent<int> OnForPlayerLevelUp => m_OnForPlayerLevelUp;
        public UnityEvent<int> OnPlayerLevelChanged => m_OnPlayerLevelChanged;

        public UnityEvent<int, int> OnForBossExperienceAdded => m_OnForBossExperienceAdded;
        public UnityEvent<int> OnForBossLevelUp => m_OnForBossLevelUp;
        public UnityEvent<CardConfig> OnCardCollected => m_OnCardCollected;
        public List<CardConfig> CustomCards => m_CustomCards;
        public List<CardConfig> CollectedCards => m_CollectedCards;

        private CardConfig GetCollectedCard(string cardID)
        {
            return m_CollectedCards.Find(card => card.BaseInfo.Id == cardID);
        }
        private bool HasCollectedCardInternal(string cardID, out CardConfig collectedCard)
        {
            collectedCard = GetCollectedCard(cardID);
            return collectedCard != null;
        }
        public bool HasCollectedCard(string cardID, out CardConfig collectedCard)
        {
            return HasCollectedCardInternal(cardID, out collectedCard);
        }
        public void AddCollectedCard(CardConfig cardConfig) => AddCollectedCardInternal(cardConfig);
        private void AddCollectedCardInternal(CardConfig cardConfig)
        {
            if (cardConfig != null && !HasCollectedCardInternal(cardConfig.BaseInfo.Id, out _))
                m_CollectedCards.Add(cardConfig);
        }
        public void NotifyCardCollected(CardConfig cardConfig)
        {
            if (cardConfig == null) return;
            AddCollectedCardInternal(cardConfig);
            OnCardCollectedInvoke(cardConfig);
        }
        private void OnCardCollectedInvoke(CardConfig cardConfig) => m_OnCardCollected?.Invoke(cardConfig);
        public void CompleteCardChoice()
        {
            if (m_PendingCardChoices > 0) m_PendingCardChoices--;
        }
        public void RemoveCollectedCard(CardConfig cardConfig)
        {
            if (HasCollectedCardInternal(cardConfig.BaseInfo.Id, out _))
            {
                m_CollectedCards.Remove(cardConfig);
            }
        }
        private void ClearCollectedCards()
        {
            m_CollectedCards.Clear();
        }   
        public List<CardConfig> GetRandomAvailableCards(int amount)
        {
            List<CardConfig> pool = new List<CardConfig>();

            // Pool dari hero deck
            if (m_CurrentHero != null && m_CurrentHero.HeroDeckConfig != null)
            {
                pool.AddRange(m_CurrentHero.HeroDeckConfig.CardConfigs);
            }

            // Tambahkan custom cards (hindari duplikat)
            foreach (var card in m_CustomCards)
            {
                if (card != null && card.IsAvailableForHero(m_CurrentHero) && !pool.Contains(card))
                {
                    pool.Add(card);
                }
            }

            pool.RemoveAll(card => card == null);
            for (int i = pool.Count - 1; i >= 0; i--)
                if (pool.IndexOf(pool[i]) != i) pool.RemoveAt(i);

            // Random pick tanpa duplikat
            List<CardConfig> result = new List<CardConfig>();
            for (int i = 0; i < amount && pool.Count > 0; i++)
            {
                int index = Random.Range(0, pool.Count);
                result.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return result;
        }
        public void SetCurrentHero(HeroUnitConfig heroConfig)
        {
            m_CurrentHero = heroConfig;
            LegionKnight.Player player = LegionKnight.Player.Instance;
            if (player != null && player.PlayerCardDeck != null)
                player.PlayerCardDeck.PrepareUsedCards();
        }
        public void AddCustomCard(CardConfig cardConfig)
        {
            AddCustomCardInternal(cardConfig);
        }
        private void AddCustomCardInternal(CardConfig cardConfig)
        {
            if (cardConfig != null && cardConfig.IsAvailableForHero(m_CurrentHero) && !m_CustomCards.Contains(cardConfig))
            {
                m_CustomCards.Add(cardConfig);
            }
        }
        public void AddCustomCards(List<CardConfig> cardConfigs)
        {
            foreach (var cardConfig in cardConfigs)
            {
                AddCustomCardInternal(cardConfig);
            }
        }
        public void RemoveCustomCards(List<CardConfig> cardConfigs)
        {
            foreach (var cardConfig in cardConfigs)
            {
                RemoveCustomCardInternal(cardConfig);
            }
        }
        public void RemoveCustomCard(CardConfig cardConfig)
        {
            RemoveCustomCardInternal(cardConfig);
        }
        private void RemoveCustomCardInternal(CardConfig cardConfig)
        {
            if (m_CustomCards.Contains(cardConfig))
            {
                m_CustomCards.Remove(cardConfig);
            }
        }
        private void ClearCustomCardsInternal()
        {
            m_CustomCards.Clear();
        }
        public void ClearCustomCards()
        {
            ClearCustomCardsInternal();
        }
        public void ResetProgression()
        {
            m_PendingCardChoices = 0;
            m_ForPlayerCurrentExperience = 0;
            ClearCustomCardsInternal();
            ClearCollectedCards();
            SetForPlayerLevel(1);
            OnForPlayerExperienceAddedInvoke(m_ForPlayerCurrentExperience);
        }
        public void AddForPlayerExperience(int amount)
        {
            if (amount <= 0) return;
            m_ForPlayerCurrentExperience = (int)System.Math.Min(int.MaxValue, (long)m_ForPlayerCurrentExperience + amount);
            CheckForPlayerLevelUp();
        }
        private void CheckForPlayerLevelUp()
        {
            if (m_IsCheckingExperience || m_Config == null || m_Config.ForPlayerLevelFormula == null) return;
            m_IsCheckingExperience = true;
            try
            {
                int maximumLevel = Mathf.Max(1, m_Config.ForPlayerLevelFormula.MaxLevel);
                m_ForPlayerCurrentLevel = Mathf.Clamp(m_ForPlayerCurrentLevel, 1, maximumLevel);
                while (m_ForPlayerCurrentLevel < maximumLevel)
                {
                    int required = Mathf.Max(1, m_Config.ForPlayerLevelFormula.GetCurrentMaxExperience(m_ForPlayerCurrentLevel + 1));
                    if (m_ForPlayerCurrentExperience < required) break;
                    m_ForPlayerCurrentExperience -= required;
                    m_PendingCardChoices++;
                    AddForPlayerLevel(1);
                }
                if (m_ForPlayerCurrentLevel >= maximumLevel) m_ForPlayerCurrentExperience = 0;
                OnForPlayerExperienceAddedInvoke(m_ForPlayerCurrentExperience);
            }
            finally
            {
                m_IsCheckingExperience = false;
            }
        }
        private void OnLevelChangedInvoke(int level)
        {
            if (level > 1) OnForPlayerLevelUpInvoke(level);
            OnPlayerLevelChangedInvoke(level);
        }
        private void OnForPlayerLevelUpInvoke(int level) => m_OnForPlayerLevelUp?.Invoke(level);
        private void OnPlayerLevelChangedInvoke(int level) => m_OnPlayerLevelChanged?.Invoke(level);
        private void OnForPlayerExperienceAddedInvoke(int amount)
        {
            int required = Mathf.Max(1, m_Config.ForPlayerLevelFormula.GetCurrentMaxExperience(m_ForPlayerCurrentLevel + 1));
            m_OnForPlayerExperienceAdded?.Invoke(amount, required);
        }
        private void SetForPlayerLevel(int level)
        {
            m_ForPlayerCurrentLevel = Mathf.Clamp(level, 1, Mathf.Max(1, m_Config.ForPlayerLevelFormula.MaxLevel));
            OnLevelChangedInvoke(m_ForPlayerCurrentLevel);
        }
        private void AddForPlayerLevel(int amount) => SetForPlayerLevel(m_ForPlayerCurrentLevel + amount);
    }
}
