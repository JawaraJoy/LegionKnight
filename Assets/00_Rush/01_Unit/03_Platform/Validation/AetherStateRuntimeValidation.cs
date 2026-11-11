#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush.Tests
{
    // Run through Unity MCP in an empty Play Mode scene. Assets are cloned; quest/profile writes are replaced by counters.
    public class AetherStateRuntimeValidation : MonoBehaviour
    {
        private const string c_PlatformRoot = "Assets/00_Rush/01_Unit/03_Platform/";
        private const float c_SpawnDelay = 0.08f;
        private const float c_DurationPerStack = 0.3f;
        private const float c_PostBoostDelay = 0.15f;
        private const float c_Tolerance = 0.002f;
        private const float c_Timeout = 8f;
        private readonly List<UnityEngine.Object> m_Temporary = new();
        private readonly List<string> m_Checks = new();
        private readonly Vector2 m_Origin = new(10000f, 10000f);
        private GameObject m_Root;
        private PlatformHandler m_Handler;
        private Platform2D m_Platform;
        private PlatformHandlerConfig m_Config;
        private PlatformConfig m_PlatformConfig;
        private RushPlayer m_Player;
        private AetherStateComboView m_View;
        private Image m_Fill;
        private object m_OldPlayer;
        private object m_OldBank;
        private bool m_OldPlayerQuitting;
        private bool m_OldBankQuitting;
        private bool m_OldMasterShow;
        private bool m_HasFixture;
        private int m_Starts;
        private int m_Ends;
        private int m_ComboRewards;
        public string Result { get; private set; } = "Not run";
        public string[] PassedChecks => m_Checks.ToArray();

        public void Run()
        {
            if (Result == "Running") return;
            StartCoroutine(RunInternal());
        }

        private IEnumerator RunInternal()
        {
            Result = "Running";
            var stack = new Stack<IEnumerator>();
            stack.Push(ChecksInternal());
            while (stack.Count > 0)
            {
                object yielded = null;
                try
                {
                    if (!stack.Peek().MoveNext()) { stack.Pop(); continue; }
                    yielded = stack.Peek().Current;
                    if (yielded is IEnumerator nested) { stack.Push(nested); continue; }
                }
                catch (Exception exception)
                {
                    Result = "FAILED: " + exception;
                    break;
                }
                yield return yielded;
            }
            if (Result == "Running") Result = "PASS";
            CleanupInternal();
            Debug.Log("AetherState validation: " + Result + " (" + m_Checks.Count + " checks)");
        }

        private IEnumerator ChecksInternal()
        {
            SetupInternal();
            LandInternal(0);
            for (int i = 0; i < m_Config.BoostField.BoostThreshold; i++)
                m_Handler.TouchDownCheckField.SetIsStayPerfect(true, null);
            CheckInternal(m_Handler.GlobalPerfectCount == 3, "Perfect landings unlock threshold");
            m_Handler.TouchDownCheckField.SetIsStayPerfect(false, null);
            CheckInternal(m_Handler.GlobalPerfectCount == 0, "Normal landing resets streak");
            m_Handler.ActivateBoost();
            CheckInternal(!m_Handler.IsBoostActive && m_Starts == 0, "Below-threshold activation rejected");
            m_Handler.ResetProgression();

            LandInternal(3);
            CheckInternal((bool)ReadInternal(m_Handler, "m_IsSpawningNextPlatform"), "Landing schedules next platform");
            int starts = m_Starts;
            int ends = m_Ends;
            m_Handler.ActivateBoost();
            float duration = m_Config.BoostField.CalculateBoostDuration(3);
            CheckInternal(m_Handler.IsBoostActive && m_Platform.IsBoosting, "Activation starts handler and platform");
            CheckInternal(m_Starts == starts + 1 && m_Handler.CurrentBoostStock == 0, "Exactly one start event and one stock consumed");
            CheckInternal(!(bool)ReadInternal(m_Player.Jump, "m_CanJump"), "Jump locked during boost");
            CheckInternal(m_View.Content.activeSelf && ActiveButtonsInternal.Count == 1, "Combo UI opens with one button at threshold");
            CheckInternal(!(bool)ReadInternal(m_Handler, "m_IsSpawningNextPlatform"), "Activation cancels pending spawn");
            var platformCollider = m_Platform.GetComponent<Collider2D>();
            var playerCollider = m_Player.GetComponent<Collider2D>();
            CheckInternal(Physics2D.GetIgnoreCollision(platformCollider, playerCollider), "Boost collision ignore applied");
            m_Handler.ActivateBoost();
            CheckInternal(m_Starts == starts + 1, "Repeated activation cannot double-spend or double-start");
            yield return TickForInternal(c_SpawnDelay * 2f);
            CheckInternal(ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform") == m_Platform, "No stale platform spawns while boosting");
            CheckInternal(m_Handler.BoostRemainingDuration < duration, "Boost time and movement advance");
            m_View.Tick();
            CheckInternal(Mathf.Abs(m_Fill.fillAmount - m_Handler.BoostRemainingDuration / duration) < c_Tolerance,
                "Duration image uses normalized remaining time");
            int rewards = m_ComboRewards;
            InvokeInternal(m_View, "OnComboButtonPressedInternal", ActiveButtonsInternal[0]);
            CheckInternal(m_ComboRewards == rewards + 1, "Active combo button awards exactly once");

            m_Handler.Pause();
            float remaining = m_Handler.BoostRemainingDuration;
            Vector3 position = m_Platform.transform.position;
            float fill = m_Fill.fillAmount;
            rewards = m_ComboRewards;
            InvokeInternal(m_View, "OnComboButtonPressedInternal", ActiveButtonsInternal[0]);
            yield return TickForInternal(c_SpawnDelay * 2f);
            CheckInternal(Mathf.Abs(m_Handler.BoostRemainingDuration - remaining) < c_Tolerance
                && m_Platform.transform.position == position, "Pause freezes boost time and movement");
            CheckInternal(Mathf.Abs(m_Fill.fillAmount - fill) < c_Tolerance, "Pause freezes UI at the same remaining time");
            CheckInternal(m_ComboRewards == rewards, "Paused combo input cannot award rewards");
            m_Handler.Resume();
            yield return WaitUntilInternal(() => !m_Handler.IsPaused, "Resume completes");
            yield return WaitUntilInternal(() => !m_Platform.IsBoosting, "Natural boost completes");
            CheckInternal(m_Ends == ends + 1 && !m_View.Content.activeSelf && ActiveButtonsInternal.Count == 0,
                "Natural completion sends one end and clears combo UI");
            CheckInternal((bool)ReadInternal(m_Player.Jump, "m_CanJump"), "Natural completion restores jump");
            CheckInternal(!Physics2D.GetIgnoreCollision(platformCollider, playerCollider), "Natural completion restores collision");
            CheckInternal(m_Handler.IsBoostActive, "Post-boost delay keeps spawn gated");
            yield return WaitUntilInternal(() => !m_Handler.IsBoostActive, "Post-boost delay releases handler");
            CheckInternal(m_Handler.GlobalPerfectCount == 0, "Completion clears perfect streak");
            yield return WaitUntilInternal(() => ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform") != m_Platform,
                "Next platform spawns after completion");
            var next = (Platform2D)ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform");
            CheckInternal(next != null && Mathf.Abs(next.transform.position.y - m_Handler.LastContactPoint.y) < c_Tolerance,
                "Next platform spawns at boost final height");
            m_Handler.ResetProgression();

            // A moving next platform already exists when the player decides to boost.
            LandInternal(3);
            var landed = m_Platform;
            yield return WaitUntilInternal(() => ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform") != landed,
                "Next platform appears before late activation");
            next = (Platform2D)ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform");
            m_Handler.ActivateBoost();
            CheckInternal(!next.gameObject.activeSelf, "Late activation returns the old-height next platform");
            m_Handler.ResetProgression();

            LandInternal(5);
            starts = m_Starts;
            ends = m_Ends;
            m_Handler.ActivateBoost();
            CheckInternal(ActiveButtonsInternal.Count == 3 && m_Starts == starts + 1,
                "Overflow creates three buttons without duplicate initialization");
            yield return TickForInternal(c_SpawnDelay);
            platformCollider = m_Platform.GetComponent<Collider2D>();
            m_Handler.ResetProgression();
            CheckInternal(!m_Handler.IsBoostActive && !m_Platform.IsBoosting
                && ReadInternal(m_Platform, "m_BoostState").ToString() == "Idle", "Mid-boost reset clears both state machines");
            CheckInternal(m_Ends == ends + 1 && !m_View.Content.activeSelf && ActiveButtonsInternal.Count == 0,
                "Mid-boost reset closes UI and emits exactly one end");
            CheckInternal((bool)ReadInternal(m_Player.Jump, "m_CanJump")
                && !Physics2D.GetIgnoreCollision(platformCollider, playerCollider), "Mid-boost reset restores jump and collision");
            CheckInternal(m_Handler.CurrentBoostStock == m_Config.BoostField.MaxBoostStock
                && m_Handler.GlobalPerfectCount == 0, "Reset restores stock and clears combo");
            CheckInternal(PoolOccurrencesInternal(m_Platform) == 1, "Stack/current aliases return platform only once");
            m_Handler.ResetProgression();
            CheckInternal(PoolOccurrencesInternal(m_Platform) == 1 && m_Ends == ends + 1, "Repeated reset is idempotent");
            yield return TickForInternal(c_SpawnDelay * 2f);
            CheckInternal(ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform") == null, "Reset does not restart spawning");

            // Explicit StopBoost must work during both Boosting and PostBoostDelay.
            LandInternal(3);
            m_Handler.ActivateBoost();
            ends = m_Ends;
            m_Platform.StopBoost();
            CheckInternal(!m_Handler.IsBoostActive && !m_Platform.IsBoosting && m_Ends == ends + 1,
                "StopBoost clears handler and emits one end");
            m_Platform.StopBoost();
            CheckInternal(m_Ends == ends + 1, "Repeated StopBoost emits no duplicate end");
            yield return WaitUntilInternal(() => ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform") != m_Platform,
                "StopBoost resumes next-platform spawning");
            m_Handler.ResetProgression();
            LandInternal(3);
            m_Handler.ActivateBoost();
            yield return WaitUntilInternal(() => !m_Platform.IsBoosting, "Enter post-boost delay");
            ends = m_Ends;
            m_Platform.StopBoost();
            CheckInternal(!m_Handler.IsBoostActive && m_Ends == ends, "Stop during post-delay releases spawn without another end");
            m_Handler.ResetProgression();

            LandInternal(3);
            m_Handler.ActivateBoost();
            yield return WaitUntilInternal(() => !m_Platform.IsBoosting, "Enter post-delay before reset");
            m_Handler.ResetProgression();
            CheckInternal(ReadInternal(m_Platform, "m_BoostState").ToString() == "Idle", "Reset also clears post-boost delay");

            LandInternal(3);
            m_Handler.ResetProgression();
            yield return TickForInternal(c_SpawnDelay * 2f);
            CheckInternal(ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform") == null,
                "Reset cancels a spawn waiting after landing");
            m_Handler.Play();
            yield return WaitUntilInternal(() => ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform") != null,
                "Fresh Play spawns after reset");
            next = (Platform2D)ReadInternal(m_Handler, "m_CurrentNewDisplayedPlatform");
            CheckInternal(!next.IsBoosting && ReadInternal(next, "m_BoostState").ToString() == "Idle",
                "Reused platform does not resume stale boost");
        }

        private List<ComboButtonView> ActiveButtonsInternal => (List<ComboButtonView>)ReadInternal(m_View, "m_ActiveButtons");

        private void SetupInternal()
        {
            m_OldPlayer = SingletonFieldInternal(typeof(RushPlayer), "s_Instance").GetValue(null);
            m_OldBank = SingletonFieldInternal(typeof(UpdateBank), "s_Instance").GetValue(null);
            m_OldPlayerQuitting = (bool)SingletonFieldInternal(typeof(RushPlayer), "s_IsApplicationQuitting").GetValue(null);
            m_OldBankQuitting = (bool)SingletonFieldInternal(typeof(UpdateBank), "s_IsApplicationQuitting").GetValue(null);
            m_OldMasterShow = LegionKnight.MasterPanelUtility.IsShow;
            m_HasFixture = true;
            typeof(LegionKnight.MasterPanelUtility).GetField("m_IsShow", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
            m_Root = new GameObject("Aether State isolated fixture");
            m_Temporary.Add(m_Root);
            var bankObject = ChildInternal("Tick bank", false);
            bankObject.transform.SetParent(null);
            m_Temporary.Add(bankObject);
            var bank = bankObject.AddComponent<UpdateBank>();
            SingletonFieldInternal(typeof(UpdateBank), "s_Instance").SetValue(null, bank);
            SingletonFieldInternal(typeof(UpdateBank), "s_IsApplicationQuitting").SetValue(null, false);

            var playerObject = ChildInternal("Fixture player", false);
            playerObject.transform.SetParent(null);
            m_Temporary.Add(playerObject);
            playerObject.transform.position = new Vector3(20000f, 10000f, 0f);
            m_Player = playerObject.AddComponent<RushPlayer>();
            m_Player.enabled = false;
            var body = playerObject.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.constraints = RigidbodyConstraints2D.FreezeAll;
            playerObject.AddComponent<CapsuleCollider2D>();
            var jump = playerObject.AddComponent<PlayerJump>();
            jump.enabled = false;
            SetInternal(jump, "m_Rb", body);
            SetInternal(m_Player, "m_Jump", jump);
            SetInternal(m_Player, "m_PlatformSpawnPost", playerObject.transform);
            SingletonFieldInternal(typeof(RushPlayer), "s_Instance").SetValue(null, m_Player);
            SingletonFieldInternal(typeof(RushPlayer), "s_IsApplicationQuitting").SetValue(null, false);
            playerObject.SetActive(true);

            m_Config = Instantiate(AssetDatabase.LoadAssetAtPath<PlatformHandlerConfig>(
                "Assets/00_Rush/02_Stage/Config/PlatformHandleConfig/PH_Demo.asset"));
            m_Temporary.Add(m_Config);
            m_PlatformConfig = Instantiate(m_Config.InitialPlatformConfigs[0]);
            m_Temporary.Add(m_PlatformConfig);
            SetInternal(m_PlatformConfig, "m_PrewarmCount", 4);
            SetInternal(m_Config, "m_InitialPlatformConfigs", new[] { m_PlatformConfig });
            SetInternal(m_Config, "m_InitialSpawnDelay", c_SpawnDelay);
            SetInternal(m_Config, "m_NextSpawnDelay", c_SpawnDelay);
            SetInternal(m_Config.BoostField, "m_BoostDurationPerStack", c_DurationPerStack);
            SetInternal(m_Config.BoostField, "m_MinBoostDuration", c_DurationPerStack);
            SetInternal(m_Config.BoostField, "m_PostBoostSpawnDelay", c_PostBoostDelay);
            m_Handler = ChildInternal("Platform handler", true).AddComponent<PlatformHandler>();
            SetInternal(m_Handler, "m_Config", m_Config);
            var touch = new TouchDownCheckField();
            SetInternal(touch, "m_OnTouchDown", new UnityEvent<bool, ISkillContext>());
            SetInternal(touch, "m_OnNormalTouchDown", new UnityEvent<ISkillContext>());
            SetInternal(touch, "m_OnPerfectTouchDown", new UnityEvent<ISkillContext>());
            SetInternal(touch, "m_OnStayPerfectCountChange", new UnityEvent<int>());
            touch.RegisterPerfectLandingCallback(context => InvokeInternal(m_Handler, "OnGlobalPerfectLanding", context));
            touch.RegisterNormalLandingCallback(context => InvokeInternal(m_Handler, "OnGlobalNormalLanding", context));
            SetInternal(m_Handler, "m_TouchDownCheckField", touch);
            ((List<PlatformConfig>)ReadInternal(m_Handler, "m_PreparedPlatformConfigs")).Add(m_PlatformConfig);
            ((List<PlatformConfig>)ReadInternal(m_Handler, "m_WaitingListPlatformConfigs")).Add(m_PlatformConfig);
            InvokeInternal(m_Handler, "PreWarm", m_PlatformConfig, null);
            m_Handler.OnBoostStart.AddListener((duration, count) => m_Starts++);
            m_Handler.OnBoostEnd.AddListener(() => m_Ends++);

            var uiRoot = ChildInternal("Inactive UI fixture", false);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(c_PlatformRoot + "Prefabs/AetherState/Aether State Combo View.prefab");
            var viewObject = Instantiate(prefab, uiRoot.transform);
            m_View = viewObject.GetComponent<AetherStateComboView>();
            SetInternal(m_View, "m_Handler", m_Handler);
            var rewardEvent = new UnityEvent();
            rewardEvent.AddListener(() => m_ComboRewards++);
            SetInternal(m_View, "m_OnComboButtonPressed", rewardEvent);
            var fillObject = new GameObject("Duration image", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            fillObject.transform.SetParent(uiRoot.transform);
            m_Fill = fillObject.GetComponent<Image>();
            m_Fill.type = Image.Type.Filled;
            SetInternal(m_View, "m_ComboStateDurationFillImage", m_Fill);
            InvokeInternal(m_View, "Start");
            m_Handler.ResetProgression();
        }

        private void LandInternal(int perfectCount)
        {
            m_Platform = (Platform2D)InvokeInternal(m_Handler, "GetFromPool", m_PlatformConfig);
            m_Platform.StartMove(m_Origin);
            m_Platform.OnReachDestinationInvoke();
            m_Handler.SetCurrentTouchedPlatform(m_Platform);
            for (int i = 0; i < perfectCount; i++)
                m_Handler.TouchDownCheckField.SetIsStayPerfect(true, null);
        }

        private IEnumerator TickForInternal(float duration)
        {
            float deadline = Time.realtimeSinceStartup + duration;
            while (Time.realtimeSinceStartup < deadline)
            {
                TickInternal();
                yield return null;
            }
        }

        private IEnumerator WaitUntilInternal(Func<bool> condition, string label)
        {
            float deadline = Time.realtimeSinceStartup + c_Timeout;
            while (!condition())
            {
                if (Time.realtimeSinceStartup > deadline) throw new InvalidOperationException("Timeout: " + label);
                TickInternal();
                yield return null;
            }
            CheckInternal(true, label);
        }

        private void TickInternal()
        {
            if (m_Platform != null && m_Platform.gameObject.activeSelf) m_Platform.Tick();
            m_View.Tick();
        }

        private int PoolOccurrencesInternal(Platform2D platform)
        {
            int count = 0;
            var pools = (Dictionary<string, Queue<Platform2D>>)ReadInternal(m_Handler, "m_Pools");
            foreach (var pool in pools.Values)
                foreach (var entry in pool)
                    if (entry == platform) count++;
            return count;
        }

        private void CheckInternal(bool condition, string label)
        {
            if (!condition) throw new InvalidOperationException(label);
            m_Checks.Add(label);
        }

        private GameObject ChildInternal(string label, bool active)
        {
            var child = new GameObject(label);
            child.SetActive(active);
            child.transform.SetParent(m_Root.transform);
            return child;
        }

        private FieldInfo SingletonFieldInternal(Type type, string name) =>
            typeof(Singleton<>).MakeGenericType(type).GetField(name, BindingFlags.Static | BindingFlags.NonPublic);

        private FieldInfo FieldInternal(object target, string name)
        {
            for (Type type = target.GetType(); type != null; type = type.BaseType)
            {
                var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null) return field;
            }
            throw new MissingFieldException(target.GetType().Name, name);
        }

        private object ReadInternal(object target, string name) => FieldInternal(target, name).GetValue(target);
        private void SetInternal(object target, string name, object value) => FieldInternal(target, name).SetValue(target, value);
        private object InvokeInternal(object target, string name, params object[] arguments) =>
            target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(target, arguments);

        private void CleanupInternal()
        {
            if (!m_HasFixture) return;
            if (m_Handler != null) m_Handler.ResetProgression();
            // Destroy views/platforms while their bank, player and configuration still exist.
            if (m_Root != null) DestroyImmediate(m_Root);
            for (int i = m_Temporary.Count - 1; i >= 0; i--)
                if (m_Temporary[i] != null) DestroyImmediate(m_Temporary[i]);
            m_Temporary.Clear();
            SingletonFieldInternal(typeof(RushPlayer), "s_Instance").SetValue(null, m_OldPlayer);
            SingletonFieldInternal(typeof(RushPlayer), "s_IsApplicationQuitting").SetValue(null, m_OldPlayerQuitting);
            SingletonFieldInternal(typeof(UpdateBank), "s_Instance").SetValue(null, m_OldBank);
            SingletonFieldInternal(typeof(UpdateBank), "s_IsApplicationQuitting").SetValue(null, m_OldBankQuitting);
            typeof(LegionKnight.MasterPanelUtility).GetField("m_IsShow", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, m_OldMasterShow);
            m_HasFixture = false;
        }

        private void OnDestroy()
        {
            CleanupInternal();
        }
    }
}
#endif
