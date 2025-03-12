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
        public void OnNormalTouchDownInvoke(int reward)
        {
            m_OnNormalTouchDown?.Invoke(reward);
        }
        public void OnPerfectTouchDownInvoke(int reward)
        {
            m_OnPerfectTouchDown?.Invoke(reward);
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
    }
}
