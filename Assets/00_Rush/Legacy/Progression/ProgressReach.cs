using Rush;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class ProgressReach : MonoBehaviour
    {
        [SerializeField] private int m_LevelTarget = 1;
        [SerializeField] private UnityEvent<bool> m_OnLevelReached = new();
        [SerializeField] private UnityEvent m_OnLevelReachedTrue = new();
        [SerializeField] private UnityEvent m_OnLevelReachedFalse = new();
        [SerializeField] private TutorialAccessPolicy m_AccessPolicy;
        [SerializeField] private Selectable m_Control;
        private Player m_Player;
        private TutorialManager m_Tutorials;
        public bool IsReached => m_Player != null && (m_AccessPolicy != null
            ? m_AccessPolicy.IsAllowed(m_Player.GetPlayerLevel(), DeveloperMode, m_Tutorials)
            : TutorialAccessPolicy.LevelIsAllowed(m_Player.GetPlayerLevel(), m_LevelTarget, DeveloperMode, true));
        private bool DeveloperMode => GameSetting.Instance != null && GameSetting.Instance.DeveloperMode;
        private void OnEnable()
        {
            m_Player = Player.Instance;
            if (m_Player == null) return;
            m_Player.AddOnStart(Init);
            m_Player.AddOnLevelUp(OnLevelUp);
            if (m_AccessPolicy != null && m_AccessPolicy.RequiredTutorial != null)
            {
                m_Tutorials = TutorialAgent.GetManager();
                if (m_Tutorials != null) m_Tutorials.AvailabilityChanged += Init;
            }
            RefreshInternal();
        }
        private void OnDisable()
        {
            if (m_Player != null)
            {
                m_Player.RemoveOnStart(Init);
                m_Player.RemoveOnLevelUp(OnLevelUp);
            }
            if (m_Tutorials != null) m_Tutorials.AvailabilityChanged -= Init;
            m_Tutorials = null;
            m_Player = null;
        }
        public void Init() => RefreshInternal();
        private void OnLevelUp(int level) => RefreshInternal();
        private void RefreshInternal()
        {
            bool allowed = IsReached;
            if (m_Control != null) m_Control.interactable = allowed;
            OnLevelReachedInvoke(allowed);
            if (allowed) OnLevelReachedTrueInvoke();
            else OnLevelReachedFalseInvoke();
        }
        private void OnLevelReachedInvoke(bool value) => m_OnLevelReached?.Invoke(value);
        private void OnLevelReachedTrueInvoke() => m_OnLevelReachedTrue?.Invoke();
        private void OnLevelReachedFalseInvoke() => m_OnLevelReachedFalse?.Invoke();
    }
}
