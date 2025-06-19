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
            if (defi != null)
            {
                m_AmountPerTick = defi.GetFinalRegenAmount(level); // Assuming AbilityDefinition has a RegenAmountPerTick property
                m_Duration = defi.GetFinalRegenDuration(level); // Assuming AbilityDefinition has a RegenDuration property
            }
            StartRegen(); // Start the regeneration process
        }
        public void Initialize()
        {
            if (m_AbilityDefinition == null) return; // Ensure the ability definition is set
            CharacterDefinition characterDefinition = Player.Instance.UsedCharacter; // Get the character definition from the player instance
            CharacterUnit unit = Player.Instance.GetCharacterUnit(characterDefinition);
            int level = unit.Level;
            m_AmountPerTick = m_AbilityDefinition.GetFinalRegenAmount(level); // Get regen amount from ability definition
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
