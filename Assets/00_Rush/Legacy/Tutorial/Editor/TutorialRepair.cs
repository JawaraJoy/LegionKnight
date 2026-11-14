#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using LegionKnight;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Rush.Editor
{
    public sealed class TutorialRepair
    {
        private const string c_Root = "Assets/00_Rush/Legacy/Tutorial";
        private const string c_Config = c_Root + "/Configuration";
        private const string c_Canvas = "Assets/00_Rush/UI/CanvasManager (singleton).prefab";
        private const string c_Manager = "Assets/00_Rush/Prefabs/GameManager.prefab";
        private readonly List<string> m_Report = new();
        private TutorialTargetChannel m_TargetChannel;
        private TutorialFlashChannel m_FlashChannel;
        private TutorialAccessPolicy m_ColossalPolicy;
        private Dictionary<string, TutorStepDefinition> m_Steps;
        public string[] Report => m_Report.ToArray();

        public void Apply()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Stop Play Mode before repairing tutorial assets.");
            EnsureFolderInternal(c_Config);
            m_TargetChannel = EnsureAsset<TutorialTargetChannel>(c_Config + "/Tutorial Targets.asset");
            m_FlashChannel = EnsureAsset<TutorialFlashChannel>(c_Config + "/Tutorial Flashes.asset");
            m_ColossalPolicy = EnsureAsset<TutorialAccessPolicy>(c_Config + "/Colossal Access.asset");
            SetObjectInternal(m_ColossalPolicy, "m_MinimumPlayerLevel", 2);
            m_Steps = AssetDatabase.FindAssets("t:TutorStepDefinition", new[] { c_Root })
                .Select(AssetDatabase.GUIDToAssetPath).ToDictionary(path => path, AssetDatabase.LoadAssetAtPath<TutorStepDefinition>);
            foreach (var pair in m_Steps)
            {
                SetObjectInternal(pair.Value, "m_Id", "step." + AssetDatabase.AssetPathToGUID(pair.Key));
                SetObjectInternal(pair.Value, "m_RequiresTarget", true);
                MigrateRewardInternal(pair.Value);
            }
            ConfigureStepsInternal();
            var definitions = AssetDatabase.FindAssets("t:TutorialDefinition", new[] { c_Root }).Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<TutorialDefinition>).OrderBy(d => AssetDatabase.GetAssetPath(d), StringComparer.Ordinal).ToArray();
            foreach (var definition in definitions)
                if (definition.Steps == null || definition.Steps.Length == 0) SetObjectInternal(definition, "m_IsEnabled", false);
            RepairManagerInternal(definitions);
            RepairCanvasInternal();
            RepairFlashFeedbacksInternal();
            AssetDatabase.SaveAssets();
            m_Report.Add("Configured " + definitions.Length + " catalog entries; empty First Gameplay is an explicitly disabled draft.");
            m_Report.Add("Stable step IDs, semantic outcomes, target channels and current Colossal Lv. 2 policy saved.");
        }

        private TutorStepDefinition Step(string suffix) => m_Steps.Single(p => p.Key.EndsWith(suffix, StringComparison.Ordinal)).Value;
        private void ConfigureStepsInternal()
        {
            foreach (var pair in m_Steps)
            {
                var step = pair.Value;
                string file = System.IO.Path.GetFileName(pair.Key);
                TutorialAction action = TutorialAction.None;
                if (file == "09_Select Lily.asset" || file == "06_Select Kaelis.asset") action = TutorialAction.HeroSelected;
                else if (file == "11_Upgrade.asset") action = TutorialAction.HeroUpgraded;
                else if (file == "07_Use.asset") action = TutorialAction.HeroUsed;
                else if (file == "004_Select Platform.asset") action = TutorialAction.CardSelected;
                else if (file == "005_Equip Platform.asset") action = TutorialAction.CardEquipped;
                else if (file == "03_Click Summon.asset") action = TutorialAction.DrawRequested;
                else if (file == "03b_Confirm Gachar Result.asset") action = TutorialAction.DrawCommitted;
                else if (file == "02_Click Classic.asset" || file == "15_Classic.asset" || file == "10_Classic.asset") action = TutorialAction.StageStarted;
                SetObjectInternal(step, "m_CompletionAction", (int)action);
                SetObjectInternal(step, "m_PreserveSuccess", action == TutorialAction.HeroUpgraded || action == TutorialAction.CardEquipped || action == TutorialAction.DrawCommitted);
                SetObjectInternal(step, "m_SkipCompletedOnResume", file.Contains("Close") || file.Contains("Close loot"));
                if (step.Conversation != null)
                {
                    var so = new SerializedObject(step);
                    var calls = so.FindProperty("m_OnStepStart.m_PersistentCalls.m_Calls");
                    for (int i = calls.arraySize - 1; i >= 0; i--)
                    {
                        var entry = calls.GetArrayElementAtIndex(i);
                        if (entry.FindPropertyRelative("m_Target").objectReferenceValue == step.Conversation &&
                            entry.FindPropertyRelative("m_MethodName").stringValue == "StartConversation") calls.DeleteArrayElementAtIndex(i);
                    }
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }
            var commit = Step("003_Gacha/Step/03b_Confirm Gachar Result.asset");
            var commitSo = new SerializedObject(commit);
            commitSo.FindProperty("m_OnStepStart.m_PersistentCalls.m_Calls").ClearArray();
            commitSo.ApplyModifiedPropertiesWithoutUndo();
            SetObjectInternal(Step("003_Gacha/Step/03_Click Summon.asset"), "m_SuccessDependency", commit);
            SetObjectInternal(Step("003_Gacha/Step/04_Close Gacha Result.asset"), "m_SuccessDependency", null);
            SetObjectInternal(Step("003_Gacha/Step/04_Close Gacha Result.asset"), "m_ResumeSuccessDependency", commit);
        }

        private void MigrateRewardInternal(TutorStepDefinition step)
        {
            var so = new SerializedObject(step);
            var calls = so.FindProperty("m_OnStepStart.m_PersistentCalls.m_Calls");
            for (int i = calls.arraySize - 1; i >= 0; i--)
            {
                var entry = calls.GetArrayElementAtIndex(i);
                var reward = entry.FindPropertyRelative("m_Target").objectReferenceValue as LootChestDefinition;
                string method = entry.FindPropertyRelative("m_MethodName").stringValue;
                if (reward == null || (method != "DirectTakeLootsShow" && method != "DirectTakeLoots")) continue;
                so.FindProperty("m_Reward").objectReferenceValue = reward;
                calls.DeleteArrayElementAtIndex(i);
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(step);
        }

        private void RepairManagerInternal(TutorialDefinition[] definitions)
        {
            var root = PrefabUtility.LoadPrefabContents(c_Manager);
            try
            {
                var manager = root.GetComponentInChildren<TutorialManager>(true);
                var so = new SerializedObject(manager);
                var entries = so.FindProperty("m_Contents");
                var existing = new HashSet<TutorialDefinition>();
                for (int i = 0; i < entries.arraySize; i++) existing.Add(entries.GetArrayElementAtIndex(i).FindPropertyRelative("m_Definition").objectReferenceValue as TutorialDefinition);
                foreach (var definition in definitions)
                {
                    if (existing.Contains(definition)) continue;
                    int index = entries.arraySize++;
                    var entry = entries.GetArrayElementAtIndex(index);
                    entry.FindPropertyRelative("m_Definition").objectReferenceValue = definition;
                    entry.FindPropertyRelative("m_IsDone").boolValue = false;
                    entry.FindPropertyRelative("m_IsUnlocked").boolValue = false;
                    entry.FindPropertyRelative("m_OnTutorialStart.m_PersistentCalls.m_Calls").ClearArray();
                    entry.FindPropertyRelative("m_OnTutorialEnd.m_PersistentCalls.m_Calls").ClearArray();
                }
                // Preserve the pre-existing tutorial dependency inside the same durable transaction.
                for (int i = 0; i < entries.arraySize; i++)
                {
                    var entry = entries.GetArrayElementAtIndex(i);
                    var definition = entry.FindPropertyRelative("m_Definition").objectReferenceValue as TutorialDefinition;
                    if (definition == null) continue;
                    var definitionSo = new SerializedObject(definition);
                    var unlocks = definitionSo.FindProperty("m_UnlockOnCompletion");
                    var calls = entry.FindPropertyRelative("m_OnTutorialEnd.m_PersistentCalls.m_Calls");
                    for (int j = calls.arraySize - 1; j >= 0; j--)
                    {
                        var call = calls.GetArrayElementAtIndex(j);
                        var next = call.FindPropertyRelative("m_Target").objectReferenceValue as TutorialDefinition;
                        if (next == null || call.FindPropertyRelative("m_MethodName").stringValue != "SetIsUnlocked" ||
                            !call.FindPropertyRelative("m_Arguments.m_BoolArgument").boolValue) continue;
                        bool found = false;
                        for (int k = 0; k < unlocks.arraySize; k++) found |= unlocks.GetArrayElementAtIndex(k).objectReferenceValue == next;
                        if (!found) unlocks.GetArrayElementAtIndex(unlocks.arraySize++).objectReferenceValue = next;
                        calls.DeleteArrayElementAtIndex(j);
                    }
                    definitionSo.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(definition);
                }
                so.FindProperty("m_TargetChannel").objectReferenceValue = m_TargetChannel;
                so.FindProperty("m_TutorTargets").ClearArray();
                var bridge = manager.GetComponent<TutorialRuntimeBridge>() ?? manager.gameObject.AddComponent<TutorialRuntimeBridge>();
                so.FindProperty("m_RuntimeBridge").objectReferenceValue = bridge;
                so.ApplyModifiedPropertiesWithoutUndo();
                foreach (var flash in root.GetComponentsInChildren<TutorFlashHandler>(true))
                {
                    SetObjectInternal(flash, "m_Channel", m_FlashChannel);
                    var flashSo = new SerializedObject(flash);
                    flashSo.FindProperty("m_FlashList").ClearArray();
                    flashSo.ApplyModifiedPropertiesWithoutUndo();
                }
                PrefabUtility.SaveAsPrefabAsset(root, c_Manager);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }

        private void RepairCanvasInternal()
        {
            string heroPrefab = null;
            string cardPrefab = null;
            var root = PrefabUtility.LoadPrefabContents(c_Canvas);
            try
            {
                foreach (var target in root.GetComponentsInChildren<TutorTarget>(true)) SetObjectInternal(target, "m_Channel", m_TargetChannel);
                foreach (var flash in root.GetComponentsInChildren<TutorFlash>(true)) SetObjectInternal(flash, "m_Channel", m_FlashChannel);
                var preparation = root.GetComponentInChildren<PreparationPanel>(true);
                heroPrefab = ReferencedPrefab(preparation.HeroTabView, "m_HeroSelectViewAsset");
                cardPrefab = ReferencedPrefab(preparation.CardTabView, "m_CardSelectViewAsset");
                RepairHeroRowsInternal(root);
                RepairCardRowsInternal(root);
                var single = Step("003_Gacha/Step/03_Click Summon.asset");
                foreach (var target in root.GetComponentsInChildren<TutorTarget>(true))
                    if (target.Definition == single && target.transform.parent != null && target.transform.parent.name.Contains("Draw 10")) UnityEngine.Object.DestroyImmediate(target);
                // Match the single-draw component itself too, rather than depending on its parent name.
                var draws = root.GetComponentsInChildren<TutorTarget>(true).Where(t => t.Definition == single).ToArray();
                foreach (var target in draws)
                    if (target.gameObject.name.Contains("Draw 10")) UnityEngine.Object.DestroyImmediate(target);
                var confirm = root.GetComponentInChildren<CurrencyConfirmationPanel>(true);
                var confirmButton = ObjectField<Button>(confirm, "m_ConfirmButton");
                BindTargetInternal(confirmButton.gameObject, Step("003_Gacha/Step/03b_Confirm Gachar Result.asset"), confirmButton);
                var result = root.GetComponentInChildren<GachaResultPanel>(true);
                var close = ObjectField<Button>(result, "m_CloseButton");
                var closeTarget = BindTargetInternal(close.gameObject, Step("003_Gacha/Step/04_Close Gacha Result.asset"), close);
                SetObjectInternal(closeTarget, "m_CompletionView", result);
                SetObjectInternal(closeTarget, "m_CompletionViewVisible", false);
                var details = preparation.CardTabView.CardDetailView;
                var equip = ObjectField<Button>(details, "m_AddOrRemoveToDeckButton");
                BindTargetInternal(equip.gameObject, Step("004_Equip Card/Step/005_Equip Platform.asset"), equip);
                RepairNavigationInternal(root, preparation);
                foreach (var gate in root.GetComponentsInChildren<ProgressReach>(true))
                {
                    var button = gate.GetComponentInParent<Button>();
                    if (button == null && gate.transform.parent != null) button = gate.transform.parent.GetComponentInChildren<Button>(true);
                    if (button != null && !button.name.Contains("Adventure")) SetObjectInternal(gate, "m_Control", button);
                    if (button != null && button.name.Contains("Colossal")) SetObjectInternal(gate, "m_AccessPolicy", m_ColossalPolicy);
                }
                foreach (var selection in root.GetComponentsInChildren<PlayModeSelection>(true)) SetObjectInternal(selection, "m_ColossalAccess", m_ColossalPolicy);
                RepairResumeInternal(root);
                PrefabUtility.SaveAsPrefabAsset(root, c_Canvas);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            RepairRowPrefabInternal(heroPrefab, true);
            RepairRowPrefabInternal(cardPrefab, false);
            m_Report.Add("Rebound confirmation, summary, hero roster and platform inventory targets; Draw 10 no longer shares the single-draw step.");
        }

        private void RepairHeroRowsInternal(GameObject root)
        {
            foreach (var row in root.GetComponentsInChildren<HeroSelectView>(true))
            {
                var button = ObjectField<Button>(row, "m_SelectButton");
                foreach (string suffix in new[] { "002_Upgrade Character/Step/09_Select Lily.asset", "003_Gacha/Step/06_Select Kaelis.asset" })
                {
                    var target = BindTargetInternal(row.gameObject, Step(suffix), button);
                    SetObjectInternal(target, "m_HeroSource", row);
                }
            }
        }
        private void RepairCardRowsInternal(GameObject root)
        {
            foreach (var row in root.GetComponentsInChildren<CardSelectView>(true))
            {
                var target = BindTargetInternal(row.gameObject, Step("004_Equip Card/Step/004_Select Platform.asset"), ObjectField<Button>(row, "m_SelectButton"));
                SetObjectInternal(target, "m_CardSource", row);
            }
        }
        private void RepairRowPrefabInternal(string path, bool hero)
        {
            if (string.IsNullOrEmpty(path)) throw new InvalidOperationException("A tutorial inventory row prefab is not assigned.");
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (hero) RepairHeroRowsInternal(root); else RepairCardRowsInternal(root);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        private void RepairNavigationInternal(GameObject root, PreparationPanel preparation)
        {
            var home = root.GetComponentInChildren<HomePanel>(true);
            var level = root.GetComponentInChildren<LevelPanel>(true);
            var loot = root.GetComponentInChildren<LootedPanel>(true);
            var summon = root.GetComponentInChildren<GachaPanel>(true);
            var upgrade = root.GetComponentInChildren<UpgradeView>(true);
            foreach (var target in root.GetComponentsInChildren<TutorTarget>(true))
            {
                if (target.Definition == null) continue;
                string file = System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(target.Definition));
                View destination = null;
                bool visible = true;
                if (file.Contains("OpenCharPanel") || file.Contains("Open Char Panel") || file.Contains("Open Peparation")) destination = preparation;
                else if (file.Contains("Click Play") || file == "14_Play.asset" || file == "09_Play.asset") destination = level;
                else if (file == "01_Open Summon.asset") destination = summon;
                else if (file == "10_ClickUpgrade.asset") destination = upgrade;
                else if (file == "12_Close Upgrade.asset") { destination = upgrade; visible = false; }
                else if (file == "13_Home.asset" || file == "08_Go home.asset") destination = home;
                else if (file.Contains("CloseReward") || file == "02_Close Rewards.asset" || file == "001_Close loot panel.asset") { destination = loot; visible = false; }
                if (destination == null) continue;
                SetObjectInternal(target, "m_CompletionView", destination);
                SetObjectInternal(target, "m_CompletionViewVisible", visible);
            }
            // Restore the reward promised by the existing Summon lesson, guarded by the v2 grant ledger.
            var player = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/00_Rush/00_Player/Prefabs/Rush Player.prefab");
            var gacha = player.GetComponentInChildren<GachaManager>(true);
            var banner = gacha.Banners.FirstOrDefault(b => b != null);
            if (banner != null)
            {
                var ticket = EnsureAsset<LootChestDefinition>(c_Config + "/Tutorial Summon Reward.asset");
                var reward = new SerializedObject(ticket);
                var fields = reward.FindProperty("m_LootFields");
                fields.arraySize = 1;
                var item = fields.GetArrayElementAtIndex(0);
                item.FindPropertyRelative("m_CollectibleConfig").objectReferenceValue = banner.AltCostCurrency != null ? banner.AltCostCurrency : banner.DrawCostCurrency;
                item.FindPropertyRelative("m_Amount").intValue = banner.AltCostCurrency != null ? banner.AltSingleDrawCost : banner.SingleDrawCost;
                item.FindPropertyRelative("m_Chance").floatValue = 1f;
                reward.ApplyModifiedPropertiesWithoutUndo();
                SetObjectInternal(Step("003_Gacha/Step/02_Close Rewards.asset"), "m_Reward", ticket);
            }
        }
        private void RepairResumeInternal(GameObject root)
        {
            var panel = root.GetComponentInChildren<TutorialPanel>(true);
            ConfigureFeedbacksInternal(panel.gameObject);
            var skip = ObjectField<Button>(panel, "m_SkipButton");
            for (int i = skip.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
                if (skip.onClick.GetPersistentTarget(i) == panel && skip.onClick.GetPersistentMethodName(i) == "EndDialogue") UnityEventTools.RemovePersistentListener(skip.onClick, i);
            var label = skip.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.text = "Later";
            var home = root.GetComponentInChildren<HomePanel>(true);
            var parent = home.Content.transform;
            var existing = parent.Find("Tutorial Resume") ?? home.transform.Find("Tutorial Resume");
            var resume = existing != null ? existing.GetComponent<Button>() : UnityEngine.Object.Instantiate(skip, parent);
            resume.transform.SetParent(parent, false);
            resume.name = "Tutorial Resume";
            var rect = (RectTransform)resume.transform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0f, -480f);
            rect.sizeDelta = new Vector2(450f, 80f);
            var text = resume.GetComponentInChildren<TMP_Text>(true);
            if (text != null) text.text = "Continue Tutorial";
            var so = new SerializedObject(resume);
            so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls").ClearArray();
            so.ApplyModifiedPropertiesWithoutUndo();
            var agent = root.GetComponentsInChildren<TutorialAgent>(true).First(a => a.gameObject.name == "Tutorial agent");
            SetObjectInternal(agent, "m_ResumeButton", resume);
            resume.gameObject.SetActive(false);
        }
        private void RepairFlashFeedbacksInternal()
        {
            var paths = new HashSet<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:TutorFlashDefinition", new[] { c_Root }))
            {
                var definition = AssetDatabase.LoadAssetAtPath<TutorFlashDefinition>(AssetDatabase.GUIDToAssetPath(guid));
                if (definition.FlashUIAsset == null) continue;
                string path = AssetDatabase.GUIDToAssetPath(definition.FlashUIAsset.AssetGUID);
                if (string.IsNullOrEmpty(path) || !paths.Add(path)) continue;
                var root = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    if (root.GetComponentsInChildren<MoreMountains.Feedbacks.MMF_Player>(true).Length == 0) continue;
                    ConfigureFeedbacksInternal(root);
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }
        }
        private void ConfigureFeedbacksInternal(GameObject owner)
        {
            foreach (var player in owner.GetComponentsInChildren<MoreMountains.Feedbacks.MMF_Player>(true))
            {
                var so = new SerializedObject(player);
                var force = so.FindProperty("ForceTimescaleMode");
                if (force != null) force.boolValue = true;
                foreach (string name in new[] { "ForcedTimescaleMode", "PlayerTimescaleMode" })
                {
                    var mode = so.FindProperty(name);
                    if (mode == null) continue;
                    int unscaled = Array.IndexOf(mode.enumNames, "Unscaled");
                    if (unscaled >= 0) mode.enumValueIndex = unscaled;
                }
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        private TutorTarget BindTargetInternal(GameObject owner, TutorStepDefinition step, Button button)
        {
            var target = owner.GetComponents<TutorTarget>().FirstOrDefault(t => t.Definition == step) ?? owner.AddComponent<TutorTarget>();
            SetObjectInternal(target, "m_Definition", step);
            SetObjectInternal(target, "m_NextButton", button);
            SetObjectInternal(target, "m_Channel", m_TargetChannel);
            return target;
        }
        private static T ObjectField<T>(UnityEngine.Object owner, string field) where T : UnityEngine.Object =>
            new SerializedObject(owner).FindProperty(field).objectReferenceValue as T;
        private static string ReferencedPrefab(UnityEngine.Object owner, string field) =>
            AssetDatabase.GUIDToAssetPath(new SerializedObject(owner).FindProperty(field).FindPropertyRelative("m_AssetGUID").stringValue);
        private T EnsureAsset<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
        private void EnsureFolderInternal(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolderInternal(parent);
            AssetDatabase.CreateFolder(parent, System.IO.Path.GetFileName(path));
        }
        private void SetObjectInternal(UnityEngine.Object target, string field, object value)
        {
            var so = new SerializedObject(target);
            var property = so.FindProperty(field) ?? throw new InvalidOperationException(target.name + " lacks field " + field);
            if (property.propertyType == SerializedPropertyType.ObjectReference) property.objectReferenceValue = value as UnityEngine.Object;
            else if (value is bool boolean) property.boolValue = boolean;
            else if (value is int number) property.intValue = number;
            else if (value is string text) property.stringValue = text;
            else throw new InvalidOperationException("Unsupported tutorial field: " + field);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }
    }
}
#endif
