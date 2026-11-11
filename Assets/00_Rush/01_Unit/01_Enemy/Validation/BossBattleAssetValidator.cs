#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Rush.Editor
{
    public sealed class BossBattleAssetValidator : IPreprocessBuildWithReport
    {
        private const string c_RushAssets = "Assets/00_Rush";
        private static readonly string[] s_AnimationSlots =
            { "m_Entrance", "m_Idle", "m_Attack", "m_Death", "m_PerfectAttack", "m_Hurt", "m_Jump", "m_Emote" };

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var errors = new List<string>(FindErrors());
            if (errors.Count > 0) throw new BuildFailedException(string.Join("\n", errors));
        }

        public static IEnumerable<string> FindErrors()
        {
            var folders = new[] { c_RushAssets };
            foreach (string guid in AssetDatabase.FindAssets("t:BossUnitConfig", folders))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var boss = AssetDatabase.LoadAssetAtPath<BossUnitConfig>(path);
                foreach (string error in FindBossErrors(boss)) yield return path + " — " + error;
            }

            foreach (string guid in AssetDatabase.FindAssets("t:EnemyWaveConfig", folders))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var wave = AssetDatabase.LoadAssetAtPath<EnemyWaveConfig>(path);
                if (wave == null || wave.BossWaveField?.BossConfig == null)
                    yield return path + " — missing boss wave reference";
            }
        }

        private static IEnumerable<string> FindBossErrors(BossUnitConfig boss)
        {
            if (boss == null)
            {
                yield return "unreadable boss asset";
                yield break;
            }
            if (boss.UnitPrefab == null) yield return "missing unit prefab";
            if (boss.RebornConfig == null) yield return "missing reborn config";
            if (boss.Skills == null) yield return "missing skill list";
            else foreach (SkillConfig skill in boss.Skills)
            {
                if (skill == null || skill.ActivatorPrefab == null)
                    yield return "missing skill or skill activator";
                else if (skill.AbilitySets != null)
                    foreach (AbilityConfig ability in skill.AbilitySets)
                        if (ability == null || ability.DeliverPrefab == null)
                            yield return skill.name + " has a missing ability or deliver prefab";
            }

            var skeleton = boss.SkeletonDataAsset == null ? null : boss.SkeletonDataAsset.GetSkeletonData(true);
            if (skeleton == null)
            {
                yield return "missing or unreadable Spine skeleton";
                yield break;
            }

            using var serializedBoss = new SerializedObject(boss);
            foreach (string slot in s_AnimationSlots)
            {
                var property = serializedBoss.FindProperty(slot);
                var clip = property.objectReferenceValue as AnimationClipConfig;
                if (clip == null)
                {
                    if (slot == "m_Entrance" || slot == "m_Idle" || slot == "m_Attack" || slot == "m_Death" ||
                        property.objectReferenceEntityIdValue != default) yield return slot + " is missing";
                    continue;
                }
                if (slot == "m_Death" && clip.Loop) yield return "death animation loops";
                var visited = new HashSet<AnimationClipConfig>();
                while (clip != null && visited.Add(clip))
                {
                    if (skeleton.FindAnimation(clip.BaseInfo.Name) == null)
                        yield return slot + " refers to absent animation " + clip.BaseInfo.Name;
                    clip = clip.NextAnimation;
                }
            }
        }
    }
}
#endif
