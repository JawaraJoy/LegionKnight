using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Regen : MonoBehaviour, IAbility, ISelfAbility
    {
        [SerializeField]
        private bool m_RegenOnStart = false; // If true, regeneration starts immediately on initialization
        [SerializeField]
        private AbilityDefinition m_AbilityDefinition; // Reference to the ability definition
        [SerializeField]
        private int m_AmountPerTick = 10; // Amount to regenerate
        [SerializeField]
        private float m_Duration = 5f; // Duration of the regeneration in seconds
        [SerializeField]
        private UnityEvent<int> m_OnRegenApply = new();
        [SerializeField]
        private UnityEvent m_OnRegenStart = new();
        [SerializeField]
        private UnityEvent m_OnRegenEnd = new();
        private void Start()
        {
            if (m_RegenOnStart)
            {
                StartRegen(); // Start regeneration if the flag is set
            }
        }
        public void Initialize(AbilityDefinition defi, int level)
        {
            // Initialize the ability with the provided definition
            // This can include setting up specific properties or configurations based on the definition
            
            if (defi != null) return;
             // Assuming AbilityDefinition has a RegenDuration property
            StartRegen(); // Start the regeneration process
        }
        public void Initialize()
        {
            if (m_AbilityDefinition == null) return; // Ensure the ability definition is set
            GameObject owner = m_AbilityDefinition.GetOwner(); // Get the owner of the ability from the definition
            if (owner == null)
            {
                Debug.LogError("Owner of the ability is null. Cannot initialize regeneration.");
                return;
            }
            int level = 1; // Default level, can be adjusted based on the character's stats
            int MaxHealth = 100; // Default max health, can be adjusted based on the character's stats
            if (owner.TryGetComponent(out Player player))
            {
                MaxHealth = player.MaxHealth; // Get the player's maximum health
                CharacterDefinition characterDefinition = player.UsedCharacter; // Get the character definition from the player instance
                level = player.GetCharacterUnit(characterDefinition).Level; // Get the character's level
            }
            if (owner.TryGetComponent(out BosEnemy boss))
            {
                MaxHealth = boss.GetBosMaxHealth(); // Get the boss's maximum health
                level = boss.GetBosLevel(); // Get the boss's level
            }
            int baseAmount = m_AbilityDefinition.GetFinalRegenAmount(level); // Assuming AbilityDefinition has a RegenAmount property
            float finalRate = m_AbilityDefinition.GetFinalRegenRate(level); // Assuming AbilityDefinition has a RegenRate property
            int finalAmount = Mathf.RoundToInt(baseAmount + MaxHealth * finalRate); // Calculate the final amount based on level and rate
            m_AmountPerTick = finalAmount; // Set the amount per tick from the ability definition
            m_Duration = m_AbilityDefinition.GetFinalRegenDuration(level); // Get regen duration from ability definition
            StartRegen(); // Start the regeneration process
        }
        private void StartRegen()
        {
            m_OnRegenStart?.Invoke();
            StartCoroutine(StartRegenCoroutine(m_AmountPerTick, m_Duration));
        }

        private IEnumerator StartRegenCoroutine(int amount, float duration)
        {
            float elapsedTime = 0f;
            float interval = 1f;
            float intervalTimer = 0f;

            while (elapsedTime < duration)
            {
                float delta = Time.deltaTime;
                elapsedTime += delta;
                intervalTimer += delta;

                if (intervalTimer >= interval)
                {
                    m_OnRegenApply?.Invoke(amount);
                    intervalTimer = 0f;
                }

                yield return null;
            }
            m_OnRegenEnd?.Invoke();
        }

    }
}
