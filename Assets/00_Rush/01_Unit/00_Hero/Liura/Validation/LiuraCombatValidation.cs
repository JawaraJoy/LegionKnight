#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public sealed class LiuraCombatValidation : MonoBehaviour
    {
        private const string c_Hero = "Assets/00_Rush/01_Unit/00_Hero/Config/Hero_Liura.asset";
        private const string c_Cards = "Assets/00_Rush/04_RogueLike/Config/Cards/cards/";
        private const float c_FireballLevelOneAttackMultiplier = 2.25f;
        private readonly List<CheckResult> m_Checks = new List<CheckResult>();
        private readonly List<string> m_Errors = new List<string>();
        private readonly List<Unit> m_Enemies = new List<Unit>();
        private UpdateBank m_Bank;
        private MethodInfo m_BankUpdate;
        private HeroUnitConfig m_Hero;
        private PlayerUnit m_Owner;
        private Damageable m_OwnerDamage;
        private SkillController m_Skills;
        private PlatformController m_Platforms;
        private CategorySkillController m_Weapons;
        private CategorySkillController m_Ultimates;
        private TouchDownCheckField m_Touchdown;
        private Platform2D m_Platform;
        private Skill m_Fire;
        private int m_Casts;
        private int m_DeathEvents;
        private LegionKnight.HeroUnit m_Profile;
        private UnityEngine.Object m_PreviousPlayer;
        private object m_PreviousQuitting;
        private bool m_Running;
        private float m_PreviousMaximumDelta;
        private string m_ConfigHash;
        private bool m_PreviousRunInBackground;
        public static string Result { get; private set; } = "Not run";
        public static string Json { get; private set; } = "{}";

        [Serializable]
        private sealed class CheckResult
        {
            public string name;
            public string expected;
            public string actual;
            public bool passed;
        }
        [Serializable]
        private sealed class RunResult
        {
            public string status;
            public bool playMode;
            public string configHash;
            public List<CheckResult> checks;
            public List<string> errors;
        }

        private void Start() { StartCoroutine(RunInternal()); }
        private void Update()
        {
            if (m_Running && m_Bank != null)
                m_BankUpdate.Invoke(m_Bank, null);
        }
        private IEnumerator RunInternal()
        {
            Result = "Running";
            Application.logMessageReceived += OnLogInternal;
            var stack = new Stack<IEnumerator>();
            stack.Push(ChecksInternal());
            m_Running = true;
            string failure = null;
            while (stack.Count > 0)
            {
                object current = null;
                bool next;
                try { next = stack.Peek().MoveNext(); if (next) current = stack.Peek().Current; }
                catch (Exception e) { failure = e.ToString(); break; }
                if (!next) { stack.Pop(); continue; }
                if (current is IEnumerator nested) { stack.Push(nested); continue; }
                yield return current;
            }
            m_Running = false;
            Application.logMessageReceived -= OnLogInternal;
            if (failure != null) m_Errors.Add(failure);
            Result = failure == null && m_Errors.Count == 0 ? "PASS" : "FAILED";
            Json = JsonUtility.ToJson(new RunResult { status = Result, playMode = Application.isPlaying, configHash = m_ConfigHash, checks = m_Checks, errors = m_Errors }, true);
            Debug.Log("Liura combat validation " + Result + ": " + m_Checks.Count + " checks");
        }

        private IEnumerator ChecksInternal()
        {
            m_PreviousRunInBackground = Application.runInBackground;
            Application.runInBackground = true;
            m_PreviousMaximumDelta = Time.maximumDeltaTime;
            var gameConfig = ScriptableObject.CreateInstance<GameConfig>();
            var manager = MakeInternal("Combat context").AddComponent<RushGameManager>();
            manager.enabled = false;
            SetInternal(manager, "m_GameConfig", gameConfig);
            m_Bank = MakeInternal("Tick bank").AddComponent<UpdateBank>();
            m_Bank.enabled = false;
            m_BankUpdate = typeof(UpdateBank).GetMethod("ProcessUpdateActionTick", BindingFlags.Instance | BindingFlags.NonPublic);

            // Local analytics sink runs the native death -> OnKill path without SDK/network calls.
            var analyticsRoot = MakeInternal("Local analytics sink");
            analyticsRoot.SetActive(false);
            var tenjin = analyticsRoot.AddComponent<LegionKnight.TenjinManager>();
            var sink = analyticsRoot.AddComponent<LiuraValidationAnalytics>();
            SetInternal(tenjin, "baseTenjin", sink);
            SetLegacySingletonInternal(typeof(LegionKnight.TenjinManager), tenjin);
            m_Hero = AssetDatabase.LoadAssetAtPath<HeroUnitConfig>(c_Hero);
            m_ConfigHash = AssetDatabase.GetAssetDependencyHash(c_Hero).ToString();
            var profileRoot = MakeInternal("Local profile");
            profileRoot.SetActive(false);
            var player = profileRoot.AddComponent<LegionKnight.Player>();
            var collection = profileRoot.AddComponent<LegionKnight.PlayerHeroesCollection>();
            m_Profile = new LegionKnight.HeroUnit();
            SetInternal(m_Profile, "m_HeroConfig", m_Hero);
            SetInternal(m_Profile, "m_Level", 1);
            SetInternal(m_Profile, "m_Star", 0);
            SetInternal(collection, "m_HeroUnits", new List<LegionKnight.HeroUnit> { m_Profile });
            SetInternal(player, "m_HeroesCollection", collection);
            var playerType = typeof(LegionKnight.Singleton<LegionKnight.Player>);
            m_PreviousPlayer = (UnityEngine.Object)playerType.GetField("m_Instance", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            m_PreviousQuitting = playerType.GetField("m_ApplicationIsQuitting", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            SetLegacySingletonInternal(typeof(LegionKnight.Player), player);

            var cardPanel = MakeInternal("Card panel fixture");
            cardPanel.SetActive(false);
            var cards = cardPanel.AddComponent<RogueLikeCardPanel>(); EventsInternal(cards);
            var pausePanel = MakeInternal("Pause panel fixture");
            pausePanel.SetActive(false);
            var pause = pausePanel.AddComponent<LegionKnight.PausePanel>(); EventsInternal(pause);

            var ownerRoot = MakeInternal("Liura");
            ownerRoot.layer = 11;
            m_Owner = ownerRoot.AddComponent<PlayerUnit>(); EventsInternal(m_Owner);
            m_OwnerDamage = ownerRoot.AddComponent<Damageable>(); EventsInternal(m_OwnerDamage);
            var stats = ownerRoot.AddComponent<StatController>();
            var status = ownerRoot.AddComponent<StatusEffectController>(); EventsInternal(status);
            var skillRoot = MakeInternal("Liura skills"); skillRoot.transform.SetParent(ownerRoot.transform, false);
            m_Skills = skillRoot.AddComponent<SkillController>(); EventsInternal(m_Skills);
            m_Platforms = ownerRoot.AddComponent<PlatformController>();
            m_Weapons = CategoryInternal("Weapons", m_Hero.Skills[0].Category);
            m_Ultimates = CategoryInternal("Ultimate", m_Hero.Skills[4].Category);
            SetInternal(m_Skills, "m_CategorySkillControllers", new[] { m_Weapons, m_Ultimates });
            SetInternal(m_Skills, "m_SkillSpawnPost", skillRoot.transform);
            SetInternal(stats, "m_ModifierPost", ownerRoot.transform);
            SetInternal(m_Owner, "m_Progression", new ProgressField(1, 60));
            SetInternal(m_Owner, "m_Binds", new MonoBehaviour[] { stats, status, m_OwnerDamage, m_Skills, m_Platforms });
            SetInternal(m_Owner, "m_IsPlayer", true);
            var ownerCollider = ownerRoot.AddComponent<CircleCollider2D>(); ownerCollider.radius = .3f; ownerCollider.isTrigger = true;
            ((UnityEvent<Skill>)GetInternal(m_Skills, "m_OnSkillAdded")).AddListener(skill =>
            {
                SetInternal(skill, "m_CardPanel", cards); SetInternal(skill, "m_PausePanel", pause);
                m_Weapons.AddSkill(skill); m_Ultimates.AddSkill(skill);
            });
            ((UnityEvent<Skill>)GetInternal(m_Skills, "m_OnSkillRemoved")).AddListener(skill =>
            { m_Weapons.RemoveSkill(skill); m_Ultimates.RemoveSkill(skill); });
            var initialContext = m_Skills.ModuleContext;
            m_Owner.Init(m_Hero);
            yield return WaitForUnitReadyInternal(initialContext);
            m_Fire = m_Skills.GetSkillActivator(m_Hero.Skills[4]);
            m_Fire.OnActivate.AddListener(OnFireCastInvoke);
            CheckInternal(m_Owner.BaseStats.Attack == 456 && m_OwnerDamage.MaxHealth == 789, "Initial profile stats", "ATK 456 / HP 789", m_Owner.BaseStats.Attack + " / " + m_OwnerDamage.MaxHealth);
            foreach (var config in m_Hero.Skills)
            {
                var skill = m_Skills.GetSkillActivator(config);
                CheckInternal(skill != null && skill.Progression.Level == config.LevelSet.Level, "Initial " + config.name, config.LevelSet.Level, skill == null ? -1 : skill.Progression.Level);
            }
            BuildTouchdownInternal();
            var avatarRoot = Instantiate(m_Hero.UnitPrefab.GetComponentInChildren<Spine.Unity.SkeletonAnimation>(true).gameObject, ownerRoot.transform, false);
            avatarRoot.name = "Liura avatar";
            avatarRoot.TryGetComponent(out Spine.Unity.SkeletonAnimation avatar);
            avatar.skeletonDataAsset = m_Hero.SkeletonDataAsset; avatar.Initialize(true);
            avatar.AnimationState.SetAnimation(0, "Idle", true);

            // Native projectile spawn, UpdateBank dispatch, fixed-step collision and damage.
            for (int level = 1; level <= 3; level++)
            {
                if (level > 1) UpgradeSkillFieldsInternal("Card_Add Skill Magic Missile.asset");
                for (int stage = 0; stage < 4; stage++)
                    yield return ShotCheckInternal(m_Skills.GetSkillActivator(m_Hero.Skills[stage]), "Magic Missile stage " + stage + " skill " + level);
            }
            yield return FireThresholdInternal();
            yield return FireExplosionInternal("Fire Ball level 1");
            UpgradeSkillFieldsInternal("Card_Add Skill Fire Ball.asset");
            yield return FireExplosionInternal("Fire Ball level 2");
            UpgradeSkillFieldsInternal("Card_Add Skill Fire Ball.asset");
            yield return FireExplosionInternal("Fire Ball level 3");

            ResetManaInternal();
            var charge = m_Skills.GetSkillActivator(m_Hero.Skills[5]);
            m_Skills.ForceActive(m_Hero.Skills[5]);
            yield return new WaitForSeconds(.2f);
            CheckInternal(Mathf.Approximately(m_Fire.RemainingCharge, ExpectedPowerInternal(charge)), "Mana Charge self-target", ExpectedPowerInternal(charge), m_Fire.RemainingCharge);
            ResetManaInternal();
            UpgradeSkillFieldsInternal("Card_Add Platform Mana Charge.asset");
            m_Skills.ForceActive(m_Hero.Skills[5]);
            yield return new WaitForSeconds(.2f);
            CheckInternal(Mathf.Approximately(m_Fire.RemainingCharge, ExpectedPowerInternal(charge)), "Mana Charge after card upgrade", ExpectedPowerInternal(charge), m_Fire.RemainingCharge);
            ResetManaInternal();
            UpgradeSkillFieldsInternal("Card_Add Platform Mana Charge.asset");
            m_Skills.ForceActive(m_Hero.Skills[5]);
            yield return new WaitForSeconds(.2f);
            CheckInternal(Mathf.Approximately(m_Fire.RemainingCharge, ExpectedPowerInternal(charge)), "Mana Charge level 3", ExpectedPowerInternal(charge), m_Fire.RemainingCharge);

            yield return KillCheckInternal("Mana Soul level 1");
            UpgradeSkillFieldsInternal("Card_Add Skill ManaSoul.asset");
            yield return KillCheckInternal("Mana Soul after upgrade");
            UpgradeSkillFieldsInternal("Card_Add Skill ManaSoul.asset");
            yield return KillCheckInternal("Mana Soul level 3");

            ResetManaInternal();
            ClearEnemiesInternal();
            m_Touchdown.SetIsStayPerfect(false, m_Platform.SkillContext);
            yield return new WaitForSeconds(.15f);
            CheckInternal(m_Fire.RemainingCharge == 3, "Normal landing mana", 3, m_Fire.RemainingCharge);
            ResetManaInternal();
            m_Touchdown.SetIsStayPerfect(true, m_Platform.SkillContext);
            yield return new WaitForSeconds(.2f);
            float firstPerfect = 6 + ExpectedPowerInternal(charge);
            CheckInternal(m_Fire.RemainingCharge == firstPerfect, "Perfect landing plus platform mana", firstPerfect, m_Fire.RemainingCharge);
            ResetManaInternal();
            m_Touchdown.SetIsStayPerfect(true, m_Platform.SkillContext);
            yield return new WaitForSeconds(.2f);
            float secondPerfect = 9 + ExpectedPowerInternal(charge);
            CheckInternal(m_Fire.RemainingCharge == secondPerfect, "Second perfect streak mana", secondPerfect, m_Fire.RemainingCharge);
            ResetManaInternal();
            SetInternal(m_Platform, "m_Direction", PlatformDirection.Right);
            m_Touchdown.SetIsStayPerfect(true, m_Platform.SkillContext);
            yield return new WaitForSeconds(.2f);
            float rightPerfect = 12 + ExpectedPowerInternal(charge);
            CheckInternal(m_Fire.RemainingCharge == rightPerfect, "Right platform perfect mana", rightPerfect, m_Fire.RemainingCharge);

            SetInternal(m_Profile, "m_Level", 20); SetInternal(m_Profile, "m_Star", 1);
            var previousContext = m_Skills.ModuleContext;
            m_Owner.RefreshInit();
            yield return WaitForUnitReadyInternal(previousContext);
            CheckInternal(m_Owner.Progression.Level == 20 && m_Owner.BaseStats.Attack == 516 && m_OwnerDamage.MaxHealth == 855, "Profile level 20 / breakthrough 1", "level 20 ATK 516 HP 855", m_Owner.Progression.Level + " / " + m_Owner.BaseStats.Attack + " / " + m_OwnerDamage.MaxHealth);
            CheckInternal(m_Hero.Progression.Level == 1, "Profile upgrade does not mutate HeroConfig", 1, m_Hero.Progression.Level);
            CheckInternal(m_Hero.Skills.All(config => m_Skills.GetSkillActivator(config).Progression.Level == config.LevelSet.Level), "Run reset restores configured skill levels", true, m_Hero.Skills.All(config => m_Skills.GetSkillActivator(config).Progression.Level == config.LevelSet.Level));
            for (int stage = 0; stage < 4; stage++)
                yield return ShotCheckInternal(m_Skills.GetSkillActivator(m_Hero.Skills[stage]), "Upgraded profile stage " + stage);
            yield return FireExplosionInternal("Upgraded profile Fire Ball");
            ResetManaInternal();
            m_Skills.ForceActive(m_Hero.Skills[5]);
            yield return new WaitForSeconds(.2f);
            float upgradedMana = ExpectedPowerInternal(m_Skills.GetSkillActivator(m_Hero.Skills[5]));
            CheckInternal(m_Fire.RemainingCharge == upgradedMana, "Upgraded profile Mana Charge", upgradedMana, m_Fire.RemainingCharge);
            yield return KillCheckInternal("Upgraded profile Mana Soul");
            yield return HitchShotCheckInternal();
            // Reuse the same cached skills after a run reset.
            ResetManaInternal();
            m_Touchdown.ResetProgression();
            yield return new WaitForSeconds(.1f);
            CheckInternal(m_Fire.RemainingCharge == 0 && m_Touchdown.StayPerfectCount == 0 && !m_Touchdown.IsStayPerfect, "Reset clears streak without granting mana", 0, m_Fire.RemainingCharge);
            m_Touchdown.SetIsStayPerfect(false, m_Platform.SkillContext);
            yield return new WaitForSeconds(.1f);
            CheckInternal(m_Fire.RemainingCharge == 3, "Landing after reset/reuse stays single", 3, m_Fire.RemainingCharge);
            CheckInternal(m_ConfigHash == AssetDatabase.GetAssetDependencyHash(c_Hero).ToString(), "Hero dependencies unchanged during run", m_ConfigHash, AssetDatabase.GetAssetDependencyHash(c_Hero).ToString());
            CheckInternal(m_Errors.Count == 0, "Runtime errors", 0, m_Errors.Count);
        }

        private IEnumerator WaitForUnitReadyInternal(IModuleContext previousContext)
        {
            float deadline = Time.time + 10f;
            while ((ReferenceEquals(m_Skills.ModuleContext, previousContext) || m_Skills.Skills.Count < m_Hero.Skills.Length || m_Platforms.PlatformConfigs.Count == 0) && Time.time < deadline)
                yield return null;
            if (ReferenceEquals(m_Skills.ModuleContext, previousContext) || m_Skills.Skills.Count < m_Hero.Skills.Length || m_Platforms.PlatformConfigs.Count == 0)
                throw new Exception("Unit initialization did not finish within 10 simulation seconds.");
            yield return null;
        }

        private IEnumerator ShotCheckInternal(Skill skill, string label)
        {
            ClearEnemiesInternal();
            var target = EnemyInternal(new Vector3(6f, 0f), 50000); target.HasBind(out Damageable hp);
            Physics2D.SyncTransforms();
            yield return null;
            yield return null;
            int before = hp.Health;
            int expected = Mathf.CeilToInt(ExpectedPowerInternal(skill));
            m_Skills.ForceActive(skill.SkillConfig);
            float deadline = Time.time + 2f;
            while (hp.Health == before && Time.time < deadline) yield return null;
            yield return new WaitForSeconds(.15f);
            CheckInternal(before - hp.Health == expected, label + " collision damage", expected, before - hp.Health);
            CheckInternal(((Shooter)skill.Delivers[0]).ActiveProjectiles.Count == 0, label + " returns projectile to pool", 0, ((Shooter)skill.Delivers[0]).ActiveProjectiles.Count);
        }
        private IEnumerator HitchShotCheckInternal()
        {
            float maximum = Time.maximumDeltaTime;
            float capture = Time.captureDeltaTime;
            try
            {
                Time.maximumDeltaTime = 1f / 3f;
                Time.captureDeltaTime = 1f / 3f;
                yield return ShotCheckInternal(m_Skills.GetSkillActivator(m_Hero.Skills[0]), "Forced 333ms frame Magic Missile");
            }
            finally
            {
                Time.captureDeltaTime = capture;
                Time.maximumDeltaTime = maximum;
            }
        }
        private IEnumerator FireThresholdInternal()
        {
            ClearEnemiesInternal(); ResetManaInternal();
            EnemyInternal(new Vector3(6f, 0f), 50000);
            int before = m_Casts;
            float cost = m_Fire.SkillConfig.Activation.Charge;
            m_Fire.AddCharge(cost - 1f);
            yield return new WaitForSeconds(.1f);
            CheckInternal(m_Casts == before, "Fire Ball does not cast below threshold", before, m_Casts);
            m_Fire.AddCharge(6f);
            yield return new WaitForSeconds(.1f);
            CheckInternal(m_Casts == before + 1 && m_Fire.RemainingCharge == 5f, "Fire Ball cost and overflow", "one cast / 5 mana", (m_Casts - before) + " / " + m_Fire.RemainingCharge);
            yield return new WaitForSeconds(.8f);
        }
        private IEnumerator FireExplosionInternal(string label)
        {
            ClearEnemiesInternal();
            var target = EnemyInternal(new Vector3(6f, 0f), 40000); target.HasBind(out Damageable targetHp);
            var nearby = EnemyInternal(new Vector3(6f, 2f), 50000); nearby.HasBind(out Damageable nearbyHp);
            var outside = EnemyInternal(new Vector3(6f, 8f), 50000); outside.HasBind(out Damageable outsideHp);
            Physics2D.SyncTransforms();
            yield return null;
            yield return null;
            int expected = Mathf.CeilToInt(ExpectedPowerInternal(m_Fire));
            m_Fire.ForceActivateAll();
            float deadline = Time.time + 2f;
            while (targetHp.Health == 40000 && Time.time < deadline) yield return null;
            yield return new WaitForSeconds(.2f);
            CheckInternal(40000 - targetHp.Health == expected && 50000 - nearbyHp.Health == expected && outsideHp.Health == 50000, label + " homing/explosion/range", expected + "/" + expected + "/0", (40000 - targetHp.Health) + "/" + (50000 - nearbyHp.Health) + "/" + (50000 - outsideHp.Health));
        }
        private IEnumerator KillCheckInternal(string label)
        {
            ClearEnemiesInternal(); ResetManaInternal();
            var target = EnemyInternal(new Vector3(6f, 0f), 1); target.HasBind(out Damageable hp);
            Physics2D.SyncTransforms();
            yield return null;
            yield return null;
            int deaths = m_DeathEvents;
            m_Skills.ForceActive(m_Hero.Skills[0]);
            float deadline = Time.time + 2f;
            while (hp.IsAlive && Time.time < deadline) yield return null;
            yield return new WaitForSeconds(.2f);
            float expected = ExpectedPowerInternal(m_Skills.GetSkillActivator(m_Hero.Skills[6]));
            CheckInternal(!hp.IsAlive && m_DeathEvents == deaths + 1 && m_Fire.RemainingCharge == expected, label + " native lethal hit restores mana once", expected, m_Fire.RemainingCharge);
        }

        private void BuildTouchdownInternal()
        {
            m_Touchdown = new TouchDownCheckField(); EventsInternal(m_Touchdown);
            var agent = MakeInternal("Ultimate landing agent").AddComponent<CategorySkillControllerAgent>();
            SetInternal(agent, "m_CategorySkillController", m_Ultimates);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/00_Rush/00_Player/Prefabs/Rush Player.prefab");
            var original = prefab.GetComponentsInChildren<CategorySkillControllerAgent>(true).First(a => a.name == "UltimateSkillControllerAgent");
            SetInternal(agent, "m_ChargeAmountMultiplyByPerfect", GetInternal(original, "m_ChargeAmountMultiplyByPerfect"));
            m_Touchdown.OnNormalTouchDown.AddListener(context => { m_Weapons.ForceActiveByIndex(0); agent.AddChargeByPerfect(0); });
            ((UnityEvent<int>)GetInternal(m_Touchdown, "m_OnStayPerfectCountChange")).AddListener(count => { m_Weapons.ForceActiveByIndex(count); agent.AddChargeByPerfect(count); });
            var landing = MakeInternal("Player touchdown dispatcher").AddComponent<PlayerTouchDown>();
            ((UnityEvent<bool, ISkillContext>)GetInternal(m_Touchdown, "m_OnTouchDown")).AddListener(landing.ForceSkillActivate);
            var platformRoot = MakeInternal("Mana platform fixture"); platformRoot.SetActive(false);
            m_Platform = platformRoot.AddComponent<Platform2D>();
            SetInternal(m_Platform, "m_PlatformConfig", m_Hero.UniquePlatforms[0]);
            SetInternal(m_Platform, "m_Direction", PlatformDirection.Left);
            SetInternal(m_Platform, "m_SkillContext", new SkillContext(m_Platform, new ModuleContext(m_Owner, m_Skills.gameObject)));
        }
        private Unit EnemyInternal(Vector3 position, int health)
        {
            var config = Instantiate(AssetDatabase.LoadAssetAtPath<MinionUnitConfig>("Assets/00_Rush/01_Unit/01_Enemy/Config/Minions/Minion_Demo.asset"));
            var reborn = ScriptableObject.CreateInstance<RebornConfig>();
            var so = new SerializedObject(config); so.FindProperty("m_RebornConfig").objectReferenceValue = reborn;
            so.FindProperty("m_MainStats.m_BaseStat.m_Health").floatValue = health;
            so.FindProperty("m_MainStats.m_BaseStat.m_Defense").floatValue = 0;
            so.ApplyModifiedPropertiesWithoutUndo();
            var go = MakeInternal("Target " + m_Enemies.Count); go.transform.position = position; go.layer = 20;
            var unit = go.AddComponent<Unit>(); var hp = go.AddComponent<Damageable>(); EventsInternal(hp);
            SetInternal(unit, "m_Config", config); SetInternal(unit, "m_Progression", new ProgressField(1, 60)); SetInternal(unit, "m_Binds", new MonoBehaviour[] { hp });
            hp.Init(unit); hp.RefreshDamageableStat(1f, true);
            hp.OnDeath.AddListener(context => { m_DeathEvents++; go.SetActive(false); });
            var collider = go.AddComponent<CircleCollider2D>(); collider.isTrigger = true; collider.radius = .6f;
            var body = go.AddComponent<Rigidbody2D>(); body.bodyType = RigidbodyType2D.Kinematic; body.useFullKinematicContacts = true;
            m_Enemies.Add(unit);
            return unit;
        }
        private void ClearEnemiesInternal()
        {
            foreach (var enemy in m_Enemies) if (enemy != null) { enemy.gameObject.SetActive(false); Destroy(enemy.gameObject); }
            m_Enemies.Clear();
            foreach (var skill in m_Skills.Skills)
                foreach (var deliver in skill.Delivers)
                    if (deliver is Shooter shooter)
                        foreach (var ammo in shooter.ActiveProjectiles.ToArray())
                            shooter.NotifyProjectileFinished(ammo);
        }
        private void ResetManaInternal() { SetInternal(m_Fire, "m_RemainingCharge", 0f); }
        private void UpgradeSkillFieldsInternal(string name)
        {
            var card = AssetDatabase.LoadAssetAtPath<CardConfig>(c_Cards + name);
            foreach (var item in card.SkillConfigs)
                if (item.CardPurpose == CardPurpose.SkillUp) m_Skills.AddNewSkill(item.SkillConfig);
        }
        private float ExpectedPowerInternal(Skill skill)
        {
            float factor = 1f + 0.15f * (skill.Progression.Level - 1);
            if (skill == m_Fire)
                return c_FireballLevelOneAttackMultiplier * m_Owner.BaseStats.Attack * factor;

            for (int stage = 0; stage < 4; stage++)
                if (skill.SkillConfig == m_Hero.Skills[stage])
                    return (stage + 1) * m_Owner.BaseStats.Attack * factor;

            // Mana Charge and Mana Soul both deliver whole mana.
            return Mathf.RoundToInt(26f * factor);
        }
        private CategorySkillController CategoryInternal(string name, SkillCategoryConfig config)
        {
            var category = MakeInternal(name).AddComponent<CategorySkillController>(); EventsInternal(category);
            SetInternal(category, "m_SkillCategoryConfig", config); SetInternal(category, "m_ActivationMode", ActivationMode.Queue);
            return category;
        }
        private void OnFireCastInvoke(Skill skill) { m_Casts++; }
        private GameObject MakeInternal(string name) { var go = new GameObject(name); go.transform.SetParent(transform, false); return go; }
        private void CheckInternal(bool passed, string name, object expected, object actual)
        {
            m_Checks.Add(new CheckResult { name = name, expected = expected.ToString(), actual = actual.ToString(), passed = passed });
            if (!passed) throw new Exception(name + ": expected " + expected + ", actual " + actual);
        }
        private void OnLogInternal(string condition, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) m_Errors.Add(condition + "\n" + stack);
        }
        private FieldInfo FieldInternal(object target, string name)
        {
            for (var t = target.GetType(); t != null; t = t.BaseType)
            {
                var field = t.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (field != null) return field;
            }
            throw new Exception("Missing field " + name);
        }
        private void SetInternal(object target, string name, object value) { FieldInternal(target, name).SetValue(target, value); }
        private object GetInternal(object target, string name) { return FieldInternal(target, name).GetValue(target); }
        private void EventsInternal(object target)
        {
            for (var t = target.GetType(); t != null; t = t.BaseType)
                foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    if (typeof(UnityEventBase).IsAssignableFrom(field.FieldType) && field.GetValue(target) == null)
                        field.SetValue(target, Activator.CreateInstance(field.FieldType));
        }
        private void SetLegacySingletonInternal(Type type, UnityEngine.Object instance)
        {
            var singleton = typeof(LegionKnight.Singleton<>).MakeGenericType(type);
            singleton.GetField("m_Instance", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, instance);
            singleton.GetField("m_ApplicationIsQuitting", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
        }
        private void OnDestroy()
        {
            Application.runInBackground = m_PreviousRunInBackground;
            if (m_PreviousMaximumDelta > 0f) Time.maximumDeltaTime = m_PreviousMaximumDelta;
            Application.logMessageReceived -= OnLogInternal;
            if (m_PreviousPlayer != null) SetLegacySingletonInternal(typeof(LegionKnight.Player), m_PreviousPlayer);
        }
    }
}
#endif

