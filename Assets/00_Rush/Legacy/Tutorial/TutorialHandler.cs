using System;
using System.Collections.Generic;
using LegionKnight.Dialogue;
using Rush;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class TutorialHandler : MonoBehaviour
    {
        private const string c_QueueKey = "tutorial.v2.queue";
        [SerializeField] private TutorialContent[] m_Contents = Array.Empty<TutorialContent>();
        [SerializeField] private List<TutorTarget> m_TutorTargets = new();
        [SerializeField] private UnityEvent<TutorialDefinition> m_OnTutorialStart = new();
        [SerializeField] private UnityEvent<TutorTarget> m_OnStepChanged = new();
        [SerializeField] private UnityEvent<TutorialDefinition> m_OnTutorialEnd = new();
        [SerializeField] private Button m_SecondNextButton;
        [SerializeField] private TutorialTargetChannel m_TargetChannel;
        [SerializeField] private TutorialRuntimeBridge m_RuntimeBridge;
        [SerializeField, Min(1f)] private float m_TargetWaitTimeout = 20f;
        private readonly List<TutorialDefinition> m_Pending = new();
        private readonly List<TutorialDefinition> m_Deferred = new();
        private ITutorialProgressStore m_Store;
        private TutorialDefinition m_CurrentTutorial;
        private TutorialContent m_CurrentContent;
        private TutorialPanel m_Panel;
        private StartGamePanel m_StartGamePanel;
        private DialoguePanel m_DialoguePanel;
        private TutorTarget m_CurrentTarget;
        private TutorTarget m_ClickConditionTarget;
        private UnityAction m_BoundClick;
        private int m_CurrentStep;
        private int m_StepVersion;
        private bool m_Initialized;
        private bool m_UsesGameServices;
        private bool m_IsQuitting;
        private bool m_IsDeveloperMode;
        private bool m_IsFinishing;
        private bool m_AdvanceRequested;
        private bool m_ClickRequested;
        private bool m_StepStarted;
        private bool m_PresentationStarted;
        private bool m_Resuming;
        private bool m_PriorDialogueRaycast;
        private bool m_OwnsDialogueRaycast;
        private bool m_OwnsDialogue;
        private float m_TargetWaitStarted;
        public event Action AvailabilityChanged;
        public TutorialDefinition CurrentTutorial => m_CurrentTutorial;
        public int CurrentStepIndex => m_CurrentStep;
        public bool IsInitialized => m_Initialized;
        public bool HasDeferredTutorial => m_Deferred.Count > 0;
        public bool IsWaitingForTarget => m_CurrentTutorial != null && m_CurrentTutorial.Steps[m_CurrentStep].RequiresTarget && m_CurrentTarget == null;

        private void OnEnable()
        {
            if (Application.isPlaying && !m_IsQuitting && m_Initialized && m_UsesGameServices) BindRuntimeInternal();
            if (m_TargetChannel != null)
            {
                m_TargetChannel.TargetAdded += AddTarget;
                m_TargetChannel.TargetRemoved += RemoveTarget;
                foreach (var target in m_TargetChannel.Targets) AddTargetInternal(target);
            }
        }
        private void OnDisable()
        {
            if (m_TargetChannel != null)
            {
                m_TargetChannel.TargetAdded -= AddTarget;
                m_TargetChannel.TargetRemoved -= RemoveTarget;
            }
            if (m_RuntimeBridge != null) m_RuntimeBridge.ActionPerformed -= NotifyAction;
            if (m_CurrentTutorial != null) DeferCurrentInternal();
            CleanupPresentationInternal();
        }
        private void OnApplicationQuit() => m_IsQuitting = true;

        public void Init()
        {
            if (m_IsQuitting)
            {
                m_IsQuitting = false;
                m_Initialized = false;
                m_Pending.Clear();
                m_Deferred.Clear();
            }
            if (m_Initialized) return;
            m_UsesGameServices = true;
            BindRuntimeInternal();
            InitializeInternal(new LocalTutorialProgressStore(UnityService.Instance.LocalSave));
        }
        private void BindRuntimeInternal()
        {
            var canvas = CanvasManager.Instance;
            m_Panel = canvas != null ? canvas.GetPanel<TutorialPanel>() : null;
            m_StartGamePanel = canvas != null ? canvas.GetPanel<StartGamePanel>() : null;
            m_DialoguePanel = canvas != null ? canvas.GetPanel<DialoguePanel>() : null;
            var player = Player.Instance;
            var rush = RushGameManager.Instance;
            var rushPlayer = RushPlayer.Instance;
            m_IsDeveloperMode = GameSetting.Instance != null && GameSetting.Instance.DeveloperMode;
            if (m_RuntimeBridge != null && canvas != null && rushPlayer != null && rush != null)
            {
                m_RuntimeBridge.Initialize(player, rushPlayer.GachaManager, rush.StageManager,
                    canvas.GetPanel<HomePanel>(), canvas.GetPanel<GameplayPanel>(), canvas.MainCanvas.GetComponentsInChildren<PanelView>(true));
                m_RuntimeBridge.ActionPerformed -= NotifyAction;
                m_RuntimeBridge.ActionPerformed += NotifyAction;
            }
            m_Panel?.Bind(this);
        }

        // Store injection also lets regression tests exercise real state transitions without a player save.
        public void Initialize(ITutorialProgressStore store) => InitializeInternal(store);
        private void InitializeInternal(ITutorialProgressStore store)
        {
            if (m_Initialized) return;
            m_Store = store ?? throw new ArgumentNullException(nameof(store));
            var ids = new HashSet<string>();
            foreach (var content in m_Contents)
            {
                if (content?.Definition == null || !content.Definition.IsEnabled) continue;
                if (!ids.Add(content.Definition.Id))
                {
                    Debug.LogError("[Tutorial] Duplicate catalog ID: " + content.Definition.Id, this);
                    continue;
                }
                content.Initialize(store);
            }
            m_Initialized = true;
            if (store.Contains(c_QueueKey))
            {
                var queue = store.Read<TutorialQueueRecord>(c_QueueKey);
                if (queue?.DeferredIds != null)
                    foreach (var id in queue.DeferredIds) RestoreRequestInternal(id, m_Deferred);
                m_Deferred.RemoveAll(d => HasCommittedFinalStep(ContentFor(d)));
                if (queue?.Ids != null)
                    foreach (var id in queue.Ids) RestoreRequestInternal(id, m_Pending);
            }
            foreach (var content in m_Contents)
                if (content?.InProgress == true && !m_Deferred.Contains(content.Definition)) EnqueueInternal(content.Definition);
            SaveQueueInternal();
            AvailabilityChanged?.Invoke();
        }

        private static bool HasCommittedFinalStep(TutorialContent content)
        {
            if (content == null || !content.InProgress) return false;
            var steps = content.Definition.Steps;
            if (steps == null || steps.Length == 0) return false;
            var last = steps[steps.Length - 1];
            return last != null && last.PreserveSuccess && content.HasCompletedStep(last.Id);
        }

        public bool HasContent(TutorialDefinition definition, out TutorialContent content)
        {
            content = ContentFor(definition);
            return content != null;
        }
        private TutorialContent ContentFor(TutorialDefinition definition)
        {
            if (definition == null) return null;
            foreach (var content in m_Contents) if (content?.Definition == definition) return content;
            return null;
        }
        public static TutorialPanel GetPanel() => CanvasManager.Instance.GetPanel<TutorialPanel>();
        public void AddTarget(TutorTarget target) => AddTargetInternal(target);
        private void AddTargetInternal(TutorTarget target)
        {
            if (target != null && !m_TutorTargets.Contains(target)) m_TutorTargets.Add(target);
        }
        public void RemoveTarget(TutorTarget target)
        {
            m_TutorTargets.Remove(target);
            if (m_CurrentTarget == target)
            {
                DetachButtonInternal();
                m_CurrentTarget = null;
                m_PresentationStarted = false;
                HideOverlayInternal();
            }
        }

        public void StartTutorial(TutorialDefinition definition)
        {
            if (definition == null || !definition.IsEnabled || m_IsDeveloperMode) return;
            if (!string.IsNullOrEmpty(definition.ValidationError))
            {
                Debug.LogWarning("[Tutorial] Cannot start " + definition.name + ": " + definition.ValidationError, this);
                return;
            }
            if (ContentFor(definition) == null)
            {
                Debug.LogWarning("[Tutorial] Definition is not registered: " + definition.name, this);
                return;
            }
            if (m_Deferred.Contains(definition)) return;
            EnqueueInternal(definition);
            if (m_Initialized) SaveQueueInternal();
            AvailabilityChanged?.Invoke();
        }
        private void EnqueueInternal(TutorialDefinition definition)
        {
            if (definition == null || !definition.IsEnabled || definition == m_CurrentTutorial || m_Pending.Contains(definition)) return;
            var content = ContentFor(definition);
            if (m_Initialized && (content == null || content.IsDone)) return;
            m_Pending.Add(definition);
        }
        private void RestoreRequestInternal(string id, List<TutorialDefinition> destination)
        {
            foreach (var content in m_Contents)
                if (content?.Definition != null && content.Definition.Id == id && !content.IsDone &&
                    string.IsNullOrEmpty(content.Definition.ValidationError) && !destination.Contains(content.Definition))
                    destination.Add(content.Definition);
        }

        private void Update()
        {
            if (!m_Initialized || m_IsDeveloperMode || m_IsFinishing) return;
            if (m_CurrentTutorial == null) { StartNextInternal(); return; }
            if (m_AdvanceRequested) { AdvanceInternal(); return; }
            if (m_ClickRequested && m_ClickConditionTarget != null && m_ClickConditionTarget.CompletionSatisfied)
            {
                m_AdvanceRequested = true;
                return;
            }
            PresentStepInternal();
        }

        private void StartNextInternal()
        {
            if (m_Pending.Count == 0) return;
            if (m_RuntimeBridge != null && !m_RuntimeBridge.CanPresent) return;
            bool removedInvalid = false;
            for (int i = 0; i < m_Pending.Count; i++)
            {
                var definition = m_Pending[i];
                var content = ContentFor(definition);
                if (content == null || content.IsDone || !string.IsNullOrEmpty(definition.ValidationError))
                { m_Pending.RemoveAt(i--); removedInvalid = true; continue; }
                if (!content.IsUnlocked) continue;
                m_Pending.RemoveAt(i);
                m_CurrentTutorial = definition;
                m_CurrentContent = content;
                m_Resuming = content.InProgress;
                // Rebuild navigation from Home, while preserving successful costly actions below.
                m_CurrentStep = HasCommittedFinalStep(content) ? definition.Steps.Length - 1 : 0;
                BeginStepInternal();
                SaveQueueInternal();
                OnTutorialStartInvoke(definition);
                content.NotifyStarted();
                AvailabilityChanged?.Invoke();
                return;
            }
            if (removedInvalid) SaveQueueInternal();
        }

        private void BeginStepInternal()
        {
            DetachButtonInternal();
            m_StepVersion++;
            m_CurrentTarget = null;
            m_AdvanceRequested = false;
            m_ClickRequested = false;
            m_ClickConditionTarget = null;
            m_StepStarted = false;
            m_PresentationStarted = false;
            m_TargetWaitStarted = Time.realtimeSinceStartup;
            var step = m_CurrentTutorial.Steps[m_CurrentStep];
            m_CurrentContent.SetCheckpoint(step.Id);
            bool priorSuccess = step.PreserveSuccess && m_CurrentContent.HasCompletedStep(step.Id);
            bool dependencyDone = step.SuccessDependency != null && m_CurrentContent.HasCompletedStep(step.SuccessDependency.Id);
            bool priorPresentation = m_Resuming && step.SkipCompletedOnResume && m_CurrentContent.HasCompletedStep(step.Id);
            bool committedBeforeRestart = m_Resuming && step.ResumeSuccessDependency != null && m_CurrentContent.HasCompletedStep(step.ResumeSuccessDependency.Id);
            if (priorSuccess || dependencyDone || priorPresentation || committedBeforeRestart) m_AdvanceRequested = true;
            HideOverlayInternal();
        }

        private void PresentStepInternal()
        {
            var step = m_CurrentTutorial.Steps[m_CurrentStep];
            if (!m_StepStarted)
            {
                m_StepStarted = true;
                if (step.Reward != null)
                {
                    if (!m_CurrentContent.LegacyCompletionUnverified)
                        m_CurrentContent.GrantRewardOnce(step.Id, step.Reward.DirectTakeLoots);
                    else
                        m_CurrentContent.GrantRewardOnce(step.Id, () => { });
                    if (m_Panel != null) CanvasManager.Instance.GetPanel<LootedPanel>()?.ShowLoot(step.Reward.LootFields);
                }
                step.NotifyStarted();
            }
            if (!step.RequiresTarget) return;
            if (m_RuntimeBridge != null && m_RuntimeBridge.IsStateSatisfied(step.CompletionAction))
            {
                NotifyAction(step.CompletionAction, string.Empty);
                return;
            }
            var target = TargetFor(step);
            if (target == null)
            {
                DetachButtonInternal();
                m_CurrentTarget = null;
                HideOverlayInternal();
                // Classic has already opened its pre-battle instructions. The stage starts when the player dismisses them.
                if (step.CompletionAction == TutorialAction.StageStarted && m_StartGamePanel != null && m_StartGamePanel.Content.activeInHierarchy)
                {
                    m_TargetWaitStarted = Time.realtimeSinceStartup;
                    return;
                }
                if (Time.realtimeSinceStartup - m_TargetWaitStarted >= m_TargetWaitTimeout)
                {
                    Debug.LogWarning("[Tutorial] Deferred step '" + step.Id + "': no available target. Continue Tutorial can retry it.", this);
                    DeferCurrentInternal();
                }
                return;
            }
            m_TargetWaitStarted = Time.realtimeSinceStartup;
            if (target == m_CurrentTarget && m_PresentationStarted) return;
            DetachButtonInternal();
            m_CurrentTarget = target;
            m_PresentationStarted = true;
            int version = m_StepVersion;
            m_SecondNextButton = target.NextButton != null ? target.NextButton : m_Panel != null ? m_Panel.NextButton : null;
            if (m_SecondNextButton != null)
            {
                m_BoundClick = () => OnButtonClickedInternal(version, target);
                m_SecondNextButton.onClick.AddListener(m_BoundClick);
            }
            m_Panel?.SetTutorial(target);
            m_Panel?.Show();
            m_Panel?.Refresh();
            target.NotifyStarted();
            OnStepChangedInvoke(target);
            if (step.Conversation != null && m_DialoguePanel != null)
            {
                GameManager.Instance.StartConversation(step.Conversation);
                m_OwnsDialogue = true;
                OwnDialogueRaycastInternal();
            }
        }

        private TutorTarget TargetFor(TutorStepDefinition step)
        {
            TutorTarget best = null;
            m_TutorTargets.RemoveAll(target => target == null);
            foreach (var target in m_TutorTargets)
            {
                if (target.Definition != step || !target.IsAvailable ||
                    (m_RuntimeBridge != null && !m_RuntimeBridge.IsTargetEligible(target))) continue;
                if (best == null || string.CompareOrdinal(target.TargetKey, best.TargetKey) < 0) best = target;
            }
            return best;
        }
        private void OnButtonClickedInternal(int version, TutorTarget target)
        {
            if (version != m_StepVersion || m_CurrentTutorial == null || m_AdvanceRequested) return;
            if (m_CurrentTutorial.Steps[m_CurrentStep].CompletionAction != TutorialAction.None) return;
            m_ClickConditionTarget = target;
            m_ClickRequested = true;
        }
        public void NotifyAction(TutorialAction action, string subjectId)
        {
            if (m_CurrentTutorial == null && action == TutorialAction.DrawCommitted)
            {
                foreach (var definition in m_Deferred)
                {
                    var deferred = ContentFor(definition);
                    if (deferred == null || !deferred.InProgress) continue;
                    bool requested = false;
                    foreach (var candidate in definition.Steps)
                        if (candidate.CompletionAction == TutorialAction.DrawRequested && deferred.HasCompletedStep(candidate.Id)) requested = true;
                    if (!requested) continue;
                    foreach (var candidate in definition.Steps)
                        if (candidate.CompletionAction == TutorialAction.DrawCommitted) deferred.AddCompletedStep(candidate.Id);
                }
                return;
            }
            if (m_CurrentTutorial == null || m_IsFinishing) return;
            var step = m_CurrentTutorial.Steps[m_CurrentStep];
            if (m_CurrentTarget?.HeroConfig != null && m_CurrentTarget.HeroConfig.BaseInfo.Id != subjectId) return;
            if (m_CurrentTarget?.CardConfig != null && m_CurrentTarget.CardConfig.BaseInfo.Id != subjectId) return;
            if (action == TutorialAction.DrawCommitted)
            {
                // A skipped confirmation can finish synchronously before its next step is presented.
                foreach (var candidate in m_CurrentTutorial.Steps)
                    if (candidate.CompletionAction == TutorialAction.DrawCommitted)
                        m_CurrentContent.AddCompletedStep(candidate.Id);
            }
            if (step.CompletionAction != action && !(step.CompletionAction == TutorialAction.DrawRequested && action == TutorialAction.DrawCommitted)) return;
            m_CurrentContent.AddCompletedStep(step.Id);
            m_AdvanceRequested = true;
        }

        private void AdvanceInternal()
        {
            var definition = m_CurrentTutorial;
            var content = m_CurrentContent;
            var step = definition.Steps[m_CurrentStep];
            DetachButtonInternal();
            m_AdvanceRequested = false;
            content.AddCompletedStep(step.Id);
            m_CurrentTarget?.NotifyEnded();
            step.NotifyEnded();
            if (m_CurrentTutorial != definition) return;
            if (m_CurrentStep + 1 < definition.Steps.Length)
            {
                m_CurrentStep++;
                BeginStepInternal();
                return;
            }
            m_IsFinishing = true;
            bool committed = false;
            try
            {
                m_Store.Transact(() =>
                {
                    content.SetIsDone(true);
                    foreach (var next in definition.UnlockOnCompletion)
                        ContentFor(next)?.SetIsUnlocked(true);
                    m_CurrentTutorial = null;
                    m_CurrentContent = null;
                    SaveQueueInternal();
                });
                committed = true;
                CleanupPresentationInternal();
                OnTutorialEndInvoke(definition);
                content.NotifyEnded();
            }
            catch
            {
                // The outer transaction can fail after the content's nested write has returned.
                if (!committed)
                {
                    content.Initialize(m_Store);
                    foreach (var next in definition.UnlockOnCompletion) ContentFor(next)?.Initialize(m_Store);
                    m_CurrentTutorial = definition;
                    m_CurrentContent = content;
                }
                throw;
            }
            finally { m_IsFinishing = false; AvailabilityChanged?.Invoke(); }
        }

        public void PauseTutorial() => DeferCurrentInternal();
        private void DeferCurrentInternal()
        {
            if (m_CurrentTutorial == null) return;
            if (!m_Deferred.Contains(m_CurrentTutorial)) m_Deferred.Add(m_CurrentTutorial);
            m_CurrentTutorial = null;
            m_CurrentContent = null;
            m_StepVersion++;
            CleanupPresentationInternal();
            SaveQueueInternal();
            AvailabilityChanged?.Invoke();
        }
        public void ResumeTutorial()
        {
            foreach (var definition in m_Deferred) EnqueueInternal(definition);
            m_Deferred.Clear();
            SaveQueueInternal();
            AvailabilityChanged?.Invoke();
        }
        private void SaveQueueInternal()
        {
            if (!m_Initialized || m_Store == null) return;
            var ids = new List<string>();
            if (m_CurrentTutorial != null) ids.Add(m_CurrentTutorial.Id);
            foreach (var definition in m_Pending) ids.Add(definition.Id);
            var deferred = new List<string>();
            foreach (var definition in m_Deferred) deferred.Add(definition.Id);
            m_Store.Write(c_QueueKey, new TutorialQueueRecord(ids, deferred));
        }
        private void DetachButtonInternal()
        {
            if (m_SecondNextButton != null && m_BoundClick != null) m_SecondNextButton.onClick.RemoveListener(m_BoundClick);
            m_SecondNextButton = null;
            m_BoundClick = null;
        }
        private void OwnDialogueRaycastInternal()
        {
            var block = m_DialoguePanel != null ? m_DialoguePanel.RaycastBlockImage : null;
            if (block == null) return;
            if (!m_OwnsDialogueRaycast) m_PriorDialogueRaycast = block.raycastTarget;
            m_OwnsDialogueRaycast = true;
            block.raycastTarget = false;
        }
        private void HideOverlayInternal()
        {
            m_Panel?.Hide();
            if (m_OwnsDialogue) m_DialoguePanel?.Hide();
            m_OwnsDialogue = false;
            if (m_OwnsDialogueRaycast && m_DialoguePanel != null && m_DialoguePanel.RaycastBlockImage != null)
                m_DialoguePanel.RaycastBlockImage.raycastTarget = m_PriorDialogueRaycast;
            m_OwnsDialogueRaycast = false;
        }
        private void CleanupPresentationInternal()
        {
            DetachButtonInternal();
            HideOverlayInternal();
            m_CurrentTarget = null;
            m_PresentationStarted = false;
        }
        private void OnTutorialStartInvoke(TutorialDefinition definition) => m_OnTutorialStart?.Invoke(definition);
        private void OnStepChangedInvoke(TutorTarget target) => m_OnStepChanged?.Invoke(target);
        private void OnTutorialEndInvoke(TutorialDefinition definition) => m_OnTutorialEnd?.Invoke(definition);
    }
}
