using System;
using LegionKnight;
using UnityEngine;

namespace Rush
{
    // Compatibility boundary: feature-owned events provide committed outcomes to the tutorial.
    public sealed class TutorialRuntimeBridge : MonoBehaviour
    {
        private Player m_Player;
        private GachaManager m_Gacha;
        private StageManager m_Stages;
        private HomePanel m_Home;
        private GameplayPanel m_Gameplay;
        private PanelView[] m_Panels = Array.Empty<PanelView>();
        private CardUnit m_SelectedCard;
        private readonly System.Collections.Generic.HashSet<string> m_Equipped = new();
        public event Action<TutorialAction, string> ActionPerformed;
        public bool CanPresent
        {
            get
            {
                if (m_Home == null || !m_Home.IsShown || (m_Gameplay != null && m_Gameplay.IsShown)) return false;
                foreach (var panel in m_Panels)
                    if (panel != null && panel != m_Home && panel.Content != null && panel.Content.activeInHierarchy && panel.IsShown) return false;
                return true;
            }
        }

        public void Initialize(Player player, GachaManager gacha, StageManager stages, HomePanel home, GameplayPanel gameplay, PanelView[] panels)
        {
            StopListeningInternal();
            m_Player = player;
            m_Gacha = gacha;
            m_Stages = stages;
            m_Home = home;
            m_Gameplay = gameplay;
            m_Panels = panels ?? Array.Empty<PanelView>();
            if (m_Player != null)
            {
                m_Equipped.Clear();
                foreach (var card in m_Player.PlayerCardDeck.GetUsedCards())
                    if (card?.CardConfig != null) m_Equipped.Add(card.CardConfig.BaseInfo.Id);
                m_Player.HeroesCollection.OnSelectedCharacter.AddListener(OnHeroSelected);
                m_Player.HeroesCollection.OnCharacterLevelUp.AddListener(OnHeroUpgraded);
                m_Player.HeroesCollection.OnCharacterUsed.AddListener(OnHeroUsed);
                m_Player.PlayerCardDeck.OnSelectedCard.AddListener(OnCardSelected);
                m_Player.PlayerCardDeck.OnUsedCardsChanged.AddListener(OnCardsChanged);
            }
            if (m_Gacha != null)
            {
                m_Gacha.OnDrawRequested.AddListener(OnDrawRequested);
                m_Gacha.OnDrawComplete.AddListener(OnDrawCompleted);
            }
            if (m_Stages != null) m_Stages.OnStageStart.AddListener(OnStageStarted);
        }

        public bool IsTargetEligible(TutorTarget target)
        {
            if (target == null || !target.IsAvailable) return false;
            if (target.HeroConfig != null && (m_Player == null || m_Player.HeroesCollection.GetHeroUnit(target.HeroConfig)?.Owned != true)) return false;
            if (target.CardConfig != null && (m_Player == null || !m_Player.PlayerCardDeck.IsCardOwned(target.CardConfig) ||
                !target.CardConfig.IsAvailableForHero(m_Player.HeroesCollection.UsedHero))) return false;
            return true;
        }
        private void OnHeroSelected(HeroUnitConfig hero)
        {
            if (hero != null && m_Player.HeroesCollection.GetHeroUnit(hero)?.Owned == true)
                ActionPerformed?.Invoke(TutorialAction.HeroSelected, hero.BaseInfo.Id);
        }
        private void OnHeroUpgraded(HeroUnitConfig hero)
        {
            if (hero != null && hero == m_Player.HeroesCollection.SelectedHero)
                ActionPerformed?.Invoke(TutorialAction.HeroUpgraded, hero.BaseInfo.Id);
        }
        private void OnHeroUsed(HeroUnitConfig hero) => ActionPerformed?.Invoke(TutorialAction.HeroUsed, hero.BaseInfo.Id);
        private void OnCardSelected(CardUnit card)
        {
            m_SelectedCard = card;
            if (card?.IsOwned == true) ActionPerformed?.Invoke(TutorialAction.CardSelected, card.CardConfig.BaseInfo.Id);
        }
        private void OnCardsChanged(System.Collections.Generic.List<CardUnit> cards)
        {
            if (cards == null) return;
            var current = new System.Collections.Generic.HashSet<string>();
            foreach (var card in cards)
                if (card?.IsAdded == true)
                {
                    string id = card.CardConfig.BaseInfo.Id;
                    current.Add(id);
                    if (!m_Equipped.Contains(id)) ActionPerformed?.Invoke(TutorialAction.CardEquipped, id);
                }
            m_Equipped.Clear();
            foreach (var id in current) m_Equipped.Add(id);
        }
        public bool IsStateSatisfied(TutorialAction action) => action == TutorialAction.CardEquipped && m_SelectedCard?.IsAdded == true;
        private void OnDrawRequested(GachaConfirmData offer)
        {
            if (offer != null) ActionPerformed?.Invoke(TutorialAction.DrawRequested, string.Empty);
        }
        private void OnDrawCompleted(CollectibleResultData result)
        {
            if (result != null && result.Entries.Count > 0) ActionPerformed?.Invoke(TutorialAction.DrawCommitted, string.Empty);
        }
        private void OnStageStarted(StageConfig stage)
        {
            if (stage != null) ActionPerformed?.Invoke(TutorialAction.StageStarted, stage.BaseInfo.Id);
        }
        private void OnDisable() => StopListeningInternal();
        private void StopListeningInternal()
        {
            if (m_Player != null)
            {
                m_Player.HeroesCollection.OnSelectedCharacter.RemoveListener(OnHeroSelected);
                m_Player.HeroesCollection.OnCharacterLevelUp.RemoveListener(OnHeroUpgraded);
                m_Player.HeroesCollection.OnCharacterUsed.RemoveListener(OnHeroUsed);
                m_Player.PlayerCardDeck.OnSelectedCard.RemoveListener(OnCardSelected);
                m_Player.PlayerCardDeck.OnUsedCardsChanged.RemoveListener(OnCardsChanged);
            }
            if (m_Gacha != null)
            {
                m_Gacha.OnDrawRequested.RemoveListener(OnDrawRequested);
                m_Gacha.OnDrawComplete.RemoveListener(OnDrawCompleted);
            }
            if (m_Stages != null) m_Stages.OnStageStart.RemoveListener(OnStageStarted);
            m_Player = null;
            m_Gacha = null;
            m_Stages = null;
        }
    }
}
