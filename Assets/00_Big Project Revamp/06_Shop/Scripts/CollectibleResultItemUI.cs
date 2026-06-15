using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class CollectibleResultItemUI : GachaCollectableItemUI
    {
        [Header("Flip Config")]
        [SerializeField] private int m_FlipCount = 6;
        [SerializeField] private float m_FlipInterval = 2f;

        [Header("Events")]
        // true = showing hero, false = showing converter
        [SerializeField] private UnityEvent<bool> m_OnFlip;
        

        private Coroutine m_FlipCoroutine;

        // Cached data for flip
        private Sprite m_HeroIcon;
        private string m_HeroName;
        private int m_HeroAmount;
        private Sprite m_ConverterIcon;
        private string m_ConverterName;
        private int m_ConverterAmount;
        private void OnDisable() => StopFlipInternal();

        public void Setup(CollectibleResultEntry entry)
        {
            StopFlipInternal();
            SetupBase(entry.Collectible, entry.Amount);

            if (GachaDuplicateHelper.IsDuplicateHero(entry.Collectible, out var heroConfig))
            {
                var converter = heroConfig.ItemDuplicateConverter;

                m_HeroIcon = entry.Collectible.CollectibleField?.Icon;
                m_HeroName = entry.Collectible.BaseInfo.Name;
                m_HeroAmount = entry.Amount;
                m_ConverterIcon = converter.ItemConfig.CollectibleField?.Icon;
                m_ConverterName = converter.ItemConfig.BaseInfo.Name ?? "-";
                m_ConverterAmount = converter.Amount;

                m_FlipCoroutine = StartCoroutine(FlipRoutine());
            }
            CollectibleControl.AddCollectibleStatic(gameObject.name, entry.Collectible, entry.Amount);
            OnSetupComplete(entry.Collectible, entry.Amount);
        }

        // ── Flip ──────────────────────────────────────────────────────────────

        private IEnumerator FlipRoutine()
        {
            for (int i = 0; i < m_FlipCount; i++)
            {
                bool showHero = i % 2 == 0;
                ApplyFlipInternal(showHero);
                m_OnFlip?.Invoke(showHero);
                yield return new WaitForSeconds(m_FlipInterval);
            }

            // End on converter
            ApplyFlipInternal(false);
            m_OnFlip?.Invoke(false);
            m_FlipCoroutine = null;
        }

        // Swap data on the same fields from base class
        private void ApplyFlipInternal(bool showHero)
        {
            if (showHero)
                SetupBase(m_HeroIcon, m_HeroName, m_HeroAmount);
            else
                SetupBase(m_ConverterIcon, m_ConverterName, m_ConverterAmount);
        }

        private void StopFlipInternal()
        {
            if (m_FlipCoroutine == null) return;
            StopCoroutine(m_FlipCoroutine);
            m_FlipCoroutine = null;
        }
    }
}