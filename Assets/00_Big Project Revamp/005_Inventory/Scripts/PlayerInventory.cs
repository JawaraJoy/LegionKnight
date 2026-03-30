using UnityEngine;

namespace Rush
{
    /// <summary>
    /// Top-level facade yang owns semua inventory types.
    /// Extend Singleton&lt;T&gt; sehingga lifetime dikelola oleh Unity
    /// dan persist antar scene via DontDestroy (sudah dihandle Singleton base).
    ///
    /// Usage
    /// ─────
    ///   // Initialise sekali di Awake (misal dari GameManager)
    ///   PlayerInventory.Instance.Initialise(allHeroes, allCards);
    ///
    ///   // Hero
    ///   PlayerInventory.Instance.Heroes.Unlock(someHeroConfig);
    ///   PlayerInventory.Instance.Heroes.TrySelectHero(someHeroConfig);
    ///
    ///   // Card
    ///   PlayerInventory.Instance.Cards.Unlock(someCardConfig);
    ///   PlayerInventory.Instance.Cards.TrySelectCard(someCardConfig);
    ///
    ///   // Item
    ///   PlayerInventory.Instance.Items.Add(someItemConfig, 5);
    ///   PlayerInventory.Instance.Items.Remove(someItemConfig, 1);
    /// </summary>
    public class PlayerInventory : Singleton<PlayerInventory>
    {
        // ── Config (assign via Inspector) ─────────────────────────────
        [SerializeField] private HeroUnitConfig[] m_AllHeroes;
        [SerializeField] private CardConfig[]     m_AllCards;

        [SerializeField, Min(0)] private int m_MaxHeroSelection    = 0;
        [SerializeField, Min(0)] private int m_MaxCardSelection     = 0;
        [SerializeField, Min(1)] private int m_DefaultItemMaxStack  = 99;

        // ── Sub-inventories ───────────────────────────────────────────
        public HeroInventory Heroes { get; private set; }
        public CardInventory Cards  { get; private set; }
        public ItemInventory Items  { get; private set; }

        // ── Unity ─────────────────────────────────────────────────────
        protected override void Awake()
        {
            base.Awake();
            InitialiseInternal(m_AllHeroes, m_AllCards,
                               m_MaxHeroSelection, m_MaxCardSelection,
                               m_DefaultItemMaxStack);
        }

        // ── Initialise ────────────────────────────────────────────────

        /// <summary>
        /// Panggil ini jika ingin override config Inspector secara runtime.
        /// Biasanya tidak perlu — Awake sudah memanggil InitialiseInternal otomatis.
        /// </summary>
        public void Initialise(
            HeroUnitConfig[] allHeroes,
            CardConfig[]     allCards,
            int              maxHeroSelection    = 0,
            int              maxCardSelection    = 0,
            int              defaultItemMaxStack = 99)
        {
            InitialiseInternal(allHeroes, allCards,
                               maxHeroSelection, maxCardSelection,
                               defaultItemMaxStack);
        }

        protected virtual void InitialiseInternal(
            HeroUnitConfig[] allHeroes,
            CardConfig[]     allCards,
            int              maxHeroSelection,
            int              maxCardSelection,
            int              defaultItemMaxStack)
        {
            Heroes = new HeroInventory(maxHeroSelection);
            Cards  = new CardInventory(maxCardSelection);
            Items  = new ItemInventory(defaultItemMaxStack);

            if (allHeroes != null) Heroes.RegisterAll(allHeroes);
            if (allCards  != null) Cards.RegisterAll(allCards);

            Debug.Log($"[PlayerInventory] Initialised — " +
                      $"{Heroes.Count} heroes | {Cards.Count} cards | ready for items.");
        }
    }
}