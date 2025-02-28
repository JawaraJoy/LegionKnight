using UnityEngine;

namespace LegionKnight
{
    public partial class PlatformContact : Contact2D
    {
        [SerializeField]
        private Platform m_Platform;
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
