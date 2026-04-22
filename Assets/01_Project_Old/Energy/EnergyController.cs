using MoreMountains.Tools;
using Rush;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class EnergyController : MonoBehaviour, IUpdater
    {
        [SerializeField] private Energy[] m_Energies;
        [SerializeField] private UnityEvent<Energy> m_OnEnergyAmountChanged;
        [SerializeField] private UnityEvent<Energy[]> m_OnTryPay;
        [SerializeField] private UnityEvent<Energy[]> m_OnCanPay;
        [SerializeField] private UnityEvent<Energy[]> m_OnCantPay;

        [SerializeField, MMReadOnly] private Energy[] m_PreviousCost;

        public UnityEvent<Energy[]> OnTryPay => m_OnTryPay;
        public UnityEvent<Energy[]> OnCanPay => m_OnCanPay;
        public UnityEvent<Energy[]> OnCantPay => m_OnCantPay;
        public Energy[] PreviousCost => m_PreviousCost;
        public bool IsActive => gameObject.activeInHierarchy;

        private void OnEnable()
        {
            if (UpdateBank.Instance != null)
            {
                UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
            }
        }

        private void OnDisable()
        {
            if (UpdateBank.Instance != null)
            {
                UpdateBank.Instance.UnregisterUpdateTick(gameObject);
            }
        }

        public void Init()
        {
            if (m_Energies == null) return;

            foreach (var energy in m_Energies)
            {
                if (energy == null) continue;
                energy.Initialize();
            }
        }

        public void ClearPreviousCost()
        {
            m_PreviousCost = null;
        }

        public void TryPayPreviousCost()
        {
            if (m_PreviousCost == null || m_PreviousCost.Length == 0)
            {
                Debug.LogWarning("[EnergyController] TryPayPreviousCost called but no previous cost exists.");
                return;
            }

            TryPayInternal(m_PreviousCost);
        }

        public void PayPreviouesCost(UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            if (m_PreviousCost == null || m_PreviousCost.Length == 0)
            {
                Debug.LogWarning("[EnergyController] PayPreviouesCost called but no previous cost exists.");
                onCantPayListen?.Invoke(Array.Empty<Energy>());
                return;
            }

            PayInternal(m_PreviousCost, onCanPayListen, onCantPayListen);
        }

        public Energy GetEnergy(EnergyConfig definition)
        {
            return GetEnergyInternal(definition);
        }

        public bool HasEnergy(EnergyConfig definition)
        {
            return GetEnergyInternal(definition) != null;
        }

        public void Add(EnergyConfig definition, int amount)
        {
            var energy = GetEnergyInternal(definition);
            if (energy == null)
            {
                Debug.LogError($"[EnergyController] Energy '{definition?.name}' not found.");
                return;
            }

            energy.Add(amount);
            m_OnEnergyAmountChanged?.Invoke(energy);

            if (energy.IsFull)
            {
                TenjinManager.Instance.SendEventToReEnergy();
            }
        }

        public void Set(EnergyConfig definition, int amount)
        {
            var energy = GetEnergyInternal(definition);
            if (energy == null)
            {
                Debug.LogError($"[EnergyController] Energy '{definition?.name}' not found.");
                return;
            }

            energy.Set(amount);
            m_OnEnergyAmountChanged?.Invoke(energy);

            if (energy.IsFull)
            {
                TenjinManager.Instance.SendEventToReEnergy();
            }
        }

        public void TryPay(Energy[] energyCosts)
        {
            TryPayInternal(energyCosts);
        }

        public void Pay(Energy[] energyCosts, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            PayInternal(energyCosts, onCanPayListen, onCantPayListen);
        }

        public void Tick()
        {
            Regen();
        }

        private Energy GetEnergyInternal(EnergyConfig definition)
        {
            if (definition == null || m_Energies == null) return null;

            foreach (var energy in m_Energies)
            {
                if (energy == null || energy.Config == null) continue;

                if (energy.Config == definition)
                {
                    return energy;
                }
            }

            return null;
        }

        private void Regen()
        {
            if (m_Energies == null) return;

            foreach (var energy in m_Energies)
            {
                if (energy == null) continue;
                energy.Regening();
            }
        }

        private void TryPayInternal(Energy[] energyCosts)
        {
            if (energyCosts == null || energyCosts.Length == 0)
            {
                Debug.LogWarning("[EnergyController] TryPayInternal called with empty costs.");
                return;
            }

            m_OnTryPay?.Invoke(energyCosts);
        }

        private void PayInternal(Energy[] energyCosts, UnityAction<Energy[]> onCanPayListen, UnityAction<Energy[]> onCantPayListen)
        {
            if (energyCosts == null || energyCosts.Length == 0)
            {
                Debug.LogWarning("[EnergyController] PayInternal called with empty costs.");
                onCantPayListen?.Invoke(Array.Empty<Energy>());
                m_OnCantPay?.Invoke(Array.Empty<Energy>());
                return;
            }

            int amountCanPay = 0;
            List<Energy> energyNeeds = new List<Energy>();

            foreach (var cost in energyCosts)
            {
                if (cost == null || cost.Config == null)
                {
                    Debug.LogWarning("[EnergyController] Found null cost/config in energyCosts.");
                    continue;
                }

                Energy ownEnergy = GetEnergyInternal(cost.Config);
                if (ownEnergy == null)
                {
                    Debug.LogError($"[EnergyController] Player does not own energy config '{cost.Config.name}'.");
                    energyNeeds.Add(new Energy(cost.Config, cost.Amount));
                    continue;
                }

                bool canPay = ownEnergy.CanPay(cost.Amount);
                if (canPay)
                {
                    amountCanPay++;
                }
                else
                {
                    int restAmount = Mathf.Max(0, cost.Amount - ownEnergy.Amount);
                    energyNeeds.Add(new Energy(cost.Config, restAmount));
                }

                Debug.Log($"[EnergyController] canPay={amountCanPay}/{energyCosts.Length}");
            }

            if (amountCanPay >= energyCosts.Length)
            {
                foreach (var cost in energyCosts)
                {
                    if (cost == null || cost.Config == null) continue;

                    Energy ownEnergy = GetEnergyInternal(cost.Config);
                    ownEnergy?.Pay(cost.Amount);
                }

                m_PreviousCost = energyCosts;
                m_OnCanPay?.Invoke(energyCosts);
                onCanPayListen?.Invoke(energyCosts);
            }
            else
            {
                Energy[] needs = energyNeeds.ToArray();
                onCantPayListen?.Invoke(needs);
                m_OnCantPay?.Invoke(needs);
            }
        }
    }

    [Serializable]
    public class Energy
    {
        [SerializeField] private EnergyConfig m_Config;
        [SerializeField] private int m_Amount;

        [SerializeField] private UnityEvent<Energy> m_OnAmountChanged;
        [SerializeField] private UnityEvent<int> m_OnAmountSpend;

        private float m_CurrentRegenTimeSpend;

        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        private string EnergyId
        {
            get
            {
                if (m_Config == null || m_Config.BaseInfo == null || string.IsNullOrWhiteSpace(m_Config.BaseInfo.Id))
                {
                    Debug.LogError("[Energy] Invalid BaseInfo.Id. Energy save key cannot be built.");
                    return "INVALID_ENERGY_ID";
                }

                return m_Config.BaseInfo.Id.Trim();
            }
        }

        private string AmountKey => $"{EnergyId}_amount";
        private string ResetTimeKey => $"{EnergyId}_resetTime";

        public EnergyConfig Config => m_Config;
        public int Amount => m_Amount;
        public bool IsFull => m_Config != null && m_Amount >= m_Config.MaxAmount;

        public Energy(EnergyConfig config, int amount)
        {
            m_Config = config;
            m_Amount = amount;
        }

        public void Initialize()
        {
            if (m_Config == null)
            {
                Debug.LogError("[Energy] Initialize failed: config is null.");
                return;
            }

            bool hasAmount = UnityService.Instance.HasData(AmountKey);
            bool hasResetTime = UnityService.Instance.HasData(ResetTimeKey);

            Debug.Log($"[Energy] Init id='{EnergyId}' amountKey='{AmountKey}' resetTimeKey='{ResetTimeKey}'");
            Debug.Log($"[Energy] hasAmount={hasAmount}, hasResetTime={hasResetTime}");

            // 1. Load amount dulu kalau ada
            if (hasAmount)
            {
                int loadedAmount = UnityService.Instance.GetData<int>(AmountKey);
                Debug.Log($"[Energy] Loaded saved amount={loadedAmount}");
                SetInternalWithoutResetUpdate(loadedAmount);
            }
            else
            {
                Debug.Log($"[Energy] No saved amount found. Using max amount={m_Config.MaxAmount}");
                SetInternalWithoutResetUpdate(m_Config.MaxAmount);
                SaveAmount();
            }

            // 2. Kalau reset time belum ada, bikin baru TANPA merusak amount
            if (!hasResetTime)
            {
                Debug.Log("[Energy] No reset time found. Creating next reset time.");
                SaveNextResetTime();
                return;
            }

            // 3. Kalau reset time invalid / corrupt, regenerate reset time TANPA overwrite amount
            if (!TryGetSavedResetTime(out DateTime nextReset))
            {
                Debug.LogWarning("[Energy] Reset time invalid/corrupt. Regenerating reset time without touching saved amount.");
                SaveNextResetTime();
                return;
            }

            // 4. Kalau sudah masuk waktu reset, baru reset amount
            if (DateTime.Now >= nextReset)
            {
                int currentSavedAmount = m_Amount;
                bool isExceedMax = currentSavedAmount > m_Config.MaxAmount;

                if (isExceedMax)
                {
                    Debug.Log($"[Energy] Daily reset skipped because saved amount exceeds max ({currentSavedAmount}).");
                }
                else
                {
                    Debug.Log("[Energy] Daily reset triggered. Setting amount to max.");
                    SetInternalWithoutResetUpdate(m_Config.MaxAmount);
                    SaveAmount();
                }

                SaveNextResetTime();
            }
        }

        public void Regening()
        {
            if (m_Config == null) return;
            if (!m_Config.CanRegen) return;

            bool isExceedMax = m_Amount > m_Config.MaxAmount;
            if (isExceedMax) return;

            // Jika game hidup terus melewati jam reset
            if (ShouldTriggerDailyReset())
            {
                Debug.Log($"[Energy] Daily reset triggered during regen for '{EnergyId}'.");
                SetInternalWithoutResetUpdate(m_Config.MaxAmount);
                SaveAmount();
                SaveNextResetTime();
                return;
            }

            if (IsFull) return;

            m_CurrentRegenTimeSpend += Time.deltaTime;
            if (m_CurrentRegenTimeSpend >= m_Config.RegenEverEverySeconds)
            {
                m_CurrentRegenTimeSpend = 0f;
                AddInternal(m_Config.RegenAmount);
            }
        }

        public void Add(int amount)
        {
            AddInternal(amount);
        }

        public void Set(int amount)
        {
            SetInternal(amount);
        }

        public bool CanPay(int cost)
        {
            return m_Amount >= cost;
        }

        public void Pay(int cost)
        {
            if (!CanPay(cost)) return;

            AddInternal(-cost);
            m_OnAmountSpend?.Invoke(cost);
        }

        private void AddInternal(int add)
        {
            m_Amount += add;
            ClampAmount();

            SaveAmount();
            m_OnAmountChanged?.Invoke(this);

            Debug.Log($"[Energy] AddInternal key='{AmountKey}' newAmount={m_Amount}");
        }

        private void SetInternal(int set)
        {
            m_Amount = set;
            ClampAmount();

            SaveAmount();
            m_OnAmountChanged?.Invoke(this);

            Debug.Log($"[Energy] SetInternal key='{AmountKey}' newAmount={m_Amount}");
        }

        /// <summary>
        /// Set amount tanpa menyentuh reset time.
        /// Tetap save amount supaya state konsisten.
        /// </summary>
        private void SetInternalWithoutResetUpdate(int set)
        {
            m_Amount = set;
            ClampAmount();

            SaveAmount();
            m_OnAmountChanged?.Invoke(this);

            Debug.Log($"[Energy] SetInternalWithoutResetUpdate key='{AmountKey}' newAmount={m_Amount}");
        }

        private void SaveAmount()
        {
            UnityService.Instance.SaveData(AmountKey, m_Amount);
        }

        private bool TryGetSavedResetTime(out DateTime nextReset)
        {
            nextReset = default;

            string savedStr = UnityService.Instance.GetData<string>(ResetTimeKey);
            if (string.IsNullOrWhiteSpace(savedStr))
            {
                return false;
            }

            return DateTime.TryParseExact(
                savedStr,
                DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out nextReset
            );
        }

        private bool ShouldTriggerDailyReset()
        {
            if (!TryGetSavedResetTime(out DateTime nextReset))
            {
                // jangan auto reset amount kalau resetTime rusak
                // cukup bikin reset time baru agar save lama tidak hilang
                SaveNextResetTime();
                return false;
            }

            return DateTime.Now >= nextReset;
        }

        /// <summary>
        /// Simpan next reset time = hari ini jam DailyResetHour.
        /// Kalau sekarang sudah lewat jam itu, maka besok jam itu.
        /// </summary>
        private void SaveNextResetTime()
        {
            int resetHour = Mathf.Clamp(m_Config.DailyResetHour, 0, 23);

            DateTime now = DateTime.Now;
            DateTime nextReset = new DateTime(now.Year, now.Month, now.Day, resetHour, 0, 0);

            if (now >= nextReset)
            {
                nextReset = nextReset.AddDays(1);
            }

            string resetStr = nextReset.ToString(DateFormat);
            UnityService.Instance.SaveData(ResetTimeKey, resetStr);

            Debug.Log($"[Energy] '{EnergyId}' next reset saved: {resetStr}");
        }

        private void ClampAmount()
        {
            if (m_Config == null) return;

            if (m_Config.CanBreakMaxAmount)
            {
                if (m_Amount < 0)
                {
                    m_Amount = 0;
                }
                return;
            }

            m_Amount = Mathf.Clamp(m_Amount, 0, m_Config.MaxAmount);
        }
    }
}