#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Rush.Tests
{
    public class MinionSpawnRuntimeValidation : MonoBehaviour
    {
        private const float c_Timeout = 12f;
        private const float c_Tolerance = 0.002f;
        private readonly List<UnityEngine.Object> m_Temporary = new();
        private readonly List<string> m_Checks = new();
        private readonly Dictionary<Type, object[]> m_Singletons = new();
        private readonly Vector3 m_Origin = new(10000f, 10000f, 0f);
        private GameObject m_Root;
        private MinionSpawnHandlerProbe m_Handler;
        private EnemyWaveConfig m_Wave;
        private RogueLikeCardPanel m_CardPanel;
        private LegionKnight.PausePanel m_PausePanel;
        private readonly List<string> m_Errors = new();
        private object[] m_OldCameraSingleton;
        private int m_InitEvents;
        private int m_KillEvents;
        private int m_Clears;
        private int m_BossStarts;
        private int m_BossEnds;
        public string Result { get; private set; } = "Not run";
        public string[] PassedChecks => m_Checks.ToArray();
        public string[] Errors => m_Errors.ToArray();

        public void Run()
        {
            if (Result != "Running") StartCoroutine(RunInternal());
        }

        private IEnumerator RunInternal()
        {
            Result = "Running";
            Application.logMessageReceived += HandleLogInternal;
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
                    ConfigureSkillsInternal();
                }
                catch (Exception exception)
                {
                    Result = "FAILED: " + exception;
                    break;
                }
                yield return yielded;
            }
            if (Result == "Running") Result = m_Errors.Count == 0 ? "PASS" : "FAILED: runtime errors";
            CleanupInternal();
            if (Result == "PASS" && m_Errors.Count > 0) Result = "FAILED: cleanup errors";
            Application.logMessageReceived -= HandleLogInternal;
            Debug.Log("Minion spawn validation: " + Result + " (" + m_Checks.Count + " checks)");
        }

        private IEnumerator ChecksInternal()
        {
            SetupInternal();
            m_Handler.SetNewWave(m_Wave);
            AttachObserversInternal();
            int allocated = OwnedInternal.Count;
            CheckInternal(allocated == 4, "Prewarm allocates three minions and one boss without initialization");
            CheckInternal(m_InitEvents == 0 && m_Handler.GetActiveEnemies().Count == 0,
                "Prewarm does not initialize or expose combat units");
            m_Handler.SetNewWave(m_Wave);
            CheckInternal(OwnedInternal.Count == allocated, "Repeated SetNewWave reuses prewarmed capacity");

            StartBatchInternal();
            CheckInternal(m_Handler.GetActiveEnemies().Count == 0 && PendingInternal.Count == 3,
                "Spawn reserves inactive units until preparation finishes");
            foreach (var unit in PendingInternal.Keys)
                CheckInternal(!unit.gameObject.activeInHierarchy, "Pending unit stays inactive: " + OwnedInternal[unit].name);
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3, "All three minions become ready");
            CheckInternal(m_InitEvents == 3, "Each spawn emits exactly one Unit initialization");
            var active = m_Handler.GetActiveEnemies().ToArray();
            foreach (Unit unit in active)
            {
                var probe = unit.GetComponent<MinionSpawnUnitProbe>();
                CheckInternal(probe.Initializations == 1 && probe.Resets == 1 && !probe.InitializedWhileActive,
                    "One inactive reset/init for " + unit.Config.name);
                CheckInternal(probe.EnabledAfterReady, "OnEnable sees initialized modules for " + unit.Config.name);
                CheckInternal(unit.HasBind(out Damageable damage) && damage.IsAlive && damage.Health == damage.MaxHealth,
                    "Health is ready before activation for " + unit.Config.name);
                if (unit.TryGetComponent(out PingPongMover2D mover))
                {
                    Vector3 expected = probe.PreparedLocalPosition;
                    Vector3 cached = (Vector3)ReadInternal(mover, "m_StartPosition");
                    CheckInternal(Vector3.Distance(cached, expected) < c_Tolerance,
                        "Moving minion anchors to assigned shape without extra jitter: " + unit.Config.name);
                    mover.Tick();
                    CheckInternal(Mathf.Abs(unit.transform.localPosition.y - expected.y) < c_Tolerance,
                        "First movement tick preserves spawn Y: " + unit.Config.name);
                }
            }
            var snapshot = active.Select(unit => unit.Progression.Level).ToArray();
            yield return PullMovingMinionsInternal(active);
            InvokeInternal(m_Handler, "RestState", 100);
            InvokeInternal(m_Handler, "RestState", 100);
            CheckInternal(active.Select(unit => unit.Progression.Level).SequenceEqual(snapshot),
                "Repeated rest/icon refresh does not change enemy levels");
            CheckInternal(!HasIconLevelListenerInternal(), "Production prefab icon event has no gameplay level listener");

            Unit defeated = active[0];
            int rewardCount = m_Handler.Rewards;
            int killCount = m_KillEvents;
            m_Handler.DespawnUnit(defeated);
            m_Handler.DespawnUnit(defeated);
            CheckInternal(m_Handler.Rewards == rewardCount + 1 && m_KillEvents == killCount + 1,
                "Repeated death callback grants rewards and kill event once");
            CheckInternal(PoolOccurrencesInternal(defeated) == 1, "Repeated despawn adds only one pool entry");
            rewardCount = m_Handler.Rewards;
            killCount = m_KillEvents;
            int clearCount = m_Clears;
            m_Handler.ResetProgression();
            CheckInternal(m_Handler.Rewards == rewardCount && m_KillEvents == killCount && m_Clears == clearCount,
                "Reset of living minions grants no rewards, kills, or wave clears");
            CheckInternal(m_Handler.GetActiveEnemies().Count == 0 && PendingInternal.Count == 0,
                "Reset leaves no active or pending minions");
            CheckInternal(OwnedInternal.Keys.SelectMany(unit => unit.GetComponentsInChildren<Skill>(true)).All(skill => !skill.IsActive),
                "Pooled skills are inactive even if previously registered in the tick bank");
            m_Handler.ResetProgression();
            CheckInternal(PoolsInternal.Values.SelectMany(queue => queue).Distinct().Count()
                == PoolsInternal.Values.Sum(queue => queue.Count), "Repeated reset preserves unique pool entries");

            // Reuse the same objects at a different origin and ensure no coroutine/reset leaks.
            m_Handler.SetNewWave(m_Wave);
            m_Handler.EnemyWavePost.PostToSpawn.position = m_Origin + new Vector3(100f, 50f, 0f);
            int initCount = m_InitEvents;
            StartBatchInternal();
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3, "Reused minions finish initialization");
            CheckInternal(m_InitEvents == initCount + 3 && OwnedInternal.Count == allocated,
                "Pool reuse performs one initialization per spawn without allocations");
            foreach (Unit unit in m_Handler.GetActiveEnemies())
                if (unit.TryGetComponent(out PingPongMover2D mover))
                {
                    var probe = unit.GetComponent<MinionSpawnUnitProbe>();
                    CheckInternal(Vector3.Distance((Vector3)ReadInternal(mover, "m_StartPosition"), probe.PreparedLocalPosition) < c_Tolerance,
                        "Reused mover replaces previous anchor: " + unit.Config.name);
                }
            yield return FramesInternal(12);
            CheckInternal(m_InitEvents == initCount + 3, "No duplicate initialization arrives on later frames");
            m_Handler.ResetProgression();

            // Stop after Damageable began its delayed preparation, then immediately reuse.
            m_Handler.SetNewWave(m_Wave);
            StartBatchInternal();
            yield return WaitInternal(() => m_InitEvents >= initCount + 6, "Pending initialization has begun");
            rewardCount = m_Handler.Rewards;
            m_Handler.ResetProgression();
            yield return FramesInternal(18);
            CheckInternal(PendingInternal.Count == 0 && m_Handler.GetActiveEnemies().Count == 0,
                "Reset cancels preparation before units become active");
            CheckInternal(m_Handler.Rewards == rewardCount, "Canceled preparation grants no reward");
            CheckInternal(OwnedInternal.Keys.All(unit => !unit.gameObject.activeInHierarchy),
                "Canceled units remain inactive in the pool");
            CheckInternal(OwnedInternal.Keys.SelectMany(unit => unit.GetComponentsInChildren<Skill>(true)).All(skill => !skill.IsActive),
                "Canceled preparation leaves no active skill ticks");

            m_Handler.SetNewWave(m_Wave);
            StartBatchInternal();
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3, "Fresh spawn works after canceled preparation");
            allocated = OwnedInternal.Count;
            m_Handler.SetNewWave(m_Wave);
            CheckInternal(OwnedInternal.Count == allocated, "Prewarm counts active units toward existing capacity");
            m_Handler.ResetProgression();

            // Complete the minion -> boss -> next-wave flow. The boss uses the same real unit prefab in this fixture.
            m_Handler.SetNewWave(m_Wave);
            StartBatchInternal();
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3, "Final minion wave spawns");
            foreach (Unit unit in m_Handler.GetActiveEnemies().ToArray()) m_Handler.DespawnUnit(unit);
            m_Handler.AddCurrentThreshold(1);
            CheckInternal(m_Handler.BossUnitExisten != null && PendingInternal.Count == 1,
                "Boss reservation prevents duplicate boss spawns during initialization");
            Unit boss = m_Handler.BossUnitExisten;
            m_Handler.AddCurrentThreshold(100);
            CheckInternal(PendingInternal.Count == 1, "Threshold input cannot reserve another pending boss");
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Contains(boss), "Boss finishes preparation");
            CheckInternal(m_BossStarts == 1 && (int)ReadInternal(m_Handler, "m_WaveLevel") == 2,
                "Boss spawn advances difficulty once through explicit gameplay event");
            m_Handler.DespawnUnit(boss);
            m_Handler.DespawnUnit(boss);
            CheckInternal(m_BossEnds == 1 && m_Clears == clearCount + 1, "Boss defeat clears wave exactly once");
            m_Handler.SetNewWave(m_Wave);
            CheckInternal(OwnedInternal.Count == allocated, "Looped wave does not grow the pool");
            StartBatchInternal();
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3, "Minions spawn in next wave cycle");
            CheckInternal(m_Handler.GetActiveEnemies().All(unit => unit.Progression.Level == 2),
                "Next-cycle minions inherit explicit wave difficulty");
            m_Handler.ResetProgression();
            m_Handler.SetNewWave(m_Wave);
            StartBatchInternal();
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3, "Spawn works after final reset");
            CheckInternal(m_Handler.GetActiveEnemies().All(unit => unit.Progression.Level == 1),
                "Reset restores initial minion difficulty");
            m_Handler.ResetProgression();
            rewardCount = m_Handler.Rewards;
            killCount = m_KillEvents;
            clearCount = m_Clears;
            int bossEnds = m_BossEnds;
            InvokeInternal(m_Handler, "SetWaveStateInternal", WaveState.Boss);
            m_Handler.AddCurrentThreshold(1);
            m_Handler.ResetProgression();
            yield return FramesInternal(18);
            CheckInternal(m_Handler.BossUnitExisten == null && PendingInternal.Count == 0
                && m_Handler.GetActiveEnemies().Count == 0, "Reset cancels a pending boss without delayed activation");
            InvokeInternal(m_Handler, "SetWaveStateInternal", WaveState.Boss);
            m_Handler.AddCurrentThreshold(1);
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 1, "Boss activates before reset check");
            m_Handler.ResetProgression();
            CheckInternal(m_Handler.BossUnitExisten == null && m_Handler.Rewards == rewardCount
                && m_KillEvents == killCount && m_Clears == clearCount && m_BossEnds == bossEnds,
                "Reset of pending and active bosses grants no rewards or stage progression");
            var classic = AssetDatabase.LoadAssetAtPath<EnemyWaveConfig>(
                "Assets/00_Rush/02_Stage/Config/WaveConfig/Classic Wave 1.asset");
            int previous = OwnedInternal.Count;
            m_Handler.SetNewWave(classic);
            int classicAllocated = OwnedInternal.Count;
            CheckInternal(classicAllocated == previous + 9, "Production Classic Wave 1 prewarms its nine required units once");
            m_Handler.SetNewWave(classic);
            m_Handler.SetNewWave(classic);
            CheckInternal(OwnedInternal.Count == classicAllocated, "Repeated production wave loops keep allocated capacity stable");
            yield return SpreadBatchesInternal();
            CheckInternal(m_Errors.Count == 0, "Lifecycle run has no runtime errors");
        }

        private IEnumerator PullMovingMinionsInternal(Unit[] units)
        {
            var owner = MakeInternal("Fixture Allain pulse owner", true).AddComponent<Unit>();
            SetInternal(owner, "m_Config", AssetDatabase.LoadAssetAtPath<HeroUnitConfig>(
                "Assets/00_Rush/01_Unit/00_Hero/Config/Hero_Allain.asset"));
            SetInternal(owner, "m_Progression", new ProgressField(1, 100));
            SetInternal(owner, "m_Binds", Array.Empty<MonoBehaviour>());
            var skill = new PulseFixtureSkill(AssetDatabase.LoadAssetAtPath<SkillConfig>(
                "Assets/00_Rush/01_Unit/00_Hero/Allain/Skills/Skill_Allain_AnomalySeeker.asset"), owner);
            var config = AssetDatabase.LoadAssetAtPath<AnomalyPulseConfig>(
                "Assets/00_Rush/01_Unit/00_Hero/Allain/Abilities/Ability_Allain_AnomalyCollapse.asset");
            foreach (Unit unit in units)
            {
                if (!unit.TryGetComponent(out PingPongMover2D mover)) continue;
                foreach (Unit other in units)
                    if (other != unit) other.transform.position += Vector3.up * 100f;
                unit.HasBind(out Damageable damage);
                damage.SetMaxHealth(5000);
                damage.SetHealth(5000);
                mover.Tick();
                Vector3 anchorBefore = (Vector3)ReadInternal(mover, "m_StartPosition");
                Vector3 center = damage.TargetTransform.position + Vector3.left * 3.5f;
                var pulse = Instantiate(config.DeliverPrefab, center, Quaternion.identity);
                m_Temporary.Add(pulse.gameObject);
                Physics2D.SyncTransforms();
                pulse.Init(config, skill.SkillContext);
                pulse.Activate();
                float deadline = Time.realtimeSinceStartup + c_Timeout;
                while (pulse != null && Time.realtimeSinceStartup < deadline)
                {
                    mover.Tick();
                    Physics2D.SyncTransforms();
                    yield return null;
                }
                CheckInternal(pulse == null,
                    "Actual Allain Pulse completes while patrol keeps ticking: " + unit.Config.name);
                mover.Tick();
                Vector3 pulledAnchor = (Vector3)ReadInternal(mover, "m_StartPosition");
                CheckInternal(pulledAnchor.x < anchorBefore.x - 2f,
                    "Allain pull survives simultaneous patrol updates: " + unit.Config.name);
                for (int frame = 0; frame < 20; frame++)
                {
                    mover.Tick();
                    yield return null;
                }
                CheckInternal(Vector3.Distance(pulledAnchor, (Vector3)ReadInternal(mover, "m_StartPosition")) < 0.05f,
                    "Moving minion does not snap back after pulse: " + unit.Config.name);
                // Test world-space patrol as well as the actual local-space prefab.
                SetInternal(mover, "m_UseLocalSpace", false);
                unit.gameObject.SetActive(false);
                unit.gameObject.SetActive(true);
                mover.Tick();
                Vector3 worldAnchor = (Vector3)ReadInternal(mover, "m_StartPosition");
                Vector3 displacement = new Vector3(-2f, 1f, 0f);
                unit.transform.position += displacement;
                mover.Tick();
                CheckInternal(Vector3.Distance((Vector3)ReadInternal(mover, "m_StartPosition"), worldAnchor + displacement) < 0.01f,
                    "World-space patrol preserves external displacement: " + unit.Config.name);
                SetInternal(mover, "m_UseLocalSpace", true);
            }
        }

        private IEnumerator SpreadBatchesInternal()
        {
            m_Handler.ResetProgression();
            var field = m_Wave.MinionWaveFields[0];
            var circle = (CircleSpawnShapeConfig)field.SpawnShapeConfig;
            SetInternal(circle, "m_DistributionMode", CircleDistributionMode.RandomInsideNoOverlap);
            m_Handler.SetNewWave(m_Wave);
            StartBatchInternal();
            var first = PendingInternal.Keys.Select(unit => unit.transform.position).ToArray();
            CheckInternal(first.Length == 3 && first.All(point => Vector3.Distance(point, m_Origin) <= 7.01f),
                "Distributed batch stays inside configured enemy spawn area");
            CheckInternal(MinimumDistanceInternal(first) >= 2f,
                "Distributed batch spreads minions at least configured minimum distance");
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3, "First distributed batch activates");
            m_Handler.SetNewWave(m_Wave);
            StartBatchInternal();
            var second = PendingInternal.Keys.Select(unit => unit.transform.position).ToArray();
            CheckInternal(second.Length == 3 && MinimumDistanceInternal(first.Concat(second).ToArray()) >= 2f,
                "Next batch avoids live enemies and pending spawn positions");
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 6, "Both distributed batches remain active");
            CheckInternal(((List<Vector3>)ReadInternal(m_Handler, "m_RecentMinionSpawnPositions")).Count <= 8,
                "Spawn history stays bounded");
            m_Handler.ResetProgression();
            CheckInternal(((List<Vector3>)ReadInternal(m_Handler, "m_RecentMinionSpawnPositions")).Count == 0,
                "Reset clears recent spawn positions");
            var original = field.MinionConfigs;
            SetInternal(field, "m_MinionConfigs", new[] { original[0] });
            m_Handler.SetNewWave(m_Wave);
            StartBatchInternal();
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 1, "Single-minion wave activates");
            Unit previous = m_Handler.GetActiveEnemies().First();
            Vector3 oldPosition = previous.transform.position;
            m_Handler.DespawnUnit(previous);
            m_Handler.SetNewWave(m_Wave);
            StartBatchInternal();
            Vector3 nextPosition = PendingInternal.Keys.Single().transform.position;
            CheckInternal(Vector3.Distance(oldPosition, nextPosition) >= 2f,
                "Single-minion waves avoid the previous spawn even after death");
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 1, "Single-minion next wave activates");
        }

        private float MinimumDistanceInternal(Vector3[] positions)
        {
            float distance = float.MaxValue;
            for (int i = 0; i < positions.Length; i++)
                for (int j = i + 1; j < positions.Length; j++)
                    distance = Mathf.Min(distance, Vector3.Distance(positions[i], positions[j]));
            return distance;
        }

        private sealed class PulseFixtureSkill : ISkill
        {
            public SkillConfig SkillConfig { get; private set; }
            public ProgressField Progression { get; } = new ProgressField(1, 100);
            public SkillContext SkillContext { get; private set; }
            public UnityEvent<Unit> OnAbilityDelivered { get; } = new();
            public PulseFixtureSkill(SkillConfig config, Unit owner) { Init(config, new ModuleContext(owner, owner.gameObject)); }
            public void Init(SkillConfig config, IModuleContext context) { SkillConfig = config; SkillContext = new SkillContext(this, context); }
            public void ForceActivateAll() { }
            public void ForceActivate(AbilityConfig config) { }
        }

        private Dictionary<Unit, EnemyUnitConfig> OwnedInternal => (Dictionary<Unit, EnemyUnitConfig>)ReadInternal(m_Handler, "m_UnitConfigs");
        private Dictionary<Unit, Coroutine> PendingInternal => (Dictionary<Unit, Coroutine>)ReadInternal(m_Handler, "m_PendingSpawns");
        private Dictionary<EnemyUnitConfig, Queue<Unit>> PoolsInternal => (Dictionary<EnemyUnitConfig, Queue<Unit>>)ReadInternal(m_Handler, "m_PoolsByConfig");

        private void SetupInternal()
        {
            m_Root = new GameObject("Minion spawn isolated fixture");
            var bank = MakeInternal("Fixture tick bank", false).AddComponent<UpdateBank>();
            SetSingletonInternal(typeof(UpdateBank), bank);
            var manager = MakeInternal("Fixture game context", false).AddComponent<RushGameManager>();
            SetSingletonInternal(typeof(RushGameManager), manager);
            SetInternal(manager, "m_GameConfig", AssetDatabase.LoadAssetAtPath<GameConfig>(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:GameConfig")[0])));
            var rogue = MakeInternal("Fixture roguelike", false).AddComponent<RogueLikeManager>();
            InitializeEventsInternal(rogue);
            SetInternal(manager, "m_RogueLikeManager", rogue);
            manager.gameObject.SetActive(true);
            var camera = MakeInternal("Fixture world UI camera", false).AddComponent<LegionKnight.PlayerCamera>();
            SetInternal(camera, "m_Camera", Camera.main);
            var cameraType = typeof(LegionKnight.Singleton<LegionKnight.PlayerCamera>);
            var cameraField = cameraType.GetField("m_Instance", BindingFlags.Static | BindingFlags.NonPublic);
            var cameraQuitting = cameraType.GetField("m_ApplicationIsQuitting", BindingFlags.Static | BindingFlags.NonPublic);
            m_OldCameraSingleton = new[] { cameraField.GetValue(null), cameraQuitting.GetValue(null) };
            cameraField.SetValue(null, camera);
            cameraQuitting.SetValue(null, false);
            var stage = MakeInternal("Fixture stage", true).AddComponent<StageManager>();
            SetInternal(manager, "m_StageManager", stage);
            m_Handler = MakeInternal("Fixture enemy handler", true).AddComponent<MinionSpawnHandlerProbe>();
            InitializeEventsInternal(m_Handler);
            SetInternal(stage, "m_EnemyWaveHandler", m_Handler);
            var post = MakeInternal("Fixture spawn origin", false).AddComponent<EnemyWaveSpawnPost>();
            SetInternal(post, "m_InitialPosition", m_Origin);
            SetInternal(post, "m_PostToSpawn", post.transform);
            post.transform.position = m_Origin;
            post.enabled = false;
            post.gameObject.SetActive(true);
            m_Handler.SetEnemyWavePost(post);
            m_CardPanel = MakeInternal("Fixture card panel", false).AddComponent<RogueLikeCardPanel>();
            m_PausePanel = MakeInternal("Fixture pause panel", false).AddComponent<LegionKnight.PausePanel>();

            m_Wave = Instantiate(AssetDatabase.LoadAssetAtPath<EnemyWaveConfig>(
                "Assets/00_Rush/02_Stage/Config/WaveConfig/Classic Wave 1.asset"));
            m_Temporary.Add(m_Wave);
            SetInternal(m_Wave, "m_RestThreshold", 1);
            var minions = new List<MinionUnitConfig>();
            foreach (string name in new[] { "Minion_Golem", "Minion_Rose", "Minion_Goat" })
            {
                var asset = AssetDatabase.FindAssets("t:MinionUnitConfig").Select(id =>
                    AssetDatabase.LoadAssetAtPath<MinionUnitConfig>(AssetDatabase.GUIDToAssetPath(id))).First(config => config.name == name);
                var copy = Instantiate(asset);
                m_Temporary.Add(copy);
                minions.Add(copy);
            }
            var shape = Instantiate(AssetDatabase.LoadAssetAtPath<CircleSpawnShapeConfig>(
                "Assets/00_Rush/01_Unit/02_Skill/Abilities/Damageable/Shoter/ShapeConfig/SpawnShape_Circle.asset"));
            m_Temporary.Add(shape);
            SetInternal(shape, "m_DistributionMode", CircleDistributionMode.Perimeter);
            var field = new MinionWaveField();
            SetInternal(field, "m_ThresholdToSpawn", 1);
            SetInternal(field, "m_MinionConfigs", minions.ToArray());
            SetInternal(field, "m_SpawnShapeConfig", shape);
            SetInternal(m_Wave, "m_MinionWaveFields", new[] { field });
            var boss = Instantiate(m_Wave.BossWaveField.BossConfig);
            m_Temporary.Add(boss);
            SetInternal(boss, "m_UnitPrefab", minions[0].UnitPrefab);
            SetInternal(m_Wave.BossWaveField, "m_BossConfig", boss);
            SetInternal(m_Wave.BossWaveField, "m_ThresholdToSpawn", 1);
            m_Handler.OnWaveSetCleared.AddListener(wave => m_Clears++);
            ((UnityEvent<Unit>)ReadInternal(m_Handler, "m_OnEnemyDespawn")).AddListener(unit => m_KillEvents++);
            ((UnityEvent<Unit>)ReadInternal(m_Handler, "m_OnBossSpawn")).AddListener(unit => m_BossStarts++);
            ((UnityEvent<Unit>)ReadInternal(m_Handler, "m_OnBossDespawn")).AddListener(unit => m_BossEnds++);
        }

        private void StartBatchInternal()
        {
            m_Handler.AddCurrentThreshold(1);
            m_Handler.AddCurrentThreshold(1);
        }

        private void AttachObserversInternal()
        {
            foreach (Unit unit in OwnedInternal.Keys)
            {
                var observer = unit.gameObject.AddComponent<MinionSpawnUnitProbe>();
                observer.Configure(unit);
                var binds = (MonoBehaviour[])ReadInternal(unit, "m_Binds");
                SetInternal(unit, "m_Binds", binds.Concat(new MonoBehaviour[] { observer }).ToArray());
                ((UnityEvent<Unit>)ReadInternal(unit, "m_OnInit")).AddListener(value => m_InitEvents++);
            }
        }

        private void ConfigureSkillsInternal()
        {
            if (m_Handler == null) return;
            foreach (Skill skill in OwnedInternal.Keys.SelectMany(unit => unit.GetComponentsInChildren<Skill>(true)))
            {
                SetInternal(skill, "m_CardPanel", m_CardPanel);
                SetInternal(skill, "m_PausePanel", m_PausePanel);
                if (!skill.gameObject.activeInHierarchy && skill.IsActive)
                    throw new InvalidOperationException("Inactive unit skill can tick during preparation");
            }
        }

        private IEnumerator WaitInternal(Func<bool> condition, string label)
        {
            float until = Time.realtimeSinceStartup + c_Timeout;
            while (!condition())
            {
                if (Time.realtimeSinceStartup > until) throw new InvalidOperationException("Timeout: " + label);
                yield return null;
            }
            CheckInternal(true, label);
        }

        private IEnumerator FramesInternal(int count)
        {
            for (int i = 0; i < count; i++) yield return null;
        }

        private bool HasIconLevelListenerInternal()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/00_Rush/00_Manager/RushGameManager (Singleton).prefab");
            var ev = prefab.GetComponentInChildren<EnemyWaveHandler>(true).OnWaveIconChanged;
            for (int i = 0; i < ev.GetPersistentEventCount(); i++)
                if (ev.GetPersistentTarget(i) is EnemyWaveEnemiesLevelAgent && ev.GetPersistentMethodName(i) == "AddLevel") return true;
            return false;
        }

        private int PoolOccurrencesInternal(Unit unit) => PoolsInternal.Values.Sum(queue => queue.Count(entry => entry == unit));
        private void CheckInternal(bool condition, string label)
        {
            if (!condition) throw new InvalidOperationException(label);
            m_Checks.Add(label);
        }

        private GameObject MakeInternal(string label, bool active)
        {
            var go = new GameObject(label);
            go.SetActive(active);
            m_Temporary.Add(go);
            return go;
        }

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
        private void InvokeInternal(object target, string name, params object[] args)
        {
            typeof(EnemyWaveHandler).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, args);
        }
        private void InitializeEventsInternal(object target)
        {
            for (Type type = target.GetType(); type != null; type = type.BaseType)
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    if (typeof(UnityEventBase).IsAssignableFrom(field.FieldType)) field.SetValue(target, Activator.CreateInstance(field.FieldType));
        }
        private void SetSingletonInternal(Type type, object instance)
        {
            var singleton = typeof(Singleton<>).MakeGenericType(type);
            var field = singleton.GetField("s_Instance", BindingFlags.Static | BindingFlags.NonPublic);
            var quitting = singleton.GetField("s_IsApplicationQuitting", BindingFlags.Static | BindingFlags.NonPublic);
            m_Singletons.Add(type, new[] { field.GetValue(null), quitting.GetValue(null) });
            field.SetValue(null, instance);
            quitting.SetValue(null, false);
        }
        private void HandleLogInternal(string message, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) m_Errors.Add(message);
        }
        private void CleanupInternal()
        {
            if (m_Handler != null)
            {
                m_Handler.ResetProgression();
                DestroyImmediate(m_Handler.gameObject);
            }
            // The tick bank and context remain alive while all dependent prefabs are destroyed.
            for (int i = m_Temporary.Count - 1; i >= 2; i--)
                if (m_Temporary[i] != null) DestroyImmediate(m_Temporary[i]);
            for (int i = Mathf.Min(1, m_Temporary.Count - 1); i >= 0; i--)
                if (m_Temporary[i] != null) DestroyImmediate(m_Temporary[i]);
            m_Temporary.Clear();
            if (m_Root != null) DestroyImmediate(m_Root);
            foreach (var entry in m_Singletons)
            {
                var type = typeof(Singleton<>).MakeGenericType(entry.Key);
                type.GetField("s_Instance", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, entry.Value[0]);
                type.GetField("s_IsApplicationQuitting", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, entry.Value[1]);
            }
            m_Singletons.Clear();
            if (m_OldCameraSingleton != null)
            {
                var cameraType = typeof(LegionKnight.Singleton<LegionKnight.PlayerCamera>);
                cameraType.GetField("m_Instance", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, m_OldCameraSingleton[0]);
                cameraType.GetField("m_ApplicationIsQuitting", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, m_OldCameraSingleton[1]);
                m_OldCameraSingleton = null;
            }
        }
    }

    public class MinionSpawnHandlerProbe : EnemyWaveHandler
    {
        public int Rewards { get; private set; }
        protected override void GrantDefeatRewardsInternal(EnemyUnitConfig config) { Rewards++; }
        protected override void SetBossDetailVisibleInternal(bool visible) { }
    }

    public class MinionSpawnUnitProbe : MonoBehaviour, IUnitExtension, IReseter
    {
        private Unit m_Unit;
        public IModuleContext ModuleContext { get; private set; }
        public int Initializations { get; private set; }
        public int Resets { get; private set; }
        public bool InitializedWhileActive { get; private set; }
        public bool EnabledAfterReady { get; private set; }
        public Vector3 PreparedLocalPosition { get; private set; }
        public void Configure(Unit unit) { m_Unit = unit; }
        public void Init(Unit unit)
        {
            ModuleContext = new ModuleContext(unit, gameObject);
            Initializations++;
            InitializedWhileActive |= unit.gameObject.activeInHierarchy;
            PreparedLocalPosition = unit.transform.localPosition;
        }
        public void ResetProgression() { Resets++; }
        private void OnEnable()
        {
            EnabledAfterReady = Initializations > 0 && m_Unit != null;
        }
    }
}
#endif
