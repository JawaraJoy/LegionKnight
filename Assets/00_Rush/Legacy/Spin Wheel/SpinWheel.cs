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

        private const float c_FullCircle = 360f;
        private const string c_SkipPreference = "spinwheel.skipAnimation";
        private bool? m_SkipOverride;
        public bool SkipAnimation => m_SkipOverride ?? m_Definition.SkipAnimationByDefault;
        public float ClockwiseAngle { get; private set; }
        public event System.Action<float> RotationChanged;

        public void SetSkipAnimation(bool value)
        {
            if (value == SkipAnimation) return;
            m_SkipOverride = value;
            UnityService.Instance.SaveData(c_SkipPreference, value);
        }

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
            m_SkipOverride = UnityService.Instance.HasData(c_SkipPreference)
                ? UnityService.Instance.GetData<bool>(c_SkipPreference) : (bool?)null;

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
            return BeginSpinInternal(SpinBlockReason.Ticket, onConsumed);
        }

        public bool TryFreeWatchSpin(UnityAction onConsumed = null)
        {
            if (!CanSpin(SpinBlockReason.FreeWatch)) return false;

            bool handled = false;
            UnityService.Instance.ShowRewardedAd(() =>
            {
                if (handled) return;
                handled = true;
                if (CanSpin(SpinBlockReason.FreeWatch))
                    BeginSpinInternal(SpinBlockReason.FreeWatch, onConsumed);
            });
            return true;
        }

        public bool CanClaim() => m_SelectedReward != null && !m_IsBusy;

        public void SetSpinDrawAmount(int amount)
        {
            Player.Instance.CurrencyControl.SetCurrencyAmount(m_SpinDraw.ItemConfig, amount);
        }

        // ── Spin logic ────────────────────────────────────────────────────────────

        public static event System.Action SpinConsumed;

        private bool BeginSpinInternal(SpinBlockReason source, UnityAction onConsumed)
        {
            if (!ValidateDefinition()) return false;
            var rewards = m_Definition.Rewards;
            int target = m_Definition.PickWeightedRewardIndex();
            if (target < 0 || target >= rewards.Length || rewards[target] == null) return false;

            m_IsBusy = true;
            m_TargetRewardIndex = target;
            bool skip = SkipAnimation;
            if (source == SpinBlockReason.Ticket)
                Player.Instance.CurrencyControl.RemoveCurrencyAmount(m_SpinDraw.ItemConfig, 1);
            else if (source == SpinBlockReason.FreeWatch)
                ConsumeFreeDrawWatch();
            if (source != SpinBlockReason.Debug) SpinConsumed?.Invoke();
            onConsumed?.Invoke();
            OnSpinStartInvoke();

            float pitch = c_FullCircle / rewards.Length;
            float landing = Mathf.Repeat(-target * pitch, c_FullCircle);
            if (skip)
            {
                LandInternal(landing, rewards);
                ClaimRewardInternal();
            }
            else
            {
                StartCoroutine(RotateInternal(landing, rewards));
            }
            return true;
        }

        private IEnumerator RotateInternal(float landing, SpinRewardDefinition[] rewards)
        {
            float start = ClockwiseAngle;
            float travel = Mathf.Max(1, m_Definition.FullTurns) * c_FullCircle
                         + Mathf.Repeat(landing - start, c_FullCircle);
            float duration = m_Definition.SpinDuration;
            double startedAt = Time.realtimeSinceStartupAsDouble;
            double elapsed = 0d;
            while (elapsed < duration)
            {
                float remaining = 1f - (float)(elapsed / duration);
                float eased = 1f - remaining * remaining * remaining;
                SetPoseInternal(start + travel * eased, rewards, false);
                yield return null;
                elapsed = Time.realtimeSinceStartupAsDouble - startedAt;
            }

            LandInternal(landing, rewards);
            if (m_Definition.ClaimDelay > 0f)
                yield return new WaitForSecondsRealtime(m_Definition.ClaimDelay);
            ClaimRewardInternal();
        }

        private void LandInternal(float landing, SpinRewardDefinition[] rewards)
        {
            SetPoseInternal(landing, rewards, true);
            m_SelectedReward = rewards[m_TargetRewardIndex];
            m_IsBusy = false;
            OnSpinEndInvoke();
        }

        private void SetPoseInternal(float angle, SpinRewardDefinition[] rewards, bool landed)
        {
            ClockwiseAngle = Mathf.Repeat(angle, c_FullCircle);
            RotationChanged?.Invoke(ClockwiseAngle);
            // Slots are authored clockwise from twelve. A clockwise turn brings the opposite index up.
            int index = landed ? m_TargetRewardIndex : Mathf.FloorToInt(
                Mathf.Repeat(-ClockwiseAngle, c_FullCircle) * rewards.Length / c_FullCircle + .5f) % rewards.Length;
            if (index == m_CurrentStepIndex && !landed) return;
            m_CurrentStepIndex = index;
            OnStepChangedInvoke(index, rewards[index]);
        }

        private void OnSpinStartInvoke() => m_OnSpinStart?.Invoke();
        private void OnStepChangedInvoke(int index, SpinRewardDefinition reward) => m_OnStepChanged?.Invoke(index, reward);
        private void OnSpinEndInvoke() => m_OnSpinEnd?.Invoke();
        private void OnClaimInvoke(SpinRewardDefinition reward) => m_OnClaim?.Invoke(reward);

        private void ClaimRewardInternal()
        {
            if (m_SelectedReward == null) return;

            SpinRewardDefinition reward = m_SelectedReward;

            CollectibleResultData resultData = reward.BuildResultData();
            UnityService.Instance.LocalSave.ExecuteTransaction(() => resultData.GrantRewards("spin"));
            m_SelectedReward = null;
            ResultPanel?.Show(resultData);
            OnClaimInvoke(reward);
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

        private enum SpinBlockReason { Ticket, FreeWatch, Debug }

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
        private void DebugForceSpin()
        {
            if (CanSpin(SpinBlockReason.Debug)) BeginSpinInternal(SpinBlockReason.Debug, null);
        }

        [ContextMenu("Debug / Reset Free Watch")]
        private void DebugResetFreeWatch()
        {
            m_FreeDrawWatch = m_Definition.FreeDrawWatchAmount;
            SaveFreeDrawWatch();
        }
#endif
    }
}