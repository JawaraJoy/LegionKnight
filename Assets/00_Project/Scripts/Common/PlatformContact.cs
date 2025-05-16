using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class PlatformContact : Contact2D
    {
        [SerializeField]
        private Platform m_Platform;

        [SerializeField]
        private UnityEvent<int> m_OnNormalTouchDown = new();
        [SerializeField]
        private UnityEvent<int> m_OnPerfectTouchDown = new();
        [SerializeField]
        private UnityEvent<int> m_OnStayPerfectCombo = new();
        [SerializeField]
        private UnityEvent<bool> m_OnStayPerfect = new();
        private bool m_Touched = false;

        private void Start()
        {
            m_Touched = false;
        }
        public bool Touched => m_Touched;
        public void SetTouched(bool set)
        {
            m_Touched = set;
        }
        public void OnNormalTouchDownInvoke(int reward)
        {
            m_OnNormalTouchDown?.Invoke(reward);
            m_Touched = true;
        }
        public void OnPerfectTouchDownInvoke(int reward)
        {
            m_OnPerfectTouchDown?.Invoke(reward);
            m_Touched = true;
        }

        public void OnStayPerfectComboInvoke(int amount)
        {
            m_OnStayPerfectCombo?.Invoke(amount);
        }
        public void OnStayPerfectInvoke(bool set)
        {
            m_OnStayPerfect?.Invoke(set);
        }

        public Currency GetNormalTouchDown()
        {
            return m_Platform.GetNormalTouchDown();
        }
        public Currency GetPerfectTouchdown()
        {
            return m_Platform.GetPerfectTouchdown();
        }
        public void SetCanMove(bool set)
        {
            m_Platform.SetCanMove(set);
        }
        public void ClearActionOnPlatformStop()
        {
            m_Platform.ClearActionOnPlatformStop();
        }
        public void SetActiveBehaviourCollider(bool set)
        {
            m_Platform.SetActiveBehaviourCollider(set);
        }
        public bool IsReached()
        {
            return m_Platform.IsReached();
        }
    }
}
