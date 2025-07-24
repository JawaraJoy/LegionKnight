using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class EnergyController : MonoBehaviour
    {
        [SerializeField]
        private Energy[] m_Energies;
        [SerializeField]
        private UnityEvent<Energy> m_OnEnergyAmountChanged;

        private Energy GetEnergyInternal(EnergyDefinition definition)
        {
            foreach (var energy in m_Energies)
            {
                if (energy.Definition == definition)
                {
                    return energy;
                }
            }
            return null;
        }
        public bool HasEnergy(EnergyDefinition definition)
        {
            return GetEnergyInternal(definition) != null;
        }
        public bool IsFull(EnergyDefinition definition)
        {
            var energy = GetEnergyInternal(definition);
            if (energy == null)
            {
                Debug.LogError($"Energy '{definition.name}' not found in controller.");
                return false;
            }
            return energy.IsFull;
        }
        public void Add(EnergyDefinition definition, int amount)
        {
            var energy = GetEnergyInternal(definition);
            if (energy == null)
            {
                Debug.LogError($"Energy '{definition.name}' not found in controller.");
                return;
            }
            energy.Add(amount);
            m_OnEnergyAmountChanged?.Invoke(energy);
        }
        public void Set(EnergyDefinition definition, int amount)
        {
            var energy = GetEnergyInternal(definition);
            if (energy == null)
            {
                Debug.LogError($"Energy '{definition.name}' not found in controller.");
                return;
            }
            energy.Set(amount);
            m_OnEnergyAmountChanged?.Invoke(energy);
        }
    }

    [System.Serializable]
    public class Energy
    {
        [SerializeField]
        private EnergyDefinition m_Definition;
        private int m_Amount;

        [SerializeField]
        private UnityEvent<Energy> m_OnAmountChanged;
        [SerializeField]
        private UnityEvent<Energy> m_OnInitialized;

        public EnergyDefinition Definition => m_Definition;
        public int Amount => m_Amount;

        public Energy(EnergyDefinition definition, int amount)
        {
            m_Definition = definition;
            m_Amount = amount;
        }

        public bool IsFull => m_Amount >= m_Definition.MaxAmount;
        public void Initialize()
        {
            UnityService.Instance.LoadData(m_Definition.Id);
            m_Amount = UnityService.Instance.GetData<int>(m_Definition.Id);
            ClampAmount();
            m_OnAmountChanged?.Invoke(this);
        }
        public void Add(int amount)
        {
            m_Amount += amount;
            ClampAmount();
            m_OnAmountChanged?.Invoke(this);
            UnityService.Instance.SaveData(m_Definition.Id, m_Amount);
        }
        public void Set(int amount)
        {
            m_Amount = amount;
            ClampAmount();
            m_OnAmountChanged?.Invoke(this);
            UnityService.Instance.SaveData(m_Definition.Id, m_Amount);
        }
        private void ClampAmount()
        {
            if (m_Definition.CanBreakMaxAmount)
            {
                // If can break max amount, we don't clamp to max amount
                if (m_Amount < 0)
                {
                    m_Amount = 0;
                }
                return;
            }
            if (m_Amount < 0)
            {
                m_Amount = 0;
            }
            else if (m_Amount > m_Definition.MaxAmount)
            {
                m_Amount = m_Definition.MaxAmount;
            }
        }
    }
}
