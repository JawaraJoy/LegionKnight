using Rush;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class TutorTarget : MonoBehaviour
    {
        [SerializeField] private TutorStepDefinition m_Definition;
        [SerializeField] private UnityEvent<TutorTarget> m_OStepStart = new();
        [SerializeField] private UnityEvent<TutorTarget> m_OnStepEnd = new();
        [SerializeField] private Button m_NextButton;
        [SerializeField] private TutorialTargetChannel m_Channel;
        [SerializeField] private HeroSelectView m_HeroSource;
        [SerializeField] private CardSelectView m_CardSource;
        [SerializeField] private View m_CompletionView;
        [SerializeField] private bool m_CompletionViewVisible = true;
        public TutorStepDefinition Definition => m_Definition;
        public Button NextButton => m_NextButton;
        public HeroUnitConfig HeroConfig => m_HeroSource != null ? m_HeroSource.HeroConfig : null;
        public CardConfig CardConfig => m_CardSource != null ? m_CardSource.CardConfig : null;
        public string TargetKey => HeroConfig != null ? HeroConfig.BaseInfo.Id : CardConfig != null ? CardConfig.BaseInfo.Id : m_Definition != null ? m_Definition.Id : string.Empty;
        public bool IsAvailable => isActiveAndEnabled && (m_HeroSource == null || HeroConfig != null) &&
            (m_CardSource == null || CardConfig != null) &&
            (m_NextButton == null || (m_NextButton.gameObject.activeInHierarchy && m_NextButton.IsInteractable()));
        public bool HasCompletionCondition => m_CompletionView != null;
        public bool CompletionSatisfied => m_CompletionView == null || m_CompletionView.IsShown == m_CompletionViewVisible;
        private void OnEnable() { if (Application.isPlaying) m_Channel?.Register(this); }
        private void OnDisable() => m_Channel?.Unregister(this);
        private void OnDestroy() => m_Channel?.Unregister(this);
        public void NotifyStarted() => OnStepStartInvoke();
        public void NotifyEnded() => OnStepEndInvoke();
        private void OnStepStartInvoke() => m_OStepStart?.Invoke(this);
        private void OnStepEndInvoke() => m_OnStepEnd?.Invoke(this);
    }
}
