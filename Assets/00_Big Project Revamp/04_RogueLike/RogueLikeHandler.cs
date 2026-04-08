
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

        [SerializeField]
        private UnityEvent<int, int> m_OnForPlayerExperienceAdded;
        [SerializeField]
        private UnityEvent<int> m_OnForPlayerLevelUp;

        [SerializeField]
        private UnityEvent<int, int> m_OnForBossExperienceAdded;

        [SerializeField]
        private UnityEvent<int> m_OnForBossLevelUp;
        [SerializeField]
        private UnityEvent<CardConfig> m_OnCardCollected;
        public RogueLikeConfig Config => m_Config;
        public int ForPlayerCurrentExperience => m_ForPlayerCurrentExperience;
        public int ForPlayerCurrentLevel => m_ForPlayerCurrentLevel;
        public UnityEvent<int, int> OnForPlayerExperienceAdded => m_OnForPlayerExperienceAdded;
        public UnityEvent<int> OnForPlayerLevelUp => m_OnForPlayerLevelUp;

        public UnityEvent<int, int> OnForBossExperienceAdded => m_OnForBossExperienceAdded;
        public UnityEvent<int> OnForBossLevelUp => m_OnForBossLevelUp;
        public UnityEvent<CardConfig> OnCardCollected => m_OnCardCollected;
        public List<CardConfig> CustomCards => m_CustomCards;

        public List<CardConfig> GetDifferenceCardRandom(int amount)
        {
            List<CardConfig> pool = new List<CardConfig>();

            // 1. Add Base Deck
            if (m_Config.BaseDeck != null)
            {
                pool.AddRange(m_Config.BaseDeck.CardConfigs);
            }

            // 2. Add Hero Deck
            if (m_CurrentHero != null && m_CurrentHero.HeroDeckConfig != null)
            {
                pool.AddRange(m_CurrentHero.HeroDeckConfig.CardConfigs);
            }

            // 3. Remove duplicates (important!)
            HashSet<CardConfig> uniquePool = new HashSet<CardConfig>(pool);

            // 4. Remove owned cards
            uniquePool.ExceptWith(m_CustomCards);

            List<CardConfig> finalPool = new List<CardConfig>(uniquePool);

            // 5. Random pick
            List<CardConfig> result = new List<CardConfig>();

            for (int i = 0; i < amount && finalPool.Count > 0; i++)
            {
                int index = Random.Range(0, finalPool.Count);
                result.Add(finalPool[index]);
                finalPool.RemoveAt(index);
            }

            return result;
        }
        public void SetCurrentHero(HeroUnitConfig heroConfig)
        {
            m_CurrentHero = heroConfig;
        }
        public void AddCard(CardConfig cardConfig)
        {
            AddCardInternal(cardConfig);
        }
        private void AddCardInternal(CardConfig cardConfig)
        {
            if (!m_CustomCards.Contains(cardConfig))
            {
                m_CustomCards.Add(cardConfig);
                m_OnCardCollected.Invoke(cardConfig);
            }
        }
        public void AddCards(List<CardConfig> cardConfigs)
        {
            foreach (var cardConfig in cardConfigs)
            {
                AddCardInternal(cardConfig);
            }
        }
        public void RemoveCards(List<CardConfig> cardConfigs)
        {
            foreach (var cardConfig in cardConfigs)
            {
                RemoveCardInternal(cardConfig);
            }
        }
        private void RemoveCardInternal(CardConfig cardConfig)
        {
            if (m_CustomCards.Contains(cardConfig))
            {
                m_CustomCards.Remove(cardConfig);
            }
        }
        private void ClearCustomCard()
        {
            m_CustomCards.Clear();
        }
        public void ResetProgression()
        {
            SetForPlayerLevel(1);
            SetForPlayerExperience(0);
            OnForPlayerExperienceAddedInvoke(m_ForPlayerCurrentExperience);
        }
        public void AddForPlayerExperience(int amount)
        {
            m_ForPlayerCurrentExperience += amount;
            CheckForPlayerLevelUp();
        }
        private void SetForPlayerExperience(int amount)
        {
            m_ForPlayerCurrentExperience = amount;
            CheckForPlayerLevelUp();
        }
        private void CheckForPlayerLevelUp()
        {
            int nextLevelExp = m_Config.ForPlayerLevelFormula.GetCurrentMaxExperience(m_ForPlayerCurrentLevel + 1);
            if (m_ForPlayerCurrentExperience >= nextLevelExp)
            {
                int excessExp = m_ForPlayerCurrentExperience - nextLevelExp;
                OnLevelUpInvoke();
                m_ForPlayerCurrentExperience = excessExp; // Carry over excess experience to the next level
            }
            OnForPlayerExperienceAddedInvoke(m_ForPlayerCurrentExperience);
        }
        private void OnLevelUpInvoke()
        {
            AddForPlayerLevel(1);
            // Implement level-up logic here (e.g., increase stats, unlock skills, etc.)
        }
        private void OnLevelChangedInvoke(int level)
        {
            if (m_ForPlayerCurrentLevel > 1)
            {
                m_OnForPlayerLevelUp.Invoke(level);
            }
        }
        private void OnForPlayerExperienceAddedInvoke(int amount)
        {
            m_OnForPlayerExperienceAdded.Invoke(amount, m_Config.ForPlayerLevelFormula.GetCurrentMaxExperience(m_ForPlayerCurrentLevel+1));
        }

        private void SetForPlayerLevel(int level)
        {
            m_ForPlayerCurrentLevel = level;
            OnLevelChangedInvoke(m_ForPlayerCurrentLevel);
            
        }
        private void AddForPlayerLevel(int amount)
        {
            m_ForPlayerCurrentLevel += amount;
            OnLevelChangedInvoke(m_ForPlayerCurrentLevel);
        }
    }
}
