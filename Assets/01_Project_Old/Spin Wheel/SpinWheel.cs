// SpinWheel.cs
using System.Collections;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    /// <summary>
    /// Core spin wheel logic. Handles spin state, step animation, free-watch tracking,
    /// and reward claiming. Does NOT depend on UI — drives it through UnityEvents.
    /// </summary>
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

        // ── Runtime state (read-only in Inspector) ────────────────────────────────

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
        [SerializeField] private UnityEvent<SpinRewardDefinition> m_OnStepChanged;
        [SerializeField] private UnityEvent m_OnSpinEnd;
        [SerializeField] private UnityEvent<SpinRewardDefinition> m_OnClaim;

        // ── Persistence ───────────────────────────────────────────────────────────

        private const string FreeDrawWatchKey = "spinwatchfree";

        // ── Lifecycle ─────────────────────────────────────────────────────────────

        private void Start()
        {
            ValidateDefinition();
        }

        /// <summary>
        /// Call this after the Player and currency system are ready.
        /// </summary>
        public void Init()
        {
            if (!ValidateDefinition()) return;

            LoadFreeDrawWatch();
            HandleDailyReset();

            m_OnInitialized?.Invoke();
        }

        // ── Public API ────────────────────────────────────────────────────────────

        /// <summary>Spend one spin ticket and spin.</summary>
        public bool TrySpin(UnityAction onConsumed = null)
        {
            if (!CanSpin(SpinBlockReason.Ticket)) return false;

            Player.Instance.CurrencyControl.RemoveCurrencyAmount(m_SpinDraw.ItemConfig, 1);
            onConsumed?.Invoke();
            StartCoroutine(RunSpin());
            return true;
        }

        /// <summary>Spend one free-watch charge and spin (after showing an ad).</summary>
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

        /// <summary>Claim the pending reward. Safe to call only when CanClaim() is true.</summary>
        public bool TryClaim()
        {
            if (!CanClaim()) return false;

            ClaimRewardInternal();
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
            int minStep = m_Definition.MinSpinStep;
            int extra = Random.Range(m_Definition.MinAdditionalSpinStep, m_Definition.MaxAdditionalSpinStep + 1);

            // Align total steps so the cursor lands exactly on the target index.
            int rawTotal = minStep + extra;
            int stepsToTarget = (m_TargetRewardIndex - m_CurrentStepIndex + rewardCount) % rewardCount;
            if (stepsToTarget == 0) stepsToTarget = rewardCount;

            // Round up to nearest multiple that lands on target.
            int fullLaps = Mathf.CeilToInt((float)(rawTotal - stepsToTarget) / rewardCount);
            int finalSteps = fullLaps * rewardCount + stepsToTarget;

            float baseDelay = m_Definition.StartStepDelay;

            for (int i = 0; i < finalSteps; i++)
            {
                m_CurrentStepIndex = (m_CurrentStepIndex + 1) % rewardCount;

                float progress = (float)i / finalSteps;
                float delay = baseDelay;

                if (progress > 0.85f)
                    delay += m_Definition.MidDelayGrowth;
                if (progress > 0.93f)
                    delay += m_Definition.EndDelayGrowth;

                m_OnStepChanged?.Invoke(m_Definition.Rewards[m_CurrentStepIndex]);

                yield return new WaitForSeconds(delay);
            }

            // Cursor is now exactly on the target.
            m_SelectedReward = m_Definition.Rewards[m_TargetRewardIndex];
            m_IsBusy = false;
            m_OnSpinEnd?.Invoke();

            yield return new WaitForSeconds(m_Definition.ClaimDelay);

            // Auto-claim after delay (can be changed to manual if preferred).
            ClaimRewardInternal();
        }

        private void ClaimRewardInternal()
        {
            if (m_SelectedReward == null) return;

            SpinRewardDefinition reward = m_SelectedReward;
            reward.Rewards.DirectTakeLoots();

            m_OnClaim?.Invoke(reward);
            m_SelectedReward = null;
        }

        // ── Free draw watch ───────────────────────────────────────────────────────

        private void LoadFreeDrawWatch()
        {
            if (UnityService.Instance.HasData(FreeDrawWatchKey))
                m_FreeDrawWatch = UnityService.Instance.GetData<int>(FreeDrawWatchKey);
            else
                m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
        }

        private void HandleDailyReset()
        {
            TimerDefinition timer = m_Definition.FreeDrawResetTime;

            if (!UnityService.Instance.HasData(timer.TimerId))
            {
                timer.StartTimer();
                return;
            }

            if (!timer.IsTimeToReset()) return;

            Player.Instance.CurrencyControl.AddCurrencyAmount(
                m_SpinDraw.ItemConfig, m_Definition.FreeSpinAmountEachDay);

            m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
            SaveFreeDrawWatch();
            timer.StartTimer();
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

        // ── Editor helpers ────────────────────────────────────────────────────────

#if UNITY_EDITOR
        [ContextMenu("Debug / Force Spin")]
        private void DebugForceSpin() => StartCoroutine(RunSpin());

        [ContextMenu("Debug / Force Claim")]
        private void DebugForceClaim() => TryClaim();

        [ContextMenu("Debug / Reset Free Watch")]
        private void DebugResetFreeWatch()
        {
            m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
            SaveFreeDrawWatch();
        }
#endif
    }
}