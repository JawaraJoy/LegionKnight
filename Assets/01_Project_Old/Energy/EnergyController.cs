using MoreMountains.Tools;
using Rush;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        // ── Config ────────────────────────────────────────────────────────────
        [SerializeField]
        private EnergyConfig m_Config;
        [SerializeField]
        private int m_Amount;

        // ── Events ────────────────────────────────────────────────────────────
        [SerializeField]
        private UnityEvent<Energy> m_OnAmountChanged;
        [SerializeField]
        private UnityEvent<int> m_OnAmountSpend;

        // ── Runtime ───────────────────────────────────────────────────────────
        private float m_CurrentRegenTimeSpend;

        // ── Save keys ─────────────────────────────────────────────────────────
        // Pisahkan key amount dan key reset time agar tidak bentrok
        private string AmountKey => m_Config.BaseInfo.Id + "amount";
        private string ResetTimeKey => m_Config.BaseInfo.Id + "resetTime";

        private static readonly string DateFormat = "yyyy-MM-dd HH:mm:ss";

        // ── Public props ──────────────────────────────────────────────────────
        public EnergyConfig Config => m_Config;
        public int Amount => m_Amount;
        public bool IsFull => m_Amount >= m_Config.MaxAmount;

        public Energy(EnergyConfig config, int amount)
        {
            m_Config = config;
            m_Amount = amount;
        }

        // ─────────────────────────────────────────────────────────────────────
        // INITIALIZE
        // ─────────────────────────────────────────────────────────────────────
        /// <summary>
        /// Urutan:
        /// 1. Cek apakah sudah waktunya daily reset
        ///    - Ya dan energy tidak exceed max → reset ke max, simpan next reset time
        /// 2. Load saved amount
        /// 3. Kalau belum pernah ada data → set ke max dan simpan next reset time
        /// </summary>
        public void Initialize()
        {
            bool hasAmount = UnityService.Instance.HasData(AmountKey);
            bool hasResetTime = UnityService.Instance.HasData(ResetTimeKey);

            Debug.Log($"[Energy] key='{m_Config.BaseInfo.Id}' AmountKey='{AmountKey}' ResetTimeKey='{ResetTimeKey}'");
            Debug.Log($"[Energy] hasAmount={hasAmount} hasResetTime={hasResetTime}");
            if (hasAmount)
                Debug.Log($"[Energy] saved amount={UnityService.Instance.GetData<int>(AmountKey)}");
            if (hasResetTime)
                Debug.Log($"[Energy] saved resetTime='{UnityService.Instance.GetData<string>(ResetTimeKey)}'");

            if (!hasAmount || !hasResetTime)
            {
                // Fresh install atau data hilang → set max dan simpan reset time berikutnya
                Debug.Log($"[Energy] '{m_Config.BaseInfo.Id}' no data found, initializing fresh.");
                SetInternal(m_Config.MaxAmount);
                SaveNextResetTime();
                return;
            }

            // Ada data → cek apakah sudah waktunya reset harian
            if (IsDailyResetTime())
            {
                int savedAmount = UnityService.Instance.GetData<int>(AmountKey);
                bool isExceedMax = savedAmount > m_Config.MaxAmount;

                if (isExceedMax)
                {
                    // Exceed max → jangan reset, tapi tetap load dan update reset time
                    Debug.Log($"[Energy] '{m_Config.BaseInfo.Id}' daily reset skipped (exceed max={savedAmount}).");
                    SetInternal(savedAmount);
                }
                else
                {
                    // Reset ke max
                    Debug.Log($"[Energy] '{m_Config.BaseInfo.Id}' daily reset triggered → set to max.");
                    SetInternal(m_Config.MaxAmount);
                }

                // Simpan next reset time untuk besok
                SaveNextResetTime();
                return;
            }

            // Belum waktunya reset → load saved amount
            int loaded = UnityService.Instance.GetData<int>(AmountKey);
            Debug.Log($"[Energy] '{m_Config.BaseInfo.Id}' loaded amount={loaded}");
            SetInternal(loaded);
        }

        // ─────────────────────────────────────────────────────────────────────
        // DAILY RESET LOGIC
        // ─────────────────────────────────────────────────────────────────────
        private bool IsDailyResetTime()
        {
            string savedStr = UnityService.Instance.GetData<string>(ResetTimeKey);

            if (string.IsNullOrEmpty(savedStr))
                return true; // Tidak ada data reset time → anggap perlu reset

            if (!DateTime.TryParseExact(savedStr, DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime nextReset))
            {
                return true; // Data korup → anggap perlu reset
            }

            bool isTime = DateTime.Now >= nextReset;
            
            return isTime;
        }

        /// <summary>
        /// Simpan next reset time = hari ini jam ResetClockHour.
        /// Kalau sekarang sudah lewat jam itu, maka besok jam itu.
        /// </summary>
        private void SaveNextResetTime()
        {
            int resetHour = m_Config.DailyResetHour; // misal 15 = jam 15:00
            DateTime now = DateTime.Now;
            DateTime nextReset = new DateTime(now.Year, now.Month, now.Day, resetHour, 0, 0);

            if (now >= nextReset)
                nextReset = nextReset.AddDays(1);

            string resetStr = nextReset.ToString(DateFormat);
            UnityService.Instance.SaveData(ResetTimeKey, resetStr);
            Debug.Log($"[Energy] '{m_Config.BaseInfo.Id}' next reset saved: {resetStr}");
        }

        // ─────────────────────────────────────────────────────────────────────
        // REGEN (dipanggil tiap frame via EnergyController.Tick)
        // ─────────────────────────────────────────────────────────────────────
        public void Regening()
        {
            if (!m_Config.CanRegen) return;

            bool isExceedMax = m_Amount > m_Config.MaxAmount;
            if (isExceedMax) return; // Exceed max → skip regen

            // Cek daily reset saat regen (game dibiarkan hidup melewati jam reset)
            if (IsDailyResetTime())
            {
                Debug.Log($"[Energy] '{m_Config.BaseInfo.Id}' daily reset triggered during regen.");
                SetInternal(m_Config.MaxAmount);
                SaveNextResetTime();
                return;
            }

            if (IsFull) return; // Sudah penuh → skip regen

            m_CurrentRegenTimeSpend += Time.deltaTime;
            if (m_CurrentRegenTimeSpend >= m_Config.RegenEverEverySeconds)
            {
                m_CurrentRegenTimeSpend = 0f;
                AddInternal(m_Config.RegenAmount);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // PUBLIC API
        // ─────────────────────────────────────────────────────────────────────
        public void Add(int amount) => AddInternal(amount);
        public void Set(int amount) => SetInternal(amount);

        public bool CanPay(int cost) => m_Amount >= cost;

        public void Pay(int cost)
        {
            if (CanPay(cost))
            {
                AddInternal(-cost);
                m_OnAmountSpend?.Invoke(cost);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // INTERNAL
        // ─────────────────────────────────────────────────────────────────────
        private void AddInternal(int add)
        {
            m_Amount += add;
            ClampAmount();
            m_OnAmountChanged?.Invoke(this);
            UnityService.Instance.SaveData(AmountKey, m_Amount);
            Debug.Log($"[Energy] AddInternal key='{AmountKey}' newAmount={m_Amount}");
        }

        private void SetInternal(int set)
        {
            m_Amount = set;
            ClampAmount();
            m_OnAmountChanged?.Invoke(this);
            UnityService.Instance.SaveData(AmountKey, m_Amount);
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
    }
}
