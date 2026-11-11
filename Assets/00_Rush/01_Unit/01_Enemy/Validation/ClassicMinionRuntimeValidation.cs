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
    // Runs in an isolated additive Play Mode scene using real pooled minion prefabs.
    public class ClassicMinionRuntimeValidation : MonoBehaviour
    {
        private const float c_Timeout = 15f;
        private readonly List<string> m_Checks = new();
        private readonly List<string> m_Errors = new();
        private readonly List<int> m_SingleTargetHits = new();
        private readonly List<int> m_IdealAreaHits = new();
        private MinionSpawnRuntimeValidation m_Context;
        private MinionSpawnHandlerProbe m_Handler;
        private StageHandler m_Stage;
        private PlatformHandler m_Platform;
        private StageConfig m_StageConfig;
        private EnemyWaveConfig m_Wave;
        private int m_Clears;
        private int m_BossStarts;
        private int m_BossEnds;
        private int m_Victories;
        private int m_Completed;
        public string Result { get; private set; } = "Not run";
        public string[] PassedChecks => m_Checks.ToArray();
        public string[] Errors => m_Errors.ToArray();
        public int[] SingleTargetHits => m_SingleTargetHits.ToArray();
        public int[] IdealAreaHits => m_IdealAreaHits.ToArray();
        private int Level => (int)ReadInternal(m_Handler, "m_WaveLevel");
        private int Stock => (int)ReadInternal(m_Platform, "m_CurrentBoostStock");
        private Dictionary<Unit, EnemyUnitConfig> Owned => (Dictionary<Unit, EnemyUnitConfig>)ReadInternal(m_Handler, "m_UnitConfigs");
        private Dictionary<Unit, Coroutine> Pending => (Dictionary<Unit, Coroutine>)ReadInternal(m_Handler, "m_PendingSpawns");
        private List<UnityEngine.Object> Temporary => (List<UnityEngine.Object>)ReadInternal(m_Context, "m_Temporary");

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
                    if (m_Context != null) InvokeInternal(m_Context, "ConfigureSkillsInternal");
                }
                catch (Exception exception) { Result = "FAILED: " + exception; break; }
                yield return yielded;
            }
            if (Result == "Running") Result = m_Errors.Count == 0 ? "PASS" : "FAILED: runtime errors";
            try { if (m_Context != null) InvokeInternal(m_Context, "CleanupInternal"); }
            catch (Exception exception) { Result = "FAILED cleanup: " + exception; }
            if (Result == "PASS" && m_Errors.Count > 0) Result = "FAILED: cleanup errors";
            Application.logMessageReceived -= HandleLogInternal;
            Debug.Log("Classic minion validation: " + Result + " (" + m_Checks.Count + " checks)");
        }
        private IEnumerator ChecksInternal()
        {
            SetupInternal();
            var next = Instantiate(m_Wave);
            Temporary.Add(next);
            StartModeInternal(StageMode.Classic, m_Wave, next);
            CheckInternal(!m_Handler.IncludesBoss && Owned.Count == 3 && Owned.Values.All(config => config is MinionUnitConfig),
                "Classic prewarms only its three real minion prefabs, with no boss allocation");
            m_Handler.AddCurrentThreshold(1);
            CheckInternal(m_Clears == 0 && Stock == 0, "Rest transition does not complete or refill a cycle");
            m_Handler.AddCurrentThreshold(1);
            CheckInternal(m_Clears == 1 && m_Stage.CurrentWaveIndex == 1 && Level == 2 && Stock == 1,
                "Final minion batch advances to next wave, raises difficulty and restores one Aether stock");
            CheckInternal(Pending.Count == 3 && m_Handler.BossUnitExisten == null,
                "Classic continues without waiting for pending minions or reserving a boss");
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3, "First Classic batch finishes real prefab initialization");
            CheckInternal(m_Handler.GetActiveEnemies().All(unit => unit.Config is MinionUnitConfig), "Every active Classic enemy is a minion");
            m_Handler.AddCurrentThreshold(0);
            CheckInternal(m_Clears == 1 && Stock == 1, "Zero threshold cannot duplicate a completed cycle reward");
            BatchInternal();
            CheckInternal(m_Clears == 2 && m_Stage.CurrentWaveIndex == 0 && Level == 3 && Stock == 2,
                "Second Classic cycle wraps to the first wave and increases difficulty again");
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 6, "Second cycle spawns while earlier minions remain alive");
            BatchInternal();
            CheckInternal(m_Clears == 3 && Level == 4 && Stock == 2, "Further cycles remain endless and obey Aether stock capacity");
            CheckInternal(m_BossStarts == 0 && m_BossEnds == 0 && m_Victories == 0 && m_Completed == 0,
                "Classic never dispatches boss presentation, boss defeat or stage completion events");
            int clears = m_Clears;
            m_Stage.ResetProgression();
            CheckInternal(m_Handler.GetActiveEnemies().Count == 0 && Pending.Count == 0 && Level == 1 && m_Clears == clears,
                "Stage reset cancels active and pending Classic minions and restores difficulty without completing cycles");
            StartModeInternal(StageMode.Classic, m_Wave);
            BatchInternal();
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3, "Classic can restart and spawn after reset");

            StartModeInternal(StageMode.Classic, m_Wave);
            clears = m_Clears;
            m_Handler.AddCurrentThreshold(7);
            CheckInternal(m_Clears == clears + 1 && (int)ReadInternal(m_Handler, "m_CurrentThreshold") == 5,
                "Threshold overflow survives a same-config loop without recursive cycle completion");
            CheckInternal(Stock == 1 && Level == 2, "Large threshold input rewards only the completed cycle once");

            var noBoss = Instantiate(m_Wave);
            Temporary.Add(noBoss);
            SetInternal(noBoss, "m_BossWaveField", null);
            StartModeInternal(StageMode.Classic, noBoss);
            clears = m_Clears;
            BatchInternal();
            CheckInternal(m_Clears == clears + 1 && m_Handler.BossUnitExisten == null && Stock == 1,
                "Classic also works when its wave has no boss configuration");

            StartModeInternal(StageMode.Collosal, m_Wave);
            CheckInternal(m_Handler.IncludesBoss && Owned.Values.Any(config => config is BossUnitConfig), "Colossal retains boss allocation and policy");
            clears = m_Clears;
            int starts = m_BossStarts;
            int victories = m_Victories;
            BatchInternal();
            CheckInternal(m_Clears == clears && Stock == 0, "Colossal minion batch waits for boss defeat instead of giving Classic refill");
            m_Handler.AddCurrentThreshold(1);
            Unit boss = m_Handler.BossUnitExisten;
            CheckInternal(boss != null && Pending.ContainsKey(boss), "Colossal reserves its boss after the minion phase");
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Contains(boss), "Colossal boss completes initialization");
            CheckInternal(m_BossStarts == starts + 1, "Colossal still emits the boss spawn event");
            m_Handler.DespawnUnit(boss);
            m_Handler.DespawnUnit(boss);
            CheckInternal(m_Clears == clears + 1 && m_Victories == victories + 1 && m_Stage.CurrentWaveIndex == 0 && Stock == 0,
                "Colossal boss defeat advances and loops once without Classic direct refill");

            StartModeInternal(StageMode.Collosal, m_Wave);
            BatchInternal();
            m_Handler.AddCurrentThreshold(1);
            clears = m_Clears;
            int rewards = m_Handler.Rewards;
            victories = m_Victories;
            SetInternal(m_StageConfig, "m_StageMode", StageMode.Classic);
            InvokeInternal(m_Stage, "StartCurrentWaveSet");
            CheckInternal(!m_Handler.IncludesBoss && m_Handler.BossUnitExisten == null && Pending.Count == 0,
                "Switching from a pending boss to Classic cancels the old encounter");
            yield return FramesInternal(20);
            CheckInternal(m_Handler.GetActiveEnemies().Count == 0 && m_Clears == clears && m_Handler.Rewards == rewards && m_Victories == victories,
                "Canceled boss cannot activate later or grant defeat rewards and victories");

            StartModeInternal(StageMode.Adventure, m_Wave, next);
            victories = m_Victories;
            int completed = m_Completed;
            for (int i = 0; i < 2; i++)
            {
                BatchInternal();
                m_Handler.AddCurrentThreshold(1);
                boss = m_Handler.BossUnitExisten;
                CheckInternal(boss != null, "Adventure wave " + i + " reserves its boss");
                yield return WaitInternal(() => m_Handler.GetActiveEnemies().Contains(boss), "Adventure boss " + i + " finishes initialization");
                m_Handler.DespawnUnit(boss);
            }
            CheckInternal(m_Victories == victories + 1 && m_Completed == completed + 1 && m_Stage.CurrentWaveIndex == 2,
                "Adventure advances after the first boss and completes after the final boss");
            CheckInternal((bool)ReadInternal(m_Platform, "m_IsPaused") && Stock == 0, "Completed Adventure pauses platforms without Classic refill");
            clears = m_Clears;
            m_Handler.AddCurrentThreshold(100);
            m_Stage.Resume();
            CheckInternal(m_Clears == clears && m_Handler.BossUnitExisten == null && (bool)ReadInternal(m_Platform, "m_IsPaused"),
                "Completed Adventure cannot resume spawning or repeat completion");
            CheckInternal(HasProductionBossRefillInternal(), "Production boss defeat keeps its serialized Aether stock refill binding for boss modes");

            // Exercise every wave in the actual Classic stage, including its wraparound.
            var productionStage = AssetDatabase.LoadAssetAtPath<StageConfig>("Assets/00_Rush/02_Stage/Config/Stage/Stage_Classic.asset");
            CheckInternal(productionStage.EnemyWaveConfigs.Length == 8 && productionStage.EnemyWaveConfigs.Distinct().Count() == 8,
                "Classic has eight distinct alternating wave configurations");
            CheckInternal(productionStage.EnemyWaveConfigs.All(wave => AssetDatabase.GetAssetPath(wave).Contains("/WaveConfig/Classic/")
                && wave.BossWaveField.BossConfig == null && wave.MinionWaveFields.Length >= 3),
                "Alternating encounters use dedicated Classic assets and no boss reference");
            CheckInternal(productionStage.EnemyWaveConfigs.SelectMany(wave => wave.MinionWaveFields).All(field => field.ThresholdToSpawn > 0
                && field.SpawnShapeConfig != null && field.MinionConfigs.Length <= 4
                && field.MinionConfigs.Count(config => config.name.StartsWith("Minion_Goat")) <= 1),
                "Every batch has valid pacing and shape, at most four minions and at most one Goat");
            int[] expectedXp = { 7, 8, 8, 9, 9, 11, 8, 11 };
            int[] expectedProgress = { 60, 70, 78, 80, 74, 86, 120, 94 };
            int[] previousSingleHits = { 37, 49, 69, 90, 107, 157, 180, 140 };
            for (int i = 0; i < productionStage.EnemyWaveConfigs.Length; i++)
            {
                var wave = productionStage.EnemyWaveConfigs[i];
                bool focus = i % 2 == 0;
                CheckInternal(focus
                    ? wave.MinionWaveFields.Length == 5 && wave.MinionWaveFields.All(field => field.MinionConfigs.Length == 1)
                    : wave.MinionWaveFields.Length == 3 && wave.MinionWaveFields.All(field => field.MinionConfigs.Length >= 2
                        && field.MinionConfigs.Length <= 4 && field.MinionConfigs.All(config => config.name.EndsWith("_ClassicSwarm"))),
                    "Wave alternates between single-target duels and AoE groups: " + (i + 1));
                CheckInternal(wave.MinionWaveFields.Sum(field => field.MinionConfigs.Sum(config => config.ExpReward)) == expectedXp[i]
                    && wave.RestThreshold + wave.MinionWaveFields.Sum(field => field.ThresholdToSpawn) == expectedProgress[i],
                    "Wave keeps its previous XP and platform progression budget: " + (i + 1));
            }
            var variants = productionStage.EnemyWaveConfigs.SelectMany(wave => wave.MinionWaveFields).SelectMany(field => field.MinionConfigs)
                .Where(config => config.name.EndsWith("_ClassicSturdy")).Distinct().ToArray();
            CheckInternal(variants.Length == 6, "Classic uses six isolated sturdy minion variants");
            foreach (var variant in variants)
            {
                string baseName = variant.name.Replace("_ClassicSturdy", "");
                var original = AssetDatabase.LoadAssetAtPath<MinionUnitConfig>("Assets/00_Rush/01_Unit/01_Enemy/Config/Minions/" + baseName + ".asset");
                CheckInternal(original != null && variant.Skills.SequenceEqual(original.Skills) && variant.UnitPrefab == original.UnitPrefab,
                    "Sturdy variant preserves the original kit and prefab: " + variant.name);
                foreach (int level in new[] { 1, 6, 13 })
                {
                    var source = original.MainStats.GetFinalStat(level);
                    var thick = variant.MainStats.GetFinalStat(level);
                    CheckInternal(thick.Health > source.Health && thick.Attack == source.Attack && thick.Defense == source.Defense,
                        "Sturdy HP scales while ATK and DEF stay unchanged: " + variant.name + ", level " + level);
                }
            }
            var swarms = productionStage.EnemyWaveConfigs.SelectMany(wave => wave.MinionWaveFields).SelectMany(field => field.MinionConfigs)
                .Where(config => config.name.EndsWith("_ClassicSwarm")).Distinct().ToArray();
            CheckInternal(swarms.Length == 6, "AoE encounters use six isolated thin minion variants including Pari Bulan");
            foreach (var swarm in swarms)
            {
                string baseName = swarm.name.Replace("_ClassicSwarm", "");
                var original = AssetDatabase.LoadAssetAtPath<MinionUnitConfig>("Assets/00_Rush/01_Unit/01_Enemy/Config/Minions/" + baseName + ".asset");
                if (original == null)
                    original = AssetDatabase.LoadAssetAtPath<MinionUnitConfig>("Assets/00_Rush/01_Unit/01_Enemy/Minions/" + baseName + ".asset");
                CheckInternal(original != null && swarm.Skills.SequenceEqual(original.Skills) && swarm.UnitPrefab == original.UnitPrefab,
                    "Swarm preserves the original kit and prefab: " + swarm.name);
                foreach (int level in new[] { 1, 6, 13 })
                {
                    var source = original.MainStats.GetFinalStat(level);
                    var thin = swarm.MainStats.GetFinalStat(level);
                    CheckInternal(thin.Health < source.Health && thin.Attack < source.Attack && thin.Defense == source.Defense,
                        "Swarm has lower HP and ATK without added armor: " + swarm.name + ", level " + level);
                }
            }
            var referenceHero = AssetDatabase.LoadAssetAtPath<HeroUnitConfig>("Assets/00_Rush/01_Unit/00_Hero/Config/Hero_Caelis.asset");
            var referencePower = referenceHero.Skills[0].AbilitySets[0].Power.AtLevel(1);
            int referenceAttack = Mathf.RoundToInt(referencePower.InitialAmount + referencePower.MultiplierAmount * referenceHero.MainStats.GetFinalStat(1).Attack);
            var referenceAttacker = new AttackerField(referenceAttack, 0f, DamageType.CompareWithDefense);
            var productionWaves = productionStage.EnemyWaveConfigs.Select(wave => Instantiate(wave)).ToArray();
            Temporary.AddRange(productionWaves);
            StartModeInternal(StageMode.Classic, productionWaves);
            clears = m_Clears;
            starts = m_BossStarts;
            var observed = new HashSet<string>();
            for (int cycle = 0; cycle < productionWaves.Length * 2; cycle++)
            {
                var production = productionWaves[cycle % productionWaves.Length];
                m_Handler.AddCurrentThreshold(production.RestThreshold);
                CheckInternal(Pending.Count == 0 && m_Handler.GetActiveEnemies().Count == 0 && m_Clears == clears + cycle,
                    "Rest creates no enemies or early completion: wave cycle " + (cycle + 1));
                int expected = 0;
                int singleHits = 0;
                int idealAreaHits = 0;
                for (int batch = 0; batch < production.MinionWaveFields.Length; batch++)
                {
                    var field = production.MinionWaveFields[batch];
                    m_Handler.AddCurrentThreshold(field.ThresholdToSpawn - 1);
                    CheckInternal(m_Handler.GetActiveEnemies().Count + Pending.Count == expected,
                        "Next batch waits for its full platform threshold: cycle " + (cycle + 1) + ", batch " + (batch + 1));
                    var previous = new HashSet<Unit>(m_Handler.GetActiveEnemies());
                    m_Handler.AddCurrentThreshold(1);
                    expected += field.MinionConfigs.Length;
                    yield return WaitInternal(() => Pending.Count == 0,
                        "Batch preparation finishes: cycle " + (cycle + 1) + ", batch " + (batch + 1));
                    int expectedLevel = cycle + (batch == production.MinionWaveFields.Length - 1 ? 2 : 1);
                    CheckInternal(m_Handler.GetActiveEnemies().Count == expected && m_Handler.GetActiveEnemies().All(unit =>
                        unit.Config is MinionUnitConfig && unit.Progression.Level == expectedLevel
                        && unit.HasBind(out Damageable damage) && damage.IsAlive && damage.Health > 0),
                        "Batch has the expected count, difficulty and live initialized health: cycle " + (cycle + 1) + ", batch " + (batch + 1));
                    var shots = new List<int>();
                    foreach (var unit in m_Handler.GetActiveEnemies().Where(unit => !previous.Contains(unit)))
                    {
                        unit.HasBind(out Damageable target);
                        int hitDamage = DamageUtility.CalculateRawDamage(referenceAttacker, target);
                        CheckInternal(hitDamage > 0 && !target.IsImmortal && target.Shield == 0 && target.Barrier == 0,
                            "Caelis single-target damage resolves against the spawned minion: " + unit.Config.name);
                        shots.Add(Mathf.CeilToInt((float)target.MaxHealth / hitDamage));
                    }
                    singleHits += shots.Sum();
                    idealAreaHits += shots.Max();
                    foreach (var unit in m_Handler.GetActiveEnemies()) observed.Add(unit.Config.name.Replace("_ClassicSturdy", "").Replace("_ClassicSwarm", ""));
                }
                CheckInternal(m_Clears == clears + cycle + 1 && m_Handler.BossUnitExisten == null
                    && m_Stage.CurrentWaveIndex == (cycle + 1) % productionWaves.Length && Level == cycle + 2,
                    "Production Classic stage advances without a boss: wave cycle " + (cycle + 1));
                yield return WaitInternal(() => Pending.Count == 0, "Production Classic batch initialization: wave cycle " + (cycle + 1));
                CheckInternal(m_Handler.GetActiveEnemies().Count > 0 && m_Handler.GetActiveEnemies().All(unit => unit.Config is MinionUnitConfig),
                    "All active production Classic enemies are minions: wave cycle " + (cycle + 1));
                if (cycle < productionWaves.Length)
                    CheckInternal(singleHits <= previousSingleHits[cycle], "Single-target hit budget does not exceed the prior wave composition: " + (cycle + 1));
                CheckInternal(cycle % 2 == 0 ? singleHits == idealAreaHits : singleHits >= idealAreaHits * 1.5f,
                    "Damage profile reflects the intended single-target or AoE encounter: cycle " + (cycle + 1));
                m_SingleTargetHits.Add(singleHits);
                m_IdealAreaHits.Add(idealAreaHits);
                foreach (var unit in m_Handler.GetActiveEnemies().ToArray()) m_Handler.DespawnUnit(unit);
            }
            CheckInternal(m_BossStarts == starts && m_Stage.CurrentWaveIndex == 0 && Stock == 2,
                "All eight production Classic waves complete two full stage loops with capped stock and no boss events");
            CheckInternal(observed.SetEquals(new[] { "Minion_Golem", "Minion_Golem Big", "Minion_Golem Mini", "Minion_Golem Corrison",
                "Minion_Rose", "Minion_Rose Corrison", "Minion_Goat", "Minion_LoncengBara", "Minion_BijiDuri", "Minion_PariBulan" }),
                "All ten intended minion types including Lonceng Bara, Biji Duri and Pari Bulan activate in Classic");
            CheckInternal(productionWaves.All(wave => wave.MinionWaveFields.All(field =>
                field.MinionConfigs.Count(config => config.name.StartsWith("Minion_PariBulan")) <= 1)),
                "No batch stacks multiple Pari Bulan death shots");
            yield return NewMinionAbilitiesInternal();
        }
        private IEnumerator NewMinionAbilitiesInternal()
        {
            var assets = new[] { "LoncengBara", "BijiDuri", "PariBulan" }.Select(name =>
                AssetDatabase.LoadAssetAtPath<MinionUnitConfig>("Assets/00_Rush/01_Unit/01_Enemy/Minions/Minion_" + name + ".asset")).ToArray();
            CheckInternal(assets.SelectMany(config => config.Skills).SelectMany(skill => skill.AbilitySets)
                .OfType<ShooterAbilityConfig>().All(ability => ability.SpawningSetup.FireCount == 1),
                "New minion shot abilities emit one projectile per cast as designed");
            var fixtureWave = Instantiate(m_Wave);
            Temporary.Add(fixtureWave);
            SetInternal(fixtureWave.MinionWaveFields[0], "m_MinionConfigs", assets);
            StartModeInternal(StageMode.Classic, fixtureWave);
            BatchInternal();
            yield return WaitInternal(() => m_Handler.GetActiveEnemies().Count == 3 && Pending.Count == 0,
                "Three new minion prefabs initialize for their ability checks");

            var heroConfig = Instantiate(AssetDatabase.LoadAssetAtPath<MinionUnitConfig>(
                "Assets/00_Rush/01_Unit/01_Enemy/Config/Minions/Minion_Golem.asset"));
            Temporary.Add(heroConfig);
            SetInternal(heroConfig, "m_Skills", Array.Empty<SkillConfig>());
            var heroFaction = AssetDatabase.LoadAssetAtPath<HeroUnitConfig>("Assets/00_Rush/01_Unit/00_Hero/Config/Hero_Caelis.asset").Faction;
            SetInternal(heroConfig, "m_Faction", heroFaction);
            heroConfig.MainStats.BaseStat.SetHealth(100000);
            var parent = new GameObject("New minion target preparation");
            parent.SetActive(false);
            Temporary.Add(parent);
            var hero = parent.AddComponent<PlayerUnit>();
            SetInternal(hero, "m_IsPlayer", true);
            var heroHealth = parent.AddComponent<Damageable>();
            parent.AddComponent<BoxCollider2D>();
            InvokeInternal(m_Context, "InitializeEventsInternal", heroHealth);
            SetInternal(hero, "m_Progression", new ProgressField(1, 100));
            SetInternal(hero, "m_Binds", new MonoBehaviour[] { heroHealth });
            hero.transform.position = m_Handler.EnemyWavePost.PostToSpawn.position;
            yield return hero.InitializeForSpawn(heroConfig, 1);
            hero.gameObject.SetActive(true);
            SetInternal(RushPlayer.Instance, "m_Unit", hero);
            heroHealth.gameObject.layer = 11;
            CheckInternal(heroHealth.IsAlive && heroHealth.Health == 100000, "Combat target is initialized with a safe health budget");

            var biji = m_Handler.GetActiveEnemies().Single(unit => unit.Config == assets[1]);
            biji.HasBind(out Damageable bijiHealth);
            biji.HasBind(out SkillController bijiSkills);
            bijiHealth.SetHealth(bijiHealth.MaxHealth / 2);
            int beforeHeal = bijiHealth.Health;
            var healSkill = bijiSkills.GetSkillActivator(assets[1].Skills.Single(skill => skill.name == "Skill_SeedMend"));
            healSkill.ForceActivateAll();
            yield return WaitInternal(() => bijiHealth.Health > beforeHeal, "Biji Duri's real heal ability restores its own health");
            CheckInternal(bijiHealth.Health == beforeHeal + Mathf.RoundToInt(bijiHealth.MaxHealth * 0.15f),
                "Biji Duri restores exactly fifteen percent of maximum HP");

            var bell = m_Handler.GetActiveEnemies().Single(unit => unit.Config == assets[0]);
            bell.HasBind(out SkillController bellSkills);
            bell.transform.position = hero.transform.position + Vector3.left * 3f;
            Vector3 beforeMove = bell.transform.position;
            bellSkills.GetSkillActivator(assets[0].Skills.Single(skill => skill.name == "Skill_EmberAdvance")).ForceActivateAll();
            yield return WaitInternal(() => Vector3.Distance(beforeMove, bell.transform.position) > 0.5f,
                "Lonceng Bara's actual approach ability moves toward the hero");
            yield return WaitInternal(() => heroHealth.Health < 100000,
                "Lonceng Bara's timed close-range pulse damages the hero");
            CheckInternal(Vector3.Distance(bell.transform.position, hero.transform.position) >= 0.8f,
                "Lonceng Bara stops before overlapping the hero");

            var pari = m_Handler.GetActiveEnemies().Single(unit => unit.Config == assets[2]);
            pari.HasBind(out SkillController pariSkills);
            Vector3 beforeDart = pari.transform.position;
            yield return WaitInternal(() => Vector3.Distance(beforeDart, pari.transform.position) > 0.1f,
                "Pari Bulan's real dart movement runs while alive");
            var farewell = pariSkills.GetSkillActivator(assets[2].Skills.Single(skill => skill.name == "Skill_MoonFarewell"));
            var shooter = farewell.GetComponentsInChildren<Shooter>(true).Single();
            int beforeShot = shooter.ActiveProjectiles.Count;
            AbilityUltility.OnSkillEventActivates(pariSkills, ForceActiveState.OnDeath);
            yield return WaitInternal(() => shooter.ActiveProjectiles.Count == beforeShot + 1,
                "Pari Bulan's death-skill dispatch emits exactly one real projectile");
            m_Handler.ResetProgression();
            yield return FramesInternal(20);
            CheckInternal(Pending.Count == 0 && m_Handler.GetActiveEnemies().Count == 0,
                "Reset returns all new minions after their abilities have run");
            CheckInternal(Owned.Keys.Where(unit => unit != null).All(unit => !unit.gameObject.activeInHierarchy),
                "New minion ability checks leave pooled units inactive");
        }
        private void SetupInternal()
        {
            m_Context = gameObject.AddComponent<MinionSpawnRuntimeValidation>();
            InvokeInternal(m_Context, "SetupInternal");
            var cardPanel = (RogueLikeCardPanel)ReadInternal(m_Context, "m_CardPanel");
            var pausePanel = (LegionKnight.PausePanel)ReadInternal(m_Context, "m_PausePanel");
            SetInternal(cardPanel, "m_Content", cardPanel.gameObject);
            SetInternal(pausePanel, "m_Content", pausePanel.gameObject);
            m_Handler = (MinionSpawnHandlerProbe)ReadInternal(m_Context, "m_Handler");
            m_Wave = (EnemyWaveConfig)ReadInternal(m_Context, "m_Wave");
            m_Stage = RushGameManager.Instance.StageManager;
            InvokeInternal(m_Context, "InitializeEventsInternal", m_Stage);
            var playerObject = new GameObject("Classic fixture player");
            playerObject.SetActive(false);
            Temporary.Add(playerObject);
            var player = playerObject.AddComponent<RushPlayer>();
            SetInternal(player, "m_PlatformSpawnPost", player.transform);
            InvokeInternal(m_Context, "SetSingletonInternal", typeof(RushPlayer), player);
            var platformObject = new GameObject("Classic fixture platforms");
            Temporary.Add(platformObject);
            m_Platform = platformObject.AddComponent<PlatformHandler>();
            InvokeInternal(m_Context, "InitializeEventsInternal", m_Platform);
            var config = Instantiate(AssetDatabase.LoadAssetAtPath<PlatformHandlerConfig>("Assets/00_Rush/02_Stage/Config/PlatformHandleConfig/PH_Demo.asset"));
            Temporary.Add(config);
            SetInternal(config.BoostField, "m_MaxBoostStock", 2);
            SetInternal(m_Platform, "m_Config", config);
            SetInternal(m_Stage, "m_PlatformHandler", m_Platform);
            m_StageConfig = Instantiate(AssetDatabase.LoadAssetAtPath<StageConfig>("Assets/00_Rush/02_Stage/Config/Stage/Stage_Classic.asset"));
            Temporary.Add(m_StageConfig);
            SetInternal(m_Stage, "m_StageSelections", Array.Empty<StageSelectionField>());
            var callback = (UnityAction<EnemyWaveConfig>)Delegate.CreateDelegate(typeof(UnityAction<EnemyWaveConfig>), m_Stage,
                MethodInternal(m_Stage, "HandleWaveSetCleared"));
            m_Handler.OnWaveSetCleared.AddListener(callback);
            m_Handler.OnWaveSetCleared.AddListener(wave => m_Clears++);
            ((UnityEvent<Unit>)ReadInternal(m_Handler, "m_OnBossSpawn")).AddListener(unit => m_BossStarts++);
            ((UnityEvent<Unit>)ReadInternal(m_Handler, "m_OnBossDespawn")).AddListener(unit => m_BossEnds++);
            ((UnityEvent<BossUnitConfig>)ReadInternal(m_Stage, "m_OnBossDefeated")).AddListener(config => m_Victories++);
            m_Stage.OnStageCompleted.AddListener(config => m_Completed++);
        }
        private void StartModeInternal(StageMode mode, params EnemyWaveConfig[] waves)
        {
            m_Stage.ResetProgression();
            m_Platform.ConsumeBoostStock(2);
            SetInternal(m_StageConfig, "m_StageMode", mode);
            SetInternal(m_StageConfig, "m_EnemyWaveConfigs", waves);
            SetInternal(m_Stage, "m_UsedStageConfig", m_StageConfig);
            SetInternal(m_Stage, "m_CurrentWaveIndex", 0);
            InvokeInternal(m_Stage, "StartCurrentWaveSet");
        }
        private void BatchInternal() { m_Handler.AddCurrentThreshold(1); m_Handler.AddCurrentThreshold(1); }
        private bool HasProductionBossRefillInternal()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/00_Rush/00_Manager/RushGameManager (Singleton).prefab");
            var handler = prefab.GetComponentInChildren<EnemyWaveHandler>(true);
            var ev = (UnityEvent<Unit>)ReadInternal(handler, "m_OnBossDespawn");
            return Enumerable.Range(0, ev.GetPersistentEventCount()).Any(i => ev.GetPersistentTarget(i) is PlatformHandler && ev.GetPersistentMethodName(i) == "AddBoostStock");
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
        private IEnumerator FramesInternal(int count) { for (int i = 0; i < count; i++) yield return null; }
        private void CheckInternal(bool condition, string label)
        {
            if (!condition) throw new InvalidOperationException(label);
            m_Checks.Add(label);
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
        private MethodInfo MethodInternal(object target, string name)
        {
            for (Type type = target.GetType(); type != null; type = type.BaseType)
            {
                var method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                if (method != null) return method;
            }
            throw new MissingMethodException(target.GetType().Name, name);
        }
        private object ReadInternal(object target, string name) => FieldInternal(target, name).GetValue(target);
        private void SetInternal(object target, string name, object value) => FieldInternal(target, name).SetValue(target, value);
        private void InvokeInternal(object target, string name, params object[] args) => MethodInternal(target, name).Invoke(target, args);
        private void HandleLogInternal(string message, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) m_Errors.Add(message);
        }
    }
}
#endif
