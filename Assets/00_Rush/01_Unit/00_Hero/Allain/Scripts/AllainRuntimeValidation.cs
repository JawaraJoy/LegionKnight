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
    // Editor-only isolated smoke test; no player profile or production scene is changed.
    public class AllainRuntimeValidation : MonoBehaviour
    {
        private const string c_Folder = "Assets/00_Rush/01_Unit/00_Hero/Allain/";
        private const float c_AegisLifeStealPerStack = 0.01f;
        private readonly List<UnityEngine.Object> m_Temporary = new List<UnityEngine.Object>();
        private readonly List<string> m_Checks = new List<string>();
        public static string Result { get; private set; } = "Not run";
        public void Run() => StartCoroutine(RunInternal());
        private IEnumerator RunInternal()
        {
            Result = "Running";
            var routine = ChecksInternal();
            while (true)
            {
                bool next;
                try { next = routine.MoveNext(); }
                catch (Exception e) { Result = "FAILED: " + e + "\nPassed: " + string.Join("; ", m_Checks); break; }
                if (!next) { Result = "PASS: " + string.Join("; ", m_Checks); break; }
                yield return routine.Current;
            }
            for (int i = m_Temporary.Count - 1; i >= 0; i--)
                if (m_Temporary[i] != null) Destroy(m_Temporary[i]);
            Debug.Log(Result);
        }
        private IEnumerator ChecksInternal()
        {
            var bank = MakeInternal("Validation tick bank").AddComponent<UpdateBank>(); bank.enabled = false;
            var manager = MakeInternal("Validation combat context").AddComponent<RushGameManager>(); manager.enabled = false;
            var gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:GameConfig")[0]));
            SetInternal(manager, "m_GameConfig", gameConfig);
            var heroConfig = AssetDatabase.LoadAssetAtPath<HeroUnitConfig>("Assets/00_Rush/01_Unit/00_Hero/Config/Hero_Allain.asset");
            int baseAttack = heroConfig.MainStats.GetFinalStat(1).Attack;
            ValidateUpgradeCardsInternal(heroConfig, baseAttack);
            var entityConfig = Load<MinionUnitConfig>("Entities/Minion_AnomalyEntity.asset");
            var owner = UnitInternal(heroConfig, new Vector3(10000, 0, 0), true);
            var healTextRoot=new GameObject("Validation heal text");healTextRoot.transform.SetParent(owner.transform,false);
            var healSpawner=healTextRoot.AddComponent<FloatingDamageTextSpawner>();
            SetInternal(healSpawner,"m_Config",AssetDatabase.LoadAssetAtPath<FloatingDamageTextConfig>("Assets/00_Rush/Floating Text/Config/HealText.asset"));
            owner.HasBind(out Damageable healOwner);int healEvents=0;int healAmount=0;
            healOwner.OnHealthRestored.AddListener(healSpawner.SpawnText);
            healOwner.OnHealthRestored.AddListener(amount=>{healEvents++;healAmount=amount;});
            var skill = new FixtureSkill(Load<SkillConfig>("Skills/Skill_Allain_AnomalySeeker.asset"), owner);
            var ability = Load<ShooterAbilityConfig>("Abilities/Ability_Allain_AnomalySeeker.asset");
            var shooter = Instantiate(ability.DeliverPrefab, owner.transform.position, Quaternion.identity) as Shooter;
            m_Temporary.Add(shooter.gameObject); shooter.Init(ability, skill.SkillContext);
            var targets = new List<Unit>();
            for (int i = 0; i < 40; i++) targets.Add(UnitInternal(entityConfig, owner.transform.position + new Vector3(10 + i % 8, i / 8, 0), false));
            targets[0].gameObject.AddComponent<CircleCollider2D>().radius = .1f;
            Physics2D.SyncTransforms();
            yield return new WaitForSeconds(.05f);
            var candidates = AbilityUltility.ApplyTargetPriority(new AbilityContext(shooter, skill.SkillContext));
            CheckInternal(candidates.Count == 40, "Seeker finds all 40 enemies and deduplicates colliders");
            shooter.Activate();
            CheckInternal(shooter.ActiveProjectiles.Count == 40, "Seeker launches one projectile per target");
            CheckInternal(Mathf.Approximately(AbilityUltility.GetFinalPowerAmount(shooter.AbilityContext), baseAttack) && skill.SkillConfig.Activation.Charge == 80, "Seeker deals 100% ATK at level one and costs 80 charge");
            foreach (var target in targets) target.gameObject.SetActive(false);
            shooter.StopAllCoroutines();
            foreach (var projectile in shooter.ActiveProjectiles) projectile.gameObject.SetActive(false);

            var flightTarget = UnitInternal(entityConfig, owner.transform.position + Vector3.right * 20, false);
            flightTarget.HasBind(out Damageable flightDamage);
            flightTarget.GetComponent<CircleCollider2D>().radius = .6f;
            var flightBody = flightTarget.gameObject.AddComponent<Rigidbody2D>(); flightBody.bodyType = RigidbodyType2D.Kinematic; flightBody.useFullKinematicContacts = true;
            Physics2D.SyncTransforms();
            shooter.ActiveOverrideTarget(new List<ITargetable>{flightDamage});
            var flying = shooter.ActiveProjectiles.First(p=>p.gameObject.activeSelf);
            float maxLateral = 0f; float flightTime = 0f; bool hitFeedback = false;
            while(flightTime < 2f && flying.gameObject.activeSelf)
            {
                flying.Tick();
                maxLateral = Mathf.Max(maxLateral, Mathf.Abs(flying.transform.position.y-owner.transform.position.y));
                flightTime += Time.deltaTime;
                yield return null;
                hitFeedback |= UnityEngine.Object.FindObjectsByType<AllainFeedbackEffect>(FindObjectsSortMode.None).Any(e=>e.name.Contains("Impact"));
            }
            CheckInternal(maxLateral > .08f, "Seeker bends laterally in flight; deviation="+maxLateral);
            CheckInternal(flightDamage.Health < 5000, "Sway projectile still hits its homing target");
            CheckInternal(hitFeedback, "Native projectile impact emits VFX/SFX");
            flightTarget.gameObject.SetActive(false);

            var marked = UnitInternal(entityConfig, owner.transform.position + Vector3.up * 100, false);
            var pulled = UnitInternal(entityConfig, marked.transform.position + Vector3.right * 3.5f, false);
            var near = UnitInternal(entityConfig, marked.transform.position + Vector3.left, false);
            var outside = UnitInternal(entityConfig, marked.transform.position + Vector3.right * 5, false);
            var extended = UnitInternal(entityConfig, marked.transform.position + Vector3.right * 5.5f, false);
            var beyond = UnitInternal(entityConfig, marked.transform.position + Vector3.right * 6.5f, false);
            var boundary = UnitInternal(entityConfig, marked.transform.position + Vector3.right * 6f, false);
            var bossConfig = AssetDatabase.LoadAssetAtPath<BossUnitConfig>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:BossUnitConfig")[0]));
            var bossNear = UnitInternal(bossConfig, marked.transform.position + Vector3.up * 2f, false);
            var bossFar = UnitInternal(bossConfig, marked.transform.position + Vector3.up * 5.5f, false);
            var bossNearStart = bossNear.transform.position; var bossFarStart = bossFar.transform.position;
            bossNear.HasBind(out Damageable bossNearDamage); bossFar.HasBind(out Damageable bossFarDamage);
            extended.HasBind(out Damageable extendedDamage); beyond.HasBind(out Damageable beyondDamage); boundary.HasBind(out Damageable boundaryDamage);
            marked.HasBind(out Damageable markedDamage); pulled.HasBind(out Damageable pulledDamage);
            near.HasBind(out Damageable nearDamage); outside.HasBind(out Damageable outsideDamage);
            yield return new WaitForSeconds(.05f);
            Physics2D.SyncTransforms();
            markedDamage.TakeDamage(shooter.AbilityContext);


            markedDamage.TakeDamage(shooter.AbilityContext);
            yield return null;
            var signVisual=marked.GetComponentInChildren<AllainSignVisual>();
            var signAura=signVisual.GetComponentInChildren<ParticleSystem>();
            CheckInternal(signAura.isPlaying, "Anomaly Sign aura plays on marked enemy");
            var signCounter=signVisual.GetComponentInChildren<LegionKnight.WeaponCounterView>(true);
            var signImages=(List<UnityEngine.UI.Image>)GetInternal(signCounter,"m_Actives");
            CheckInternal(signImages.Count==3 && signImages.Count(i=>i.gameObject.activeInHierarchy)==2, "Anomaly Sign native bar displays 2 of 3 segments");
            CheckInternal(nearDamage.Health == 5000, "No Collapse before 3 stacks");
            markedDamage.TakeDamage(shooter.AbilityContext);
            yield return new WaitForSeconds(.4f);
            CheckInternal(Mathf.Abs(extended.transform.position.x-marked.transform.position.x-1.5f)<.03f, "Extended 6m pull includes target at 5.5m");
            CheckInternal(Mathf.Abs(beyond.transform.position.x-marked.transform.position.x-6.5f)<.03f, "Target beyond 6m is not pulled");
            CheckInternal(!signAura.isPlaying, "Anomaly Sign aura clears when three stacks are consumed");
            CheckInternal(!signCounter.gameObject.activeInHierarchy, "Anomaly Sign segment bar hides when stacks are consumed");
            CheckInternal(extendedDamage.Health==5000-baseAttack && boundaryDamage.Health==5000-baseAttack, "6m damage radius includes 5.5m and exactly 6m targets");
            CheckInternal(beyondDamage.Health==5000, "Target at 6.5m takes no Pulse damage");
            CheckInternal(Vector3.Distance(bossNearStart,bossNear.transform.position)<.001f && Vector3.Distance(bossFarStart,bossFar.transform.position)<.001f, "Bosses at 2m and 5.5m resist all Pulse displacement");
            CheckInternal(bossNearDamage.Health==5000-baseAttack && bossFarDamage.Health==5000-baseAttack, "Bosses still take Pulse damage throughout the 6m area");
            CheckInternal(nearDamage.Health == 5000 - baseAttack, "Third Sign triggers 100% ATK prototype Collapse once; actual HP=" + nearDamage.Health);
            CheckInternal(Mathf.Abs(Vector3.Distance(pulled.transform.position, marked.transform.position)) < .03f, "Pull from 3.5m stops at center with 4m cap");
            CheckInternal(Vector3.Distance(near.transform.position, marked.transform.position) < .03f, "Pull never overshoots center");
            CheckInternal(outsideDamage.Health == 5000-baseAttack && Mathf.Abs(outside.transform.position.x - marked.transform.position.x - 1) < .01f, "Enemy at 5m takes Pulse damage and is pulled exactly 4m");
            markedDamage.TakeDamage(shooter.AbilityContext);
            CheckInternal(nearDamage.Health == 5000 - baseAttack, "Sign restarts after consuming 3 stacks");

            var aegisConfig = Load<AegisConversionConfig>("Abilities/Ability_Allain_AegisConversion.asset");
            var aegis = Instantiate(aegisConfig.DeliverPrefab) as AegisConversion; m_Temporary.Add(aegis.gameObject);
            aegis.Init(aegisConfig, skill.SkillContext); aegis.Activate();
            owner.HasBind(out Damageable ownerDamage); owner.HasBind(out StatController stats);
            CheckInternal(ownerDamage.Shield == 0 && stats.GetFinalStat(heroConfig.MainStats.GetFinalStat(1)).Attack == baseAttack, "Aegis leaves ATK unchanged and grants no shield on first activation");
            CheckInternal(Mathf.Approximately(stats.LifeStealRate, c_AegisLifeStealPerStack), "Aegis first stack gives 1% life steal");
            var stackBar=stats.StatModifiers[0].GetComponentInChildren<LegionKnight.WeaponCounterView>(true);
            var stackImages=(List<UnityEngine.UI.Image>)GetInternal(stackBar,"m_Actives");
            CheckInternal(stackImages.Count==5 && stackImages.Count(i=>i.gameObject.activeInHierarchy)==1, "Aegis native stack bar shows 1 of 5 segments");
            var aura=stats.StatModifiers[0].GetComponentInChildren<AllainBuffVisual>().GetComponentInChildren<ParticleSystem>();
            CheckInternal(aura != null && aura.isPlaying, "Aegis character aura is playing while buff is active");
            var orb=aura.transform.GetChild(0);var orbBefore=orb.localPosition;
            yield return new WaitForSeconds(.1f);
            CheckInternal(Vector3.Distance(orbBefore,orb.localPosition)>.01f, "Aegis orbs move around character");
            CheckInternal(aura.GetComponentsInChildren<Renderer>().All(r=>r.sortingLayerName=="Vfx"), "Aegis orb renderers use foreground Vfx layer");
            ownerDamage.SetHealth(100);
            var drainTarget=UnitInternal(entityConfig,owner.transform.position+Vector3.up*200,false);
            drainTarget.HasBind(out Damageable drainDamage);int healthBefore=drainDamage.Health;
            drainDamage.TakeDamage(shooter.AbilityContext);
            CheckInternal(ownerDamage.Health==100+Mathf.RoundToInt((healthBefore-drainDamage.Health) * c_AegisLifeStealPerStack), "One-stack life steal heals 1% of HP damage dealt");
            CheckInternal(healEvents>0 && healAmount==ownerDamage.Health-100, "Life steal emits restored amount for heal text");
            SetInternal(stats.StatModifiers[0], "m_RemainingDuration", 2f); aegis.Activate();
            CheckInternal(ownerDamage.Shield == 0 && stats.StatModifiers.Count == 1 && stats.StatModifiers[0].RemainingDuration == 5f && stats.StatModifiers[0].StackCount == 2, "Aegis stacks and refreshes to 5 seconds");
            for (int i=0;i<6;i++) aegis.Activate();
            CheckInternal(stats.StatModifiers[0].StackCount == 5 && stats.GetFinalStat(heroConfig.MainStats.GetFinalStat(1)).Attack == baseAttack, "Aegis grants no ATK even at five stacks");
            float fullStackLifeSteal = 5 * c_AegisLifeStealPerStack;
            CheckInternal(Mathf.Approximately(stats.LifeStealRate, fullStackLifeSteal), "Five stacks grant 5% life steal at level one");
            CheckInternal(stackImages.Count(i=>i.gameObject.activeInHierarchy)==5, "Aegis native stack bar caps at 5 lit segments");
            ownerDamage.SetHealth(100);healthBefore=drainDamage.Health;drainDamage.TakeDamage(shooter.AbilityContext);
            CheckInternal(ownerDamage.Health==100+Mathf.RoundToInt((healthBefore-drainDamage.Health) * fullStackLifeSteal), "Five-stack life steal heals 5% of HP damage dealt");
            var guarded=UnitInternal(entityConfig,owner.transform.position+Vector3.up*220,false);guarded.HasBind(out Damageable guardedDamage);
            ownerDamage.SetHealth(100);guardedDamage.AddBarrier(1,false);guardedDamage.TakeDamage(shooter.AbilityContext);
            CheckInternal(ownerDamage.Health==100, "Barrier-blocked damage gives no life steal");
            guardedDamage.AddShield(5000,false);guardedDamage.TakeDamage(shooter.AbilityContext);
            CheckInternal(ownerDamage.Health==100, "Shield-absorbed damage gives no life steal");
            var almostDead=UnitInternal(entityConfig,owner.transform.position+Vector3.up*240,false);almostDead.HasBind(out Damageable lowDamage);
            SetInternal(lowDamage,"m_IsImmortal",true);lowDamage.SetHealth(30);lowDamage.TakeDamage(shooter.AbilityContext);
            CheckInternal(ownerDamage.Health==100+Mathf.RoundToInt(29 * fullStackLifeSteal) && lowDamage.Health==1, "Life steal uses the 29 HP lost to an overkill hit");
            ownerDamage.SetHealth(ownerDamage.MaxHealth-1);var capped=UnitInternal(entityConfig,owner.transform.position+Vector3.up*260,false);capped.HasBind(out Damageable cappedDamage);cappedDamage.TakeDamage(shooter.AbilityContext);
            CheckInternal(ownerDamage.Health==ownerDamage.MaxHealth, "Life steal does not exceed max HP");
            SetInternal(stats.StatModifiers[0], "m_RemainingDuration", 0f); stats.StatModifiers[0].Tick();
            CheckInternal(stats.GetFinalStat(heroConfig.MainStats.GetFinalStat(1)).Attack == baseAttack && ownerDamage.Shield == 0, "ATK and shield stay unchanged on Aegis expiry");

            CheckInternal(stats.LifeStealRate==0f && !aura.isPlaying, "Life steal and character aura stop when Aegis expires");
            CheckInternal(!stackBar.gameObject.activeInHierarchy, "Aegis stack bar hides when buff expires");
            ownerDamage.SetHealth(100);cappedDamage.TakeDamage(shooter.AbilityContext);
            CheckInternal(ownerDamage.Health==100, "Damage after buff expiry gives no life steal");

            healOwner.SetHealth(healOwner.MaxHealth-3);int eventsBefore=healEvents;healOwner.SimpleHeal(100);
            CheckInternal(healEvents==eventsBefore+1 && healAmount==3, "Heal reports actual 3 HP restored, excluding overheal");
            CheckInternal(healTextRoot.GetComponentsInChildren<TMPro.TextMeshPro>().Any(t=>t.text.Contains("+3")), "Actual HP recovery spawns native green +3 text");
            healOwner.SimpleHeal(100);
            CheckInternal(healEvents==eventsBefore+1, "Full HP does not emit false healing text");
            var stage = MakeInternal("Validation stage").AddComponent<StageManager>(); stage.enabled = false; SetInternal(manager, "m_StageManager", stage);
            var handler = MakeInternal("Validation enemy waves").AddComponent<EnemyWaveHandler>(); handler.enabled = false; SetInternal(stage, "m_EnemyWaveHandler", handler);
            var post = MakeInternal("Validation spawn post").AddComponent<EnemyWaveSpawnPost>(); post.enabled = false;
            post.transform.position = owner.transform.position + Vector3.down * 100;
            SetInternal(post, "m_PostToSpawn", post.transform); handler.SetEnemyWavePost(post);
            var wave = ScriptableObject.CreateInstance<EnemyWaveConfig>(); m_Temporary.Add(wave);
            var waveField = new MinionWaveField(); SetInternal(waveField, "m_MinionConfigs", new[]{entityConfig,entityConfig,entityConfig});
            SetInternal(wave, "m_MinionWaveFields", new[]{waveField}); SetInternal(handler, "m_CurrentEnemyWave", wave);
            var incursionConfig = Load<AnomalyIncursionConfig>("Abilities/Ability_Allain_AnomalyIncursion.asset");
            var incursion = Instantiate(incursionConfig.DeliverPrefab) as AnomalyIncursion; m_Temporary.Add(incursion.gameObject); incursion.Init(incursionConfig, skill.SkillContext); incursion.Activate();
            for (int i=0;i<25;i++) yield return new WaitForSeconds(.05f);
            var entities = (List<Unit>)GetInternal(incursion, "m_Entities");
            CheckInternal(entities.Count == 3 && entities.All(u=>u.Config is EnemyUnitConfig && u.Config.Faction == entityConfig.Faction), "Incursion creates 3 EnemyUnit entities");
            CheckInternal(entities.All(u=>u.HasBind(out Damageable d) && d.Health == 100 && d.Barrier == 0), "All entities initialize with 100 HP and no barriers");
            CheckInternal(entities.All(u=>u.GetComponentsInChildren<MeshRenderer>().Any(r=>r.sortingLayerName=="Character")), "Entity avatar sorts on Character above Environment");
            CheckInternal(entities.All(u=>Vector3.Distance(u.transform.position,post.transform.position)<.01f), "Entities use enemy spawn post, not a live target");
            CheckInternal(entities.All(u=>u.HasBind(out AvatarSpine a) && a.SkeletonAnimation.AnimationState.GetTrack(0).Animation.Name == "Idle"), "All entities play Idle on spawn");
            var entity = entities[0]; entity.HasBind(out Damageable entityDamage);
            var healthView=entity.GetComponentInChildren<HealthSliderView>(true);
            var barriers=entity.GetComponentsInChildren<CountStockView>(true).First(v=>v.name=="Barriers Container");
            CheckInternal(healthView.gameObject.activeInHierarchy && Mathf.Approximately((float)GetInternal(healthView,"m_Rate"),1f), "Entity native healthbar shows full 100 HP");
            CheckInternal(!barriers.IsShown && entityDamage.Barrier==0, "Entity has no barrier and barrier UI is hidden");
            for(int i=1;i<entities.Count;i++) entities[i].gameObject.SetActive(false); // Isolate one explosion from valid entity chain reactions.
            var neighbor = UnitInternal(entityConfig, entity.transform.position + Vector3.right, false); neighbor.HasBind(out Damageable neighborDamage);
            yield return new WaitForSeconds(.05f); Physics2D.SyncTransforms();
            SetInternal(entityDamage, "m_Health", 0); OnDeathFixtureInvoke(entityDamage, shooter.AbilityContext);
            CheckInternal(!entity.GetComponent<Collider2D>().enabled && entity.HasBind(out AvatarSpine deadAvatar) && deadAvatar.SkeletonAnimation.AnimationState.GetTrack(0).Animation.Name == "Dead", "Death event disables collision and plays Dead");
            yield return new WaitForSeconds(.1f);
            CheckInternal(neighborDamage.Health == 5000 - Mathf.CeilToInt(baseAttack * .5f), "Entity death explosion deals 50% current Allain ATK");
            yield return new WaitForSeconds(.65f);
            CheckInternal(entity == null, "Entity despawns after Dead finishes");
            foreach(var cue in new[]{"VFX_Allain_Muzzle","VFX_Allain_Impact","VFX_Allain_Collapse","VFX_Allain_AttackGain","VFX_Allain_SeekerCast","VFX_Allain_IncursionCast","VFX_Allain_EntitySpawn","VFX_Allain_EntityExplosion","SFX_Allain_Sign"})
            {
                yield return new WaitForSeconds(.2f);
                var sound=Instantiate(Load<GameObject>("Feedback/"+cue+".prefab"));m_Temporary.Add(sound);
                sound.GetComponent<AllainFeedbackEffect>().Play();
                var audio=sound.GetComponentInChildren<AudioSource>();
                CheckInternal(audio.clip!=null && audio.isPlaying && audio.volume>=.2f, "Audible audio source starts: "+cue);
                if(cue=="VFX_Allain_Collapse")
                {
                    var areaLines=sound.GetComponentsInChildren<LineRenderer>();
                    CheckInternal(areaLines.Length>0 && areaLines.All(l=>l.enabled), "Pulse area boundary is visible on activation");
                    yield return new WaitForSeconds(.9f);
                    CheckInternal(areaLines.All(l=>l==null || !l.enabled), "Pulse area boundary fades while its SFX finishes");
                }
            }
        }
        private void ValidateUpgradeCardsInternal(HeroUnitConfig hero, int attack)
        {
            var player = UnitInternal(hero, new Vector3(22000f, 0f), true);
            try
            {
                var skills = player.gameObject.AddComponent<SkillController>();
                var platforms = player.gameObject.AddComponent<PlatformController>();
                var binds = (MonoBehaviour[])GetInternal(player, "m_Binds");
                SetInternal(player, "m_Binds", binds.Concat(new MonoBehaviour[] { skills, platforms }).ToArray());
                SetInternal(skills, "m_SkillSpawnPost", player.transform);
                skills.Init(player);
                platforms.Init(player);
                player.HasBind(out StatController stats);
                var cards = new[] { "ParallaxBolt", "AnomalySeeker", "AegisConversion", "AnomalyIncursion" }
                    .Select(id => Load<CardConfig>("Cards/Card_Allain_" + id + ".asset")).ToArray();
                var incursion = skills.GetSkillActivator(Load<SkillConfig>("Skills/Skill_Allain_AnomalyIncursion.asset"));
                var seeker = skills.GetSkillActivator(Load<SkillConfig>("Skills/Skill_Allain_AnomalySeeker.asset"));
                var passive = skills.GetSkillActivator(Load<SkillConfig>("Skills/Skill_Allain_AegisConversion.asset"));
                var bolts = Enumerable.Range(1, 4).Select(stage => skills.GetSkillActivator(
                    Load<SkillConfig>("Skills/Skill_Allain_ParallaxBolt_Stage" + stage + ".asset"))).ToArray();
                CheckInternal(incursion.Progression.Level == 1, "Both perfect sides register Incursion at level one");
                CheckInternal(cards[0].SkillConfigs.Length == 4 && cards.Skip(1).All(card => card.SkillConfigs.Length == 1)
                    && cards.SelectMany(card => card.SkillConfigs).All(field => field.CardPurpose == CardPurpose.SkillUp),
                    "All four cards use native SkillUp, including the platform skill and all four weapon stages");
                passive.HasAbility(passive.SkillConfig.AbilitySets[0].BaseInfo.Id, out AbilityDeliver buff);
                buff.Activate();
                var collapse = Load<AnomalySignConfig>("Status/Status_Allain_AnomalySign.asset").Collapse;
                var explosion = ((AnomalyIncursionConfig)incursion.SkillConfig.AbilitySets[0]).Entity.DeathExplosion;
                var secondary = new[] { collapse, collapse, explosion };
                var sources = new[] { bolts[0], seeker, incursion };
                var pulses = new AbilityDeliver[secondary.Length];
                for (int i = 0; i < pulses.Length; i++)
                {
                    pulses[i] = Instantiate(secondary[i].DeliverPrefab, player.transform, false);
                    pulses[i].Init(secondary[i], sources[i].SkillContext);
                }
                var shotSkills = bolts.Concat(new[] { seeker }).ToArray();
                for (int level = 1; level <= 3; level++)
                {
                    if (level > 1)
                        foreach (var card in cards)
                            foreach (var field in card.SkillConfigs) skills.AddNewSkill(field.SkillConfig);
                    float factor = 1f + .15f * (level - 1);
                    foreach (var shot in shotSkills)
                    {
                        shot.HasAbility(shot.SkillConfig.AbilitySets[0].BaseInfo.Id, out AbilityDeliver deliver);
                        CheckInternal(shot.Progression.Level == level && Mathf.Approximately(
                            AbilityUltility.GetFinalPowerAmount(deliver.AbilityContext), attack * factor),
                            shot.SkillConfig.name + " damage at skill level " + level);
                    }
                    CheckInternal(Mathf.Approximately(stats.LifeStealRate, c_AegisLifeStealPerStack * factor)
                        && stats.GetFinalStat(hero.MainStats.GetFinalStat(1)).Attack == attack,
                        "One active Aegis stack grants only the correct life steal at level " + level);
                    for (int i = 0; i < pulses.Length; i++)
                        CheckInternal(Mathf.Approximately(AbilityUltility.GetFinalPowerAmount(pulses[i].AbilityContext),
                            attack * (i == 2 ? .5f : 1f) * factor), secondary[i].name + " inherits " + sources[i].SkillConfig.name + " level " + level);
                }
            }
            finally { player.gameObject.SetActive(false); }
        }
        private Unit UnitInternal(UnitConfig config, Vector3 position, bool owner)
        {
            var copy = Instantiate(config); m_Temporary.Add(copy);
            var so = new SerializedObject(copy); so.FindProperty("m_RebornConfig").objectReferenceValue = Load<RebornConfig>("Entities/Reborn_AnomalyEntity.asset");
            if (!owner)
            {
                so.FindProperty("m_MainStats.m_BaseStat.m_Health").floatValue = 5000;
                so.FindProperty("m_MainStats.m_BaseStat.m_Defense").floatValue = 0;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            var go = MakeInternal(owner ? "Validation Allain" : "Validation Enemy"); go.transform.position = position; go.layer = owner ? 11 : 20;
            var unit = go.AddComponent<Unit>(); var damage = go.AddComponent<Damageable>(); var status = go.AddComponent<StatusEffectController>(); var stats = go.AddComponent<StatController>();
            SetInternal(unit, "m_Config", copy); SetInternal(unit, "m_Binds", new MonoBehaviour[]{damage,status,stats}); SetInternal(unit, "m_Progression", new ProgressField(1, 100)); SetInternal(stats,"m_ModifierPost",go.transform);
            stats.Init(unit); status.Init(unit); damage.Init(unit); damage.RefreshDamageableStat(1f,true);
            var collider = go.AddComponent<CircleCollider2D>(); collider.radius=.1f; collider.isTrigger=true;
            return unit;
        }
        private GameObject MakeInternal(string name) { var go = new GameObject(name); m_Temporary.Add(go); return go; }
        private T Load<T>(string path) where T:UnityEngine.Object => AssetDatabase.LoadAssetAtPath<T>(c_Folder+path);
        private void CheckInternal(bool condition, string label) { if(!condition) throw new Exception(label); m_Checks.Add(label); }
        private FieldInfo FieldInternal(object target,string name) { for(var t=target.GetType();t!=null;t=t.BaseType){var f=t.GetField(name,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);if(f!=null)return f;}throw new Exception(name); }
        private void SetInternal(object target,string name,object value) => FieldInternal(target,name).SetValue(target,value);
        private object GetInternal(object target,string name) => FieldInternal(target,name).GetValue(target);
        private void OnDeathFixtureInvoke(Damageable damageable,IAbilityContext context) => damageable.OnDeath.Invoke(context);
        private sealed class FixtureSkill : ISkill
        {
            public SkillConfig SkillConfig {get;private set;}
            public ProgressField Progression {get;} = new ProgressField(1, 100);
            public SkillContext SkillContext {get;private set;}
            public UnityEvent<Unit> OnAbilityDelivered {get;} = new UnityEvent<Unit>();
            public FixtureSkill(SkillConfig config, Unit owner) { Init(config,new ModuleContext(owner,owner.gameObject)); }
            public void Init(SkillConfig config,IModuleContext context) {SkillConfig=config;SkillContext=new SkillContext(this,context);}
            public void ForceActivateAll() { }
            public void ForceActivate(AbilityConfig config) { }
        }
    }
}
#endif
