using LegionKnight;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class HeroProgressFeedback : MonoBehaviour
    {
        [SerializeField] private MMF_Player m_Player;
        [SerializeField] private CanvasGroup m_Group;
        [SerializeField] private TMP_Text m_Message;
        [SerializeField] private Image m_Emblem;
        [SerializeField] private Sprite m_LevelSprite;
        [SerializeField] private Sprite m_StarSprite;
        [SerializeField] private Color m_LevelColor;
        [SerializeField] private Color m_EvolveColor;
        private HeroesCollection m_Collection;

        public void Bind(HeroesCollection collection)
        {
            if (m_Collection == collection) return;
            UnbindInternal();
            m_Collection = collection;
            if (m_Collection == null) return;
            m_Collection.OnCharacterLevelUp.AddListener(PlayLevelInternal);
            m_Collection.OnCharacterStarUp.AddListener(PlayEvolveInternal);
        }

        private void OnDisable()
        {
            UnbindInternal();
            m_Group.alpha = 0f;
        }

        private void UnbindInternal()
        {
            if (m_Collection == null) return;
            m_Collection.OnCharacterLevelUp.RemoveListener(PlayLevelInternal);
            m_Collection.OnCharacterStarUp.RemoveListener(PlayEvolveInternal);
            m_Collection = null;
        }

        private void PlayLevelInternal(HeroUnitConfig config) => PlayInternal(config, false);
        private void PlayEvolveInternal(HeroUnitConfig config) => PlayInternal(config, true);

        private void PlayInternal(HeroUnitConfig config, bool evolve)
        {
            if (config == null || m_Collection.SelectedHero != config || !m_Player.gameObject.activeInHierarchy) return;
            var hero = m_Collection.GetHeroUnit(config);
            if (hero == null) return;
            if (m_Player.IsPlaying) m_Player.StopFeedbacks();
            m_Message.text = evolve ? $"{config.BaseInfo.Name}  ·  Evolve {hero.Star}" : $"{config.BaseInfo.Name}  ·  Level {hero.Level}";
            m_Emblem.sprite = evolve ? m_StarSprite : m_LevelSprite;
            m_Emblem.color = evolve ? m_EvolveColor : m_LevelColor;
            m_Player.PlayFeedbacks();
        }
    }
}
