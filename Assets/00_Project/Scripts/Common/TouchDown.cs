using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class TouchDown : Contact2D
    {
        [SerializeField]
        private float m_MinDistanceToPerfect = 0.1f;

        private bool m_StayPerfect;
        private int m_StayPerfectCombo;

        [SerializeField]
        private UnityEvent<int> m_OnNormalTouchDown = new();
        [SerializeField]
        private UnityEvent<int> m_OnPerfectTouchDown = new();
        [SerializeField]
        private UnityEvent<int> m_OnStayPerfectCombo = new();
        [SerializeField]
        private UnityEvent<bool> m_OnStayPerfect = new();
        protected override void OnContactedBehaviourInvoke(IContactable other)
        {
            base.OnContactedBehaviourInvoke(other);
            if (other.GetSelf().TryGetComponent(out PlatformContact platform))
            {
                //Player.Instance.AddCurrencyAmount(platform.GetNormalTouchDown().CurrencyDefinition, platform.GetNormalTouchDown().Amount);
                GameManager.Instance.SetCurrentTouchDownPost(transform.position);
                GameManager.Instance.SpawnPlatform();
                //GameManager.Instance.Up();

                 // sementara normal, nanti bisa ditentukan apapkah perfect touch down atau normal

                platform.SetCanMove(false);
                platform.gameObject.SetActive(false);
                platform.SetActiveBehaviourCollider(false);

                TouchDownPoint(platform.transform.position);
            }
        }

        private void TouchDownPoint(Vector2 platformPost)
        {
            float distance = Vector2.Distance(transform.position, platformPost);

            if (distance > m_MinDistanceToPerfect)
            {
                OnNormalTouchDownInvoke();
            }
            else
            {
                OnPerfectTouchDownInvoke();
            }
            SetStayPerfect(distance > m_MinDistanceToPerfect);
        }
        private void SetStayPerfect(bool set)
        {
            m_StayPerfect = set;
            OnStayPerfectInvoke(m_StayPerfect);
        }
        private void SetStayPerfectCombo(int set)
        {
            m_StayPerfectCombo = set;
        }
        private void AddStayPerfectCombo(int add)
        {
            m_StayPerfectCombo += add;
            OnStayPerfectComboInvoke(m_StayPerfectCombo);
        }
        private void OnNormalTouchDownInvoke()
        {
            m_OnNormalTouchDown?.Invoke(GameManager.Instance.GetNormalTouchDownPoint());
            GameManager.Instance.ApplyNormalReward();
            SetStayPerfectCombo(0);
        }
        private void OnPerfectTouchDownInvoke()
        {
            m_OnPerfectTouchDown?.Invoke(GameManager.Instance.GetPerfectTouchDownPoint());
            GameManager.Instance.ApplyPerfectReward();
            AddStayPerfectCombo(1);
        }

        private void OnStayPerfectComboInvoke(int amount)
        {
            m_OnStayPerfectCombo?.Invoke(amount);
        }
        private void OnStayPerfectInvoke(bool set)
        {
            m_OnStayPerfect?.Invoke(set);
        }
    }
}
