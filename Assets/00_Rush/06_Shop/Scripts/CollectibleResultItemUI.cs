using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class CollectibleResultItemUI : GachaCollectableItemUI
    {

        [SerializeField] private int m_FlipCount = 6;
        [SerializeField] private float m_FlipInterval = 2f;

        // true = showing hero, false = showing converter
        [SerializeField] private UnityEvent<bool> m_OnFlip;
        

        private Coroutine m_FlipCoroutine;

        protected virtual void OnDisable() => StopFlipInternal();

        public void Setup(CollectibleResultEntry entry) => BindEntryInternal(entry, true);
        internal void Setup(CollectibleResultEntry entry, bool animateConversion) =>
            BindEntryInternal(entry, animateConversion);

        private void BindEntryInternal(CollectibleResultEntry entry, bool animateConversion)
        {
            StopFlipInternal();
            BindOriginInternal(entry);
            var converter = entry.WasDuplicateHero && entry.Collectible is HeroUnitConfig hero
                ? hero.ItemDuplicateConverter : null;
            if (converter?.ItemConfig == null)
                BindFaceInternal(entry.Collectible, entry.Amount);
            else if (animateConversion)
                m_FlipCoroutine = StartCoroutine(FlipRoutine(entry, converter));
            else
                BindFaceInternal(converter.ItemConfig, converter.Amount);

            OnSetupComplete(entry.Collectible, entry.Amount);
        }

        protected virtual void BindOriginInternal(CollectibleResultEntry entry) { }

        protected virtual void BindFaceInternal(CollectibleConfig collectible, int amount) =>
            SetupBase(collectible, amount);

        // ── Flip ──────────────────────────────────────────────────────────────

        private IEnumerator FlipRoutine(CollectibleResultEntry entry, LegionKnight.Currency converter)
        {
            for (int i = 0; i < m_FlipCount; i++)
            {
                bool showHero = i % 2 == 0;
                BindFaceInternal(showHero ? entry.Collectible : converter.ItemConfig, showHero ? entry.Amount : converter.Amount);
                OnFlipInvoke(showHero);
                yield return new WaitForSecondsRealtime(m_FlipInterval);
            }

            // End on converter
            BindFaceInternal(converter.ItemConfig, converter.Amount);
            OnFlipInvoke(false);
            m_FlipCoroutine = null;
        }

        private void StopFlipInternal()
        {
            if (m_FlipCoroutine == null) return;
            StopCoroutine(m_FlipCoroutine);
            m_FlipCoroutine = null;
        }
        private void OnFlipInvoke(bool value0)
        {
            m_OnFlip?.Invoke(value0);
        }
    }
}
