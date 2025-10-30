using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField]
        private UnityEvent<Energy[]> m_OnTryPay;
        [SerializeField]
        private UnityEvent<Energy[]> m_OnCanPay;
        [SerializeField]
        private UnityEvent<Energy[]> m_OnCantPay;

        public UnityEvent<Energy[]> OnTryPay => m_OnTryPay;
        public UnityEvent<Energy[]> OnCanPay => m_OnCanPay;
        public UnityEvent<Energy[]> OnCantPay => m_OnCantPay;

        private Energy[] m_PreviousCost;
        public Energy[] PreviousCost => m_PreviousCost;

        public void ClearPreviousCost()
        {
            m_PreviousCost = null;
        }
        public void TryPayPreviousCost()
        {
            TryPayInternal(m_PreviousCost);
        }
        public void PayPreviouesCost(UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            PayInternal(m_PreviousCost, onCanPayListen, onCantPayListen);
        }
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
        public Energy GetEnergy(EnergyDefinition definition)
        {
            return GetEnergyInternal(definition);
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
        public void Init()
        {
            foreach(var energy in m_Energies)
            {
                energy.Initialize();
            }
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
        private void Update()
        {
            Regen();
        }

        private void Regen()
        {
            foreach(var energy in m_Energies)
            {
                energy.Regening();
            }
        }
        private void TryPayInternal(Energy[] energiyCosts)
        {
            m_OnTryPay?.Invoke(energiyCosts);
        }
        public void TryPay(Energy[] energiyCosts)
        {
            TryPayInternal(energiyCosts);
        }
        private void PayInternal(Energy[] energyCosts, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            int amountcanPay = 0;
            List<Energy> energyNeeds = new List<Energy>();
            foreach (var cost in energyCosts)
            {
                Energy ownEnergy = GetEnergyInternal(cost.Definition);
                bool canPay = ownEnergy.CanPay(cost.Amount);
                if (canPay)
                {
                    amountcanPay++;
                }
                else
                {
                    int restAmount = cost.Amount - ownEnergy.Amount;
                    Energy restEnergy = new Energy(cost.Definition, restAmount);
                    energyNeeds.Add(restEnergy);
                }
                Debug.Log($"ammount canpay = {amountcanPay}/ energyCosts Lenght {energyCosts.Length}");
            }
            if (amountcanPay >= energyCosts.Length)
            {
                foreach (var cost in energyCosts)
                {
                    Energy ownEnergy = GetEnergyInternal(cost.Definition);
                    ownEnergy.Pay(cost.Amount);
                }
                m_OnCanPay.Invoke(energyCosts);
                onCanPayListen.Invoke(energyCosts);
                m_PreviousCost = energyCosts;
            }
            else
            {
                onCantPayListen.Invoke(energyNeeds.ToArray());
                m_OnCantPay.Invoke(energyNeeds.ToArray());
            }
        }
        public void Pay(Energy[] energyCosts, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            PayInternal(energyCosts, onCanPayListen, onCantPayListen);
        }
    }

    [System.Serializable]
    public class Energy
    {
        [SerializeField]
        private EnergyDefinition m_Definition;
        [SerializeField]
        private TimerDefinition m_Timer;
        [SerializeField]
        private int m_Amount;

        [SerializeField]
        private UnityEvent<Energy> m_OnAmountChanged;
        [SerializeField]
        private UnityEvent<int> m_OnAmountSpend;
        [SerializeField]
        private UnityEvent<Energy> m_OnInitialized;

        public EnergyDefinition Definition => m_Definition;
        public int Amount => m_Amount;

        private float m_CurrentTimeSpend;

        public Energy(EnergyDefinition definition, int amount)
        {
            m_Definition = definition;
            m_Amount = amount;
        }

        public bool IsFull => m_Amount >= m_Definition.MaxAmount;

        public void Initialize()
        {
            if (UnityService.Instance.HasData(m_Definition.Id))
            {
                SetInternal(UnityService.Instance.GetData<int>(m_Definition.Id));
            }
            else
            {
                ResetEnergy();
            }
            m_Timer.CheckTimer(ResetEnergy, () => AddInternal(0));
            ClampAmount();
        }
        public void Add(int amount)
        {
            AddInternal(amount);
        }
        public void Set(int amount)
        {
            SetInternal(amount);
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
        public void Regening()
        {
            if (!m_Definition.CanRegen) return;
            int interval = m_Definition.RegenEverEverySeconds;
            bool offsiteMax = m_Amount > m_Definition.MaxAmount;
            //bool canRegen = !offsiteMax && underTimeSpend;
            if (!offsiteMax)
            {
                m_CurrentTimeSpend += Time.deltaTime;
                if (m_CurrentTimeSpend > interval)
                {
                    m_Timer.CheckTimer(ResetEnergy, () => AddInternal(m_Definition.RegenAmount));
                    m_CurrentTimeSpend = 0f;
                }
            }
        }
        private void AddInternal(int add)
        {
            m_Amount += add;
            ClampAmount();
            m_OnAmountChanged?.Invoke(this);
            UnityService.Instance.SaveData(m_Definition.Id, m_Amount);
        }
        private void SetInternal(int set)
        {
            m_Amount = set;
            ClampAmount();
            m_OnAmountChanged?.Invoke(this);
            UnityService.Instance.SaveData(m_Definition.Id, m_Amount);
        }
        private void ResetEnergy()
        {
            bool offsiteMax = m_Amount >= m_Definition.MaxAmount;
            if (!offsiteMax)
            {
                SetInternal(m_Definition.MaxAmount);
            }
            m_Timer.StartTimer();
        }

        private bool CanPayInternal(int cost)
        {
            return m_Amount >= cost;
        }
        public bool CanPay(int cost)
        {
            return CanPayInternal(cost);
        }

        public void Pay(int cost)
        {
            if (CanPayInternal(cost))
            {
                AddInternal(-cost);
                m_OnAmountSpend?.Invoke(cost);
            }
        }
    }
}
