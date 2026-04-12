using MoreMountains.Tools;
using Rush;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class EnergyController : MonoBehaviour, IUpdater
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

        [SerializeField, MMReadOnly]
        private Energy[] m_PreviousCost;
        public Energy[] PreviousCost => m_PreviousCost;

        public bool IsActive => gameObject.activeInHierarchy;
        private void OnEnable()
        {
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }
        private void OnDisable()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
        }
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
        private Energy GetEnergyInternal(EnergyConfig definition)
        {
            foreach (var energy in m_Energies)
            {
                if (energy.Config == definition)
                {
                    return energy;
                }
            }
            return null;
        }
        public Energy GetEnergy(EnergyConfig definition)
        {
            return GetEnergyInternal(definition);
        }
        public bool HasEnergy(EnergyConfig definition)
        {
            return GetEnergyInternal(definition) != null;
        }
        private bool IsFull(EnergyConfig definition)
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
        public void Add(EnergyConfig definition, int amount)
        {
            var energy = GetEnergyInternal(definition);
            if (energy == null)
            {
                Debug.LogError($"Energy '{definition.name}' not found in controller.");
                return;
            }
            energy.Add(amount);
            m_OnEnergyAmountChanged?.Invoke(energy);

            if(IsFull(definition))
            {
                //--Tenjin Record
                TenjinManager.Instance.SendEventToReEnergy();
            }
        }
        public void Set(EnergyConfig definition, int amount)
        {
            var energy = GetEnergyInternal(definition);
            if (energy == null)
            {
                Debug.LogError($"Energy '{definition.name}' not found in controller.");
                return;
            }
            energy.Set(amount);
            m_OnEnergyAmountChanged?.Invoke(energy);

            if(IsFull(definition))
            {
                //--Tenjin Record
                TenjinManager.Instance.SendEventToReEnergy();
            }
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
                Energy ownEnergy = GetEnergyInternal(cost.Config);
                bool canPay = ownEnergy.CanPay(cost.Amount);
                if (canPay)
                {
                    amountcanPay++;
                }
                else
                {
                    int restAmount = cost.Amount - ownEnergy.Amount;
                    Energy restEnergy = new Energy(cost.Config, restAmount);
                    energyNeeds.Add(restEnergy);
                }
                Debug.Log($"ammount canpay = {amountcanPay}/ energyCosts Lenght {energyCosts.Length}");
            }
            if (amountcanPay >= energyCosts.Length)
            {
                foreach (var cost in energyCosts)
                {
                    Energy ownEnergy = GetEnergyInternal(cost.Config);
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

        public void Tick()
        {
            Regen();
        }
    }

    [System.Serializable]
    public class Energy
    {
        [SerializeField]
        private EnergyConfig m_Config;
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

        public EnergyConfig Config => m_Config;
        public int Amount => m_Amount;

        private float m_CurrentTimeSpend;

        public Energy(EnergyConfig config, int amount)
        {
            m_Config = config;
            m_Amount = amount;
        }

        public bool IsFull => m_Amount >= m_Config.MaxAmount;

        /// <summary>
        /// Urutan yang benar:
        /// 1. Cek timer dulu — kalau expired, reset ke max dan start timer baru (tidak perlu load)
        /// 2. Kalau timer belum expired, baru load saved amount
        /// 3. Kalau belum pernah ada data sama sekali, reset ke max dan start timer
        /// </summary>
        public void Initialize()
        {
            string key = m_Config.BaseInfo.Id;
            bool hasData = UnityService.Instance.HasData(key);

            Debug.Log($"[Energy] Initialize key='{key}' hasData={hasData} IsTimeToReset={m_Timer.IsTimeToReset()}");

            if (!hasData)
            {
                Debug.Log($"[Energy] No data found → ResetEnergy()");
                ResetEnergy();
            }
            else if (m_Timer.IsTimeToReset())
            {
                Debug.Log($"[Energy] Timer expired → ResetEnergy()");
                ResetEnergy();
            }
            else
            {
                int saved = UnityService.Instance.GetData<int>(key);
                Debug.Log($"[Energy] Loading saved amount={saved}");
                SetInternal(saved);
            }

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
            if (m_Config.CanBreakMaxAmount)
            {
                if (m_Amount < 0) m_Amount = 0;
                return;
            }
            if (m_Amount < 0)
                m_Amount = 0;
            else if (m_Amount > m_Config.MaxAmount)
                m_Amount = m_Config.MaxAmount;
        }

        public void Regening()
        {
            if (!m_Config.CanRegen) return;

            int interval = m_Config.RegenEverEverySeconds;
            bool offSiteMax = m_Amount > m_Config.MaxAmount;

            if (!offSiteMax)
            {
                m_CurrentTimeSpend += Time.deltaTime;
                if (m_CurrentTimeSpend > interval)
                {
                    // Cek timer sebelum regen — kalau expired, reset dulu
                    if (m_Timer.IsTimeToReset())
                    {
                        ResetEnergy();
                    }
                    else
                    {
                        AddInternal(m_Config.RegenAmount);
                    }
                    m_CurrentTimeSpend = 0f;
                }
            }
        }

        private void AddInternal(int add)
        {
            m_Amount += add;
            ClampAmount();
            m_OnAmountChanged?.Invoke(this);
            UnityService.Instance.SaveData(m_Config.BaseInfo.Id, m_Amount);
        }

        private void SetInternal(int set)
        {
            m_Amount = set;
            ClampAmount();
            m_OnAmountChanged?.Invoke(this);
            UnityService.Instance.SaveData(m_Config.BaseInfo.Id, m_Amount);
        }

        private void ResetEnergy()
        {
            bool offSiteMax = m_Amount >= m_Config.MaxAmount;
            if (!offSiteMax)
            {
                SetInternal(m_Config.MaxAmount);
            }
            // ✅ Selalu start timer baru setelah reset
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
