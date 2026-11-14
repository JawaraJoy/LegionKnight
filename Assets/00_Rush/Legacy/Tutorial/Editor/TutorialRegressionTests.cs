#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using LegionKnight;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Rush.Tests
{
    public sealed class TutorialRegressionTests
    {
        private readonly List<Object> m_Objects = new();
        private TestStore m_Store;

        private sealed class TestStore : ITutorialProgressStore
        {
            private Dictionary<string, string> m_Data = new();
            public string FailKey;
            public int Writes;
            public bool Contains(string key) => m_Data.ContainsKey(key);
            public T Read<T>(string key) => typeof(T) == typeof(bool)
                ? (T)(object)bool.Parse(m_Data[key]) : JsonUtility.FromJson<T>(m_Data[key]);
            public void Write<T>(string key, T value)
            {
                if (key == FailKey) throw new InvalidOperationException("Simulated write failure.");
                Writes++;
                m_Data[key] = value is bool boolean ? boolean.ToString() : JsonUtility.ToJson(value);
            }
            public void Transact(Action changes)
            {
                var before = new Dictionary<string, string>(m_Data);
                try { changes(); }
                catch { m_Data = before; throw; }
            }
        }
        [SetUp] public void SetUp() => m_Store = new TestStore();
        [TearDown] public void TearDown()
        {
            for (int i = m_Objects.Count - 1; i >= 0; i--) if (m_Objects[i] != null) Object.DestroyImmediate(m_Objects[i]);
            m_Objects.Clear();
        }
        private T Asset<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            m_Objects.Add(asset);
            return asset;
        }
        private TutorStepDefinition Step(TutorialAction action = TutorialAction.None, bool target = false)
        {
            var step = Asset<TutorStepDefinition>();
            Set(step, "m_Id", Guid.NewGuid().ToString("N"));
            Set(step, "m_RequiresTarget", target);
            Set(step, "m_CompletionAction", (int)action);
            return step;
        }
        private TutorialDefinition Definition(params TutorStepDefinition[] steps)
        {
            var definition = Asset<TutorialDefinition>();
            Set(definition, "m_Id", Guid.NewGuid().ToString("N"));
            Set(definition, "m_UnlockedAtFirst", true);
            var so = new SerializedObject(definition);
            var entries = so.FindProperty("m_Steps");
            entries.arraySize = steps.Length;
            for (int i = 0; i < steps.Length; i++) entries.GetArrayElementAtIndex(i).objectReferenceValue = steps[i];
            so.ApplyModifiedPropertiesWithoutUndo();
            return definition;
        }
        private TutorialHandler Handler(params TutorialDefinition[] definitions)
        {
            var go = new GameObject("Isolated tutorial test");
            m_Objects.Add(go);
            go.SetActive(false);
            var handler = go.AddComponent<TutorialHandler>();
            var contents = new TutorialContent[definitions.Length];
            for (int i = 0; i < definitions.Length; i++) contents[i] = new TutorialContent(definitions[i]);
            Field(handler, "m_Contents", contents);
            handler.Initialize(m_Store);
            go.SetActive(true);
            return handler;
        }
        private TutorTarget Target(TutorStepDefinition step, TutorialHandler handler)
        {
            var go = new GameObject("Isolated tutorial button", typeof(RectTransform), typeof(Button));
            m_Objects.Add(go);
            var target = go.AddComponent<TutorTarget>();
            Set(target, "m_Definition", step);
            Set(target, "m_NextButton", go.GetComponent<Button>());
            handler.AddTarget(target);
            return target;
        }
        private TutorialContent Content(TutorialDefinition definition)
        {
            var content = new TutorialContent(definition);
            content.Initialize(m_Store);
            return content;
        }
        private void Tick(TutorialHandler handler, int count = 1)
        {
            var update = typeof(TutorialHandler).GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic);
            for (int i = 0; i < count; i++) update.Invoke(handler, null);
        }
        private void Field(object instance, string name, object value) => instance.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
        private void Set(Object owner, string name, object value)
        {
            var so = new SerializedObject(owner);
            var property = so.FindProperty(name);
            if (property.propertyType == SerializedPropertyType.ObjectReference) property.objectReferenceValue = value as Object;
            else if (value is string text) property.stringValue = text;
            else if (value is bool boolean) property.boolValue = boolean;
            else if (value is int number) property.intValue = number;
            else if (value is float fraction) property.floatValue = fraction;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        [Test] public void SuspendedLessonIsIgnoredForEverySavedEntryPoint()
        {
            var lesson = Definition(Step(TutorialAction.CardSelected));
            var previous = Content(lesson);
            previous.SetCheckpoint(lesson.Steps[0].Id);
            var ids = new[] { lesson.Id };
            m_Store.Write("tutorial.v2.queue", new TutorialQueueRecord(ids, ids));
            Set(lesson, "m_IsEnabled", false);
            var handler = Handler(lesson);
            handler.StartTutorial(lesson);
            handler.ResumeTutorial();
            Tick(handler, 3);
            var queue = m_Store.Read<TutorialQueueRecord>("tutorial.v2.queue");
            Assert.That(queue.Ids, Is.Empty);
            Assert.That(queue.DeferredIds, Is.Empty);
            Assert.That(handler.CurrentTutorial, Is.Null);
            Assert.That(Content(lesson).InProgress, Is.True);
            Assert.That(Content(lesson).IsDone, Is.False);
        }

        [Test] public void SavedUpgradeFinishesShortenedLessonWithoutRepeatingNavigationOrPayment()
        {
            var navigation = Step(target: true);
            var purchase = Step(TutorialAction.HeroUpgraded);
            Set(purchase, "m_PreserveSuccess", true);
            var lesson = Definition(navigation, purchase);
            var saved = Content(lesson);
            saved.SetCheckpoint("former-return-home-step");
            saved.AddCompletedStep(purchase.Id);
            m_Store.Write("tutorial.v2.queue", new TutorialQueueRecord(Array.Empty<string>(), new[] { lesson.Id }));
            var handler = Handler(lesson);
            Tick(handler, 2);
            Assert.That(Content(lesson).IsDone, Is.True);
            Assert.That(handler.HasDeferredTutorial, Is.False);
            Assert.That(handler.CurrentTutorial, Is.Null);
        }

        [Test] public void UnlockSurvivesReloadWithoutMarkingCompleted()
        {
            var definition = Definition(Step(TutorialAction.HeroSelected));
            Set(definition, "m_UnlockedAtFirst", false);
            var content = Content(definition);
            content.SetIsUnlocked(true);
            var restored = Content(definition);
            Assert.That(restored.IsUnlocked, Is.True);
            Assert.That(restored.IsDone, Is.False);
            Assert.That(m_Store.Read<bool>(definition.Id + "unlock"), Is.True);
            Assert.That(m_Store.Read<bool>(definition.Id + "isdone"), Is.False);
        }
        [Test] public void AmbiguousLegacyFlagPreservesAccessWithoutInventingCompletion()
        {
            var definition = Definition(Step(TutorialAction.HeroSelected));
            Set(definition, "m_UnlockedAtFirst", false);
            m_Store.Write(definition.Id + "isdone", true);
            var content = Content(definition);
            Assert.That(content.IsUnlocked, Is.True);
            Assert.That(content.IsDone, Is.False);
            Assert.That(content.LegacyCompletionUnverified, Is.True);
        }
        [Test] public void CheckpointAndSuccessfulActionsSurviveReload()
        {
            var step = Step(TutorialAction.HeroUpgraded);
            var definition = Definition(step);
            var content = Content(definition);
            content.SetCheckpoint(step.Id);
            content.AddCompletedStep(step.Id);
            var restored = Content(definition);
            Assert.That(restored.CheckpointStepId, Is.EqualTo(step.Id));
            Assert.That(restored.HasCompletedStep(step.Id), Is.True);
            Assert.That(restored.InProgress, Is.True);
        }
        [Test] public void RewardIsGrantedOnlyOnceAcrossReload()
        {
            var definition = Definition(Step(TutorialAction.HeroSelected));
            int grants = 0;
            Content(definition).GrantRewardOnce("grant", () => grants++);
            Content(definition).GrantRewardOnce("grant", () => grants++);
            Assert.That(grants, Is.EqualTo(1));
        }
        [Test] public void RewardAndLedgerRollBackTogetherOnWriteFailure()
        {
            var definition = Definition(Step(TutorialAction.HeroSelected));
            var content = Content(definition);
            m_Store.FailKey = "tutorial.v2." + definition.Id;
            Assert.Throws<InvalidOperationException>(() => content.GrantRewardOnce("grant", () => m_Store.Write("testReward", true)));
            Assert.That(m_Store.Contains("testReward"), Is.False);
            Assert.That(content.HasGrantedReward("grant"), Is.False);
            m_Store.FailKey = null;
            content.GrantRewardOnce("grant", () => m_Store.Write("testReward", true));
            Assert.That(content.HasGrantedReward("grant"), Is.True);
        }
        [Test] public void QueueDeduplicatesAndStartsNextAfterCompletion()
        {
            var first = Definition(Step(TutorialAction.HeroSelected));
            var second = Definition(Step(TutorialAction.CardSelected));
            var handler = Handler(first, second);
            handler.StartTutorial(first);
            handler.StartTutorial(second);
            handler.StartTutorial(second);
            Tick(handler);
            Assert.That(handler.CurrentTutorial, Is.SameAs(first));
            handler.NotifyAction(TutorialAction.HeroSelected, string.Empty);
            Tick(handler, 2);
            Assert.That(handler.CurrentTutorial, Is.SameAs(second));
        }
        [Test] public void EndCallbackCanQueueAnotherTutorial()
        {
            var first = Definition(Step(TutorialAction.HeroSelected));
            var second = Definition(Step(TutorialAction.CardSelected));
            var handler = Handler(first, second);
            handler.HasContent(first, out var content);
            content.OnTutorialEnd.AddListener(_ => handler.StartTutorial(second));
            handler.StartTutorial(first);
            Tick(handler);
            handler.NotifyAction(TutorialAction.HeroSelected, string.Empty);
            Tick(handler, 2);
            Assert.That(handler.CurrentTutorial, Is.SameAs(second));
        }
        [Test] public void OldButtonsAndDoubleClicksCannotSkipSteps()
        {
            var first = Step(target: true);
            var second = Step(target: true);
            var definition = Definition(first, second);
            var handler = Handler(definition);
            var old = Target(first, handler);
            var current = Target(second, handler);
            handler.StartTutorial(definition);
            Tick(handler, 2);
            old.NextButton.onClick.Invoke();
            Tick(handler, 3);
            Assert.That(handler.CurrentStepIndex, Is.EqualTo(1));
            old.NextButton.onClick.Invoke();
            Tick(handler);
            Assert.That(handler.CurrentTutorial, Is.SameAs(definition));
            current.NextButton.onClick.Invoke();
            current.NextButton.onClick.Invoke();
            Tick(handler, 2);
            Assert.That(Content(definition).IsDone, Is.True);
        }
        [Test] public void TransactionStepIgnoresClicksAndUnrelatedSignals()
        {
            var step = Step(TutorialAction.HeroUpgraded, true);
            var definition = Definition(step);
            var handler = Handler(definition);
            var target = Target(step, handler);
            handler.StartTutorial(definition);
            Tick(handler, 2);
            target.NextButton.onClick.Invoke();
            handler.NotifyAction(TutorialAction.CardSelected, string.Empty);
            Tick(handler, 2);
            Assert.That(handler.CurrentTutorial, Is.SameAs(definition));
            handler.NotifyAction(TutorialAction.HeroUpgraded, string.Empty);
            Tick(handler);
            Assert.That(Content(definition).IsDone, Is.True);
        }
        [Test] public void MissingTargetDefersWithoutInventingCompletion()
        {
            var step = Step(target: true);
            var definition = Definition(step);
            var handler = Handler(definition);
            handler.StartTutorial(definition);
            Tick(handler, 2);
            Field(handler, "m_TargetWaitStarted", Time.realtimeSinceStartup - 100f);
            Tick(handler);
            Assert.That(handler.CurrentTutorial, Is.Null);
            Assert.That(handler.HasDeferredTutorial, Is.True);
            Assert.That(Content(definition).IsDone, Is.False);
            Target(step, handler);
            handler.ResumeTutorial();
            Tick(handler, 2);
            Assert.That(handler.CurrentTutorial, Is.SameAs(definition));
        }
        [Test] public void EmptyAndNullStepDefinitionsAreRejectedSafely()
        {
            var empty = Definition();
            var missing = Definition((TutorStepDefinition)null);
            Assert.That(empty.ValidationError, Is.Not.Empty);
            Assert.That(missing.ValidationError, Is.Not.Empty);
            var handler = Handler(empty, missing);
            handler.StartTutorial(empty);
            handler.StartTutorial(missing);
            Tick(handler);
            Assert.That(handler.CurrentTutorial, Is.Null);
        }
        [Test] public void DuplicateStepIdsAreRejected()
        {
            var first = Step(TutorialAction.HeroSelected);
            var second = Step(TutorialAction.HeroUsed);
            Set(second, "m_Id", first.Id);
            Assert.That(Definition(first, second).ValidationError, Does.Contain("Duplicate"));
        }
        [Test] public void NavigationWaitsForDestinationToActuallyOpen()
        {
            var step = Step(target: true);
            var definition = Definition(step);
            var handler = Handler(definition);
            var target = Target(step, handler);
            var go = new GameObject("Isolated destination");
            m_Objects.Add(go);
            var view = go.AddComponent<View>();
            var content = new GameObject("Destination content");
            content.transform.SetParent(go.transform);
            content.SetActive(false);
            Set(view, "m_Content", content);
            Set(target, "m_CompletionView", view);
            handler.StartTutorial(definition);
            Tick(handler, 2);
            target.NextButton.onClick.Invoke();
            Tick(handler, 2);
            Assert.That(Content(definition).IsDone, Is.False);
            content.SetActive(true);
            Tick(handler, 2);
            Assert.That(Content(definition).IsDone, Is.True);
        }
        [Test] public void DeferredTutorialRemainsDeferredAfterRestart()
        {
            var definition = Definition(Step(TutorialAction.HeroSelected));
            var first = Handler(definition);
            first.StartTutorial(definition);
            Tick(first);
            first.PauseTutorial();
            var second = Handler(definition);
            Tick(second);
            Assert.That(second.CurrentTutorial, Is.Null);
            Assert.That(second.HasDeferredTutorial, Is.True);
            second.ResumeTutorial();
            Tick(second);
            Assert.That(second.CurrentTutorial, Is.SameAs(definition));
        }
        [Test] public void PreviouslyCommittedDrawDoesNotRequireAnotherPaymentOnResume()
        {
            var request = Step(TutorialAction.DrawRequested);
            var commit = Step(TutorialAction.DrawCommitted);
            Set(commit, "m_PreserveSuccess", true);
            Set(request, "m_SuccessDependency", commit);
            var definition = Definition(request, commit);
            var saved = Content(definition);
            saved.AddCompletedStep(commit.Id);
            saved.SetCheckpoint(commit.Id);
            var handler = Handler(definition);
            Tick(handler, 4);
            Assert.That(Content(definition).IsDone, Is.True);
        }
        [Test] public void SuccessfulDrawIsSavedBeforeNextFrame()
        {
            var request = Step(TutorialAction.DrawRequested);
            var commit = Step(TutorialAction.DrawCommitted);
            Set(commit, "m_PreserveSuccess", true);
            var definition = Definition(request, commit);
            var handler = Handler(definition);
            handler.StartTutorial(definition);
            Tick(handler);
            handler.NotifyAction(TutorialAction.DrawCommitted, string.Empty);
            Assert.That(Content(definition).HasCompletedStep(commit.Id), Is.True);
        }
        [Test] public void IdleHandlerDoesNotWriteSaveEveryFrame()
        {
            var handler = Handler(Definition(Step(TutorialAction.HeroSelected)));
            int before = m_Store.Writes;
            Tick(handler, 100);
            Assert.That(m_Store.Writes, Is.EqualTo(before));
        }
        [Test] public void AccessPolicyPreservesExistingLevelAndUsesOneDecision()
        {
            Assert.That(TutorialAccessPolicy.LevelIsAllowed(1, 2, false, true), Is.False);
            Assert.That(TutorialAccessPolicy.LevelIsAllowed(2, 2, false, true), Is.True);
            Assert.That(TutorialAccessPolicy.LevelIsAllowed(10, 2, false, false), Is.False);
            Assert.That(TutorialAccessPolicy.LevelIsAllowed(1, 2, true, false), Is.True);
        }
        [Test] public void SkipCompatibilityEntrypointExists()
        {
            Assert.That(typeof(TutorialPanel).GetMethod("EndDialogue"), Is.Not.Null);
        }

        [Test] public void DrawFinishingAfterPauseIsNotLost()
        {
            var request = Step(TutorialAction.DrawRequested);
            var commit = Step(TutorialAction.DrawCommitted);
            Set(commit, "m_PreserveSuccess", true);
            Set(request, "m_SuccessDependency", commit);
            var definition = Definition(request, commit);
            var handler = Handler(definition);
            handler.StartTutorial(definition);
            Tick(handler);
            handler.NotifyAction(TutorialAction.DrawRequested, string.Empty);
            Tick(handler);
            handler.PauseTutorial();
            handler.NotifyAction(TutorialAction.DrawCommitted, string.Empty);
            Assert.That(Content(definition).HasCompletedStep(commit.Id), Is.True);
            handler.ResumeTutorial();
            Tick(handler, 4);
            Assert.That(Content(definition).IsDone, Is.True);
        }
        [Test] public void CompletionTransactionFailureRestoresActiveTutorial()
        {
            var definition = Definition(Step(TutorialAction.HeroSelected));
            var handler = Handler(definition);
            handler.StartTutorial(definition);
            Tick(handler);
            handler.NotifyAction(TutorialAction.HeroSelected, string.Empty);
            m_Store.FailKey = "tutorial.v2.queue";
            Assert.Throws<TargetInvocationException>(() => Tick(handler));
            Assert.That(handler.CurrentTutorial, Is.SameAs(definition));
            Assert.That(Content(definition).IsDone, Is.False);
            m_Store.FailKey = null;
        }
        [Test] public void CallbackFailureDoesNotUndoDurableCompletion()
        {
            var definition = Definition(Step(TutorialAction.HeroSelected));
            var handler = Handler(definition);
            handler.HasContent(definition, out var content);
            content.OnTutorialEnd.AddListener(_ => throw new InvalidOperationException("Simulated subscriber failure."));
            handler.StartTutorial(definition);
            Tick(handler);
            handler.NotifyAction(TutorialAction.HeroSelected, string.Empty);
            Assert.Throws<TargetInvocationException>(() => Tick(handler));
            Assert.That(handler.CurrentTutorial, Is.Null);
            Assert.That(Content(definition).IsDone, Is.True);
        }

        [Test] public void FlashRestoresReadStateWithoutWritingDuringInitialization()
        {
            var definition = Asset<TutorFlashDefinition>();
            Set(definition, "m_Id", "isolated-flash");
            var first = new FlashContent(definition);
            first.Initialize(m_Store);
            first.SetHasDone(true);
            int before = m_Store.Writes;
            var restored = new FlashContent(definition);
            restored.Initialize(m_Store);
            Assert.That(restored.HasDone, Is.True);
            Assert.That(restored.HasPlaying, Is.False);
            Assert.That(m_Store.Writes, Is.EqualTo(before));
        }
        [Test] public void MovingAndResizingMaskUpdatesEvenAtZeroTimescale()
        {
            float previousScale = Time.timeScale;
            InvertMaskingAuto mask = null;
            try
            {
                Time.timeScale = 0f;
                var canvasGo = new GameObject("Isolated mask canvas", typeof(RectTransform), typeof(Canvas));
                m_Objects.Add(canvasGo);
                var canvas = canvasGo.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var maskGo = new GameObject("Isolated mask", typeof(RectTransform), typeof(Image));
                maskGo.transform.SetParent(canvasGo.transform, false);
                var maskRect = maskGo.GetComponent<RectTransform>();
                maskRect.sizeDelta = new Vector2(1000f, 2000f);
                mask = maskGo.AddComponent<InvertMaskingAuto>();
                // Ordinary MonoBehaviour lifecycle is not dispatched by these EditMode fixtures.
                typeof(InvertMaskingAuto).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(mask, null);
                var targetGo = new GameObject("Isolated mask target", typeof(RectTransform));
                targetGo.transform.SetParent(maskGo.transform, false);
                var targetRect = targetGo.GetComponent<RectTransform>();
                targetRect.sizeDelta = new Vector2(200f, 100f);
                var target = targetGo.AddComponent<TutorTarget>();
                mask.SetMaskingTarget(target);
                Canvas.ForceUpdateCanvases();
                var material = maskGo.GetComponent<Image>().material;
                Vector4 before = material.GetVector("_HoleRect");
                targetRect.anchoredPosition = new Vector2(150f, 220f);
                targetRect.sizeDelta = new Vector2(350f, 180f);
                Canvas.ForceUpdateCanvases();
                Vector4 after = material.GetVector("_HoleRect");
                Assert.That(after.x, Is.Not.EqualTo(before.x));
                Assert.That(after.y, Is.Not.EqualTo(before.y));
                Assert.That(after.z, Is.GreaterThan(before.z));
                Vector2 screen = RectTransformUtility.WorldToScreenPoint(null, targetRect.position);
                Assert.That(mask.IsRaycastLocationValid(screen, null), Is.False);
            }
            finally
            {
                if (mask != null) typeof(InvertMaskingAuto).GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(mask, null);
                Time.timeScale = previousScale;
            }
        }
        [Test] public void TargetChannelRemovesDisabledTargetsWithoutGlobalSearch()
        {
            var channel = Asset<TutorialTargetChannel>();
            var step = Step(target: true);
            var handler = Handler(Definition(step));
            var target = Target(step, handler);
            Set(target, "m_Channel", channel);
            channel.Register(target);
            Assert.That(channel.Targets.Count, Is.EqualTo(1));
            target.gameObject.SetActive(false);
            typeof(TutorTarget).GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
            Assert.That(channel.Targets.Count, Is.EqualTo(0));
        }
        [Test] public void CompletionAndExistingDependencyUnlockAreSavedTogether()
        {
            var first = Definition(Step(TutorialAction.HeroSelected));
            var next = Definition(Step(TutorialAction.HeroUsed));
            Set(next, "m_UnlockedAtFirst", false);
            var so = new SerializedObject(first);
            var unlocks = so.FindProperty("m_UnlockOnCompletion");
            unlocks.arraySize = 1;
            unlocks.GetArrayElementAtIndex(0).objectReferenceValue = next;
            so.ApplyModifiedPropertiesWithoutUndo();
            var handler = Handler(first, next);
            handler.StartTutorial(first);
            Tick(handler);
            handler.NotifyAction(TutorialAction.HeroSelected, string.Empty);
            Tick(handler);
            Assert.That(Content(first).IsDone, Is.True);
            Assert.That(Content(next).IsUnlocked, Is.True);
        }
        [Test] public void EditorReenableDoesNotConnectRuntimeSingletons()
        {
            var handler = Handler(Definition(Step(TutorialAction.HeroSelected)));
            Field(handler, "m_UsesGameServices", true);
            Field(handler, "m_IsDeveloperMode", true);
            typeof(TutorialHandler).GetMethod("OnApplicationQuit", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(handler, null);
            typeof(TutorialHandler).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(handler, null);
            Assert.That(handler.IsInitialized, Is.True);
            Assert.That(typeof(TutorialHandler).GetField("m_IsDeveloperMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(handler), Is.True);
            Field(handler, "m_UsesGameServices", false);
        }

        public object RunAll()
        {
            var results = new List<object>();
            foreach (var method in GetType().GetMethods())
            {
                if (method.GetCustomAttribute<TestAttribute>() == null) continue;
                string error = null;
                SetUp();
                try { method.Invoke(this, null); }
                catch (Exception ex) { error = (ex.InnerException ?? ex).ToString(); }
                finally { TearDown(); }
                results.Add(new { test = method.Name, passed = error == null, error });
            }
            return results;
        }
    }
}
#endif
