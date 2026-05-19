// SpinWheel.cs
using System.Collections;
using MoreMountains.Tools;
using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class SpinWheel : MonoBehaviour
    {
        // ── Definition ────────────────────────────────────────────────────────────

        [SerializeField]
        private SpinWheelDefinition m_Definition;
        public SpinWheelDefinition Definition => m_Definition;

        // ── Spin currency ─────────────────────────────────────────────────────────

        [SerializeField]
        private Currency m_SpinDraw;
        public Currency SpinDraw => m_SpinDraw;

        // ── Runtime state ─────────────────────────────────────────────────────────

        [SerializeField, MMReadOnly]
        private int m_FreeDrawWatch;
        public int FreeDrawWatch => m_FreeDrawWatch;

        [SerializeField, MMReadOnly]
        private int m_CurrentStepIndex;

        [SerializeField, MMReadOnly]
        private int m_TargetRewardIndex = -1;

        [SerializeField, MMReadOnly]
        private SpinRewardDefinition m_SelectedReward;
        public SpinRewardDefinition SelectedReward => m_SelectedReward;

        [SerializeField, MMReadOnly]
        private bool m_IsBusy;
        public bool IsBusy => m_IsBusy;

        // ── Events ────────────────────────────────────────────────────────────────

        [SerializeField] private UnityEvent m_OnInitialized;
        [SerializeField] private UnityEvent m_OnSpinStart;

        /// <summary>
        /// Param 1: index slot yang aktif di m_Definition.Rewards[].
        /// Param 2: definisi reward di slot tersebut.
        /// SpinWheelPanel pakai index ini untuk highlight view yang tepat.
        /// </summary>
        [SerializeField] private UnityEvent<int, SpinRewardDefinition> m_OnStepChanged;

        [SerializeField] private UnityEvent m_OnSpinEnd;
        [SerializeField] private UnityEvent<SpinRewardDefinition> m_OnClaim;

        public UnityEvent<int, SpinRewardDefinition> OnStepChanged => m_OnStepChanged;
        public UnityEvent<SpinRewardDefinition> OnClaim => m_OnClaim;
        public UnityEvent OnSpinStart => m_OnSpinStart;
        public UnityEvent OnSpinEnd => m_OnSpinEnd;

        // ── Persistence ───────────────────────────────────────────────────────────

        private const string FreeDrawWatchKey = "spinwatchfree";

        // ── Result panel cache ────────────────────────────────────────────────────

        private ShopResultPanel m_ResultPanel;
        private ShopResultPanel ResultPanel
        {
            get
            {
                if (m_ResultPanel == null)
                    m_ResultPanel = CanvasManager.Instance.GetPanel<ShopResultPanel>();
                return m_ResultPanel;
            }
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────────

        private void Start()
        {
            UnityService.Instance.DailyCheckIn.OnFirstCheckInToday.AddListener(ResetSpinKeyEachDay);
            UnityService.Instance.DailyCheckIn.OnFirstCheckInToday.AddListener(ResetFreeDrawWatch);
        }

        public void Init()
        {

            ResolveSpinDrawFromPlayer();
            SubscribeToPlayerCurrency();

            LoadFreeDrawWatch();

            m_OnInitialized?.Invoke();
        }

        // ── Currency sync ─────────────────────────────────────────────────────────

        private void ResolveSpinDrawFromPlayer()
        {
            if (m_SpinDraw == null)
            {
                Debug.LogError("[SpinWheel] m_SpinDraw belum di-assign di Inspector.");
                return;
            }

            if (!Player.Instance.CurrencyControl.HasCurrency(m_SpinDraw.ItemConfig, out Currency playerCurrency))
            {
                Debug.LogError($"[SpinWheel] Currency '{m_SpinDraw.ItemConfig.BaseInfo.Id}' " +
                               $"tidak ditemukan di PlayerCurrencyControl.");
                return;
            }

            m_SpinDraw = playerCurrency;
        }

        private void SubscribeToPlayerCurrency()
        {
            Player.Instance.CurrencyControl.OnCurrencyChanged.RemoveListener(OnPlayerCurrencyChanged);
            Player.Instance.CurrencyControl.OnCurrencyChanged.AddListener(OnPlayerCurrencyChanged);
        }

        private void OnPlayerCurrencyChanged(Currency currency)
        {
            if (currency.ItemConfig.BaseInfo.Id != m_SpinDraw.ItemConfig.BaseInfo.Id) return;
            m_OnInitialized?.Invoke();
        }

        // ── Public API ────────────────────────────────────────────────────────────

        public bool TrySpin(UnityAction onConsumed = null)
        {
            if (!CanSpin(SpinBlockReason.Ticket)) return false;

            Player.Instance.CurrencyControl.RemoveCurrencyAmount(m_SpinDraw.ItemConfig, 1);
            onConsumed?.Invoke();
            StartCoroutine(RunSpin());
            return true;
        }

        public bool TryFreeWatchSpin(UnityAction onConsumed = null)
        {
            if (!CanSpin(SpinBlockReason.FreeWatch)) return false;

            UnityService.Instance.ShowRewardedAd(() =>
            {
                ConsumeFreeDrawWatch();
                onConsumed?.Invoke();
                StartCoroutine(RunSpin());
            });
            return true;
        }

        public bool CanClaim() => m_SelectedReward != null && !m_IsBusy;

        public void SetSpinDrawAmount(int amount)
        {
            Player.Instance.CurrencyControl.SetCurrencyAmount(m_SpinDraw.ItemConfig, amount);
        }

        // ── Spin logic ────────────────────────────────────────────────────────────

        private IEnumerator RunSpin()
        {
            m_IsBusy = true;
            m_SelectedReward = null;
            m_TargetRewardIndex = m_Definition.PickWeightedRewardIndex();

            m_OnSpinStart?.Invoke();

            int rewardCount = m_Definition.Rewards.Length;
            int extra = Random.Range(m_Definition.MinAdditionalSpinStep, m_Definition.MaxAdditionalSpinStep + 1);

            int rawTotal = m_Definition.MinSpinStep + extra;
            int stepsToTarget = (m_TargetRewardIndex - m_CurrentStepIndex + rewardCount) % rewardCount;
            if (stepsToTarget == 0) stepsToTarget = rewardCount;

            int fullLaps = Mathf.CeilToInt((float)(rawTotal - stepsToTarget) / rewardCount);
            int finalSteps = fullLaps * rewardCount + stepsToTarget;

            float baseDelay = m_Definition.StartStepDelay;

            for (int i = 0; i < finalSteps; i++)
            {
                m_CurrentStepIndex = (m_CurrentStepIndex + 1) % rewardCount;

                float progress = (float)i / finalSteps;
                float delay = baseDelay;
                if (progress > 0.85f) delay += m_Definition.MidDelayGrowth;
                if (progress > 0.93f) delay += m_Definition.EndDelayGrowth;

                // Broadcast index agar panel highlight view di posisi yang benar
                m_OnStepChanged?.Invoke(m_CurrentStepIndex, m_Definition.Rewards[m_CurrentStepIndex]);

                yield return new WaitForSeconds(delay);
            }

            m_SelectedReward = m_Definition.Rewards[m_TargetRewardIndex];
            m_IsBusy = false;
            m_OnSpinEnd?.Invoke();

            yield return new WaitForSeconds(m_Definition.ClaimDelay);

            ClaimRewardInternal();
        }

        private void ClaimRewardInternal()
        {
            if (m_SelectedReward == null) return;

            SpinRewardDefinition reward = m_SelectedReward;

            CollectibleResultData resultData = reward.BuildResultData();
            ResultPanel.Show(resultData);

            m_OnClaim?.Invoke(reward);
            m_SelectedReward = null;

            CollectibleControl.AddCollectibleStatic("spin", reward.Collectible, reward.Amount);
        }

        // ── Free draw watch ───────────────────────────────────────────────────────

        private void LoadFreeDrawWatch()
        {
            if (UnityService.Instance.HasData(FreeDrawWatchKey))
                m_FreeDrawWatch = UnityService.Instance.GetData<int>(FreeDrawWatchKey);
            else
                m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
        }

        private void ResetSpinKeyEachDay()
        {
            Player.Instance.CurrencyControl.AddCurrencyAmount(m_SpinDraw.ItemConfig, m_Definition.FreeSpinAmountEachDay);
        }
        private void ResetFreeDrawWatch()
        {
            m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
            SaveFreeDrawWatch();
        }

        private void ConsumeFreeDrawWatch()
        {
            m_FreeDrawWatch = Mathf.Clamp(m_FreeDrawWatch - 1, 0, m_Definition.FreeDrawWatchAmount);
            SaveFreeDrawWatch();
        }

        private void SaveFreeDrawWatch()
        {
            UnityService.Instance.SaveData(FreeDrawWatchKey, m_FreeDrawWatch);
        }

        // ── Guard helpers ─────────────────────────────────────────────────────────

        private enum SpinBlockReason { Ticket, FreeWatch }

        private bool CanSpin(SpinBlockReason reason)
        {
            if (m_IsBusy)
            {
                Debug.LogWarning("[SpinWheel] Spin ignored: already spinning.");
                return false;
            }
            if (m_SelectedReward != null)
            {
                Debug.LogWarning("[SpinWheel] Spin ignored: unclaimed reward pending.");
                return false;
            }
            if (reason == SpinBlockReason.Ticket && m_SpinDraw.Amount <= 0)
            {
                Debug.LogWarning("[SpinWheel] Spin ignored: no tickets.");
                return false;
            }
            if (reason == SpinBlockReason.FreeWatch && m_FreeDrawWatch <= 0)
            {
                Debug.LogWarning("[SpinWheel] Spin ignored: no free watch spins.");
                return false;
            }
            return true;
        }

        private bool ValidateDefinition()
        {
            if (m_Definition == null)
            {
                Debug.LogError("[SpinWheel] SpinWheelDefinition is not assigned.");
                return false;
            }
            if (m_Definition.Rewards == null || m_Definition.Rewards.Length == 0)
            {
                Debug.LogError("[SpinWheel] SpinWheelDefinition has no rewards.");
                return false;
            }
            return true;
        }

#if UNITY_EDITOR
        [ContextMenu("Debug / Force Spin")]
        private void DebugForceSpin() => StartCoroutine(RunSpin());

        [ContextMenu("Debug / Force Claim")]
        private void DebugForceClaim() => ClaimRewardInternal();

        [ContextMenu("Debug / Reset Free Watch")]
        private void DebugResetFreeWatch()
        {
            m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
            SaveFreeDrawWatch();
        }
#endif
    }
}