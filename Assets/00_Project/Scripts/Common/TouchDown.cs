using Rush;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class PerfectTouchDownEvent
    {
        [SerializeField]
        private string m_Message;
        [SerializeField]
        private int m_PerfectComboReach;
        [SerializeField]
        private UnityEvent<string> m_OnReachTrigger = new(); 

        public void OnReachInvoke(int combo)
        {
            if (combo == m_PerfectComboReach)
            {
                m_OnReachTrigger?.Invoke(m_Message);
            }
        }
    }
    public partial class TouchDown : Contact2D
    {
        [SerializeField]
        private float m_MinDistanceToPerfect = 0.1f;
        private bool m_StayPerfect;
        private int m_StayPerfectCombo;

        [SerializeField]
        private List<StanbyPlatform> m_CharacterPlatform = new();

        [SerializeField]
        private List<PerfectTouchDownEvent> m_PerfectTouchDownEvents = new();

        [SerializeField]
        private UnityEvent<int> m_OnNormalTouchDown = new();
        [SerializeField]
        private UnityEvent<int> m_OnPerfectTouchDown = new();
        [SerializeField]
        private UnityEvent<int> m_OnStayPerfectCombo = new();
        [SerializeField]
        private UnityEvent<bool> m_OnStayPerfect = new();

        private PlatformContact m_PlatformContact;

        public void AddCharacterPlatform()
        {
            GameManager.Instance.AddStandbyPlatform(m_CharacterPlatform);
        }
        private void OnReachPerfectComboInvoke(int combo)
        {
            foreach(PerfectTouchDownEvent perfectTouch in m_PerfectTouchDownEvents)
            {
                perfectTouch.OnReachInvoke(combo);
            }
        }
        protected override void OnContactedBehaviourInvoke(IContactable other)
        {
            base.OnContactedBehaviourInvoke(other);
            if (other.GetSelf().TryGetComponent(out PlatformContact platform))
            {
                m_PlatformContact = platform;
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
            SetStayPerfectCombo(0);
            int reward = GameManager.Instance.GetNormalTouchDownPoint();
            //GameManager.Instance.AddCurrencyRewardAmount(reward);
            
            m_OnNormalTouchDown?.Invoke(reward);
            m_PlatformContact.OnNormalTouchDownInvoke(reward);
        }
        private void OnPerfectTouchDownInvoke()
        {
            AddStayPerfectCombo(1);
            int reward = m_StayPerfectCombo * GameManager.Instance.GetPerfectTouchDownPoint();
            //GameManager.Instance.AddCurrencyRewardAmount(reward);
            
            m_OnPerfectTouchDown?.Invoke(reward);
            m_PlatformContact?.OnPerfectTouchDownInvoke(reward);

        }

        private void OnStayPerfectComboInvoke(int amount)
        {
            m_OnStayPerfectCombo?.Invoke(amount);
            OnReachPerfectComboInvoke(amount);
            m_PlatformContact?.OnStayPerfectComboInvoke(amount);
        }
        private void OnStayPerfectInvoke(bool set)
        {
            m_OnStayPerfect?.Invoke(set);
            m_PlatformContact?.OnStayPerfectInvoke(set);
        }
    }

    public partial class Player
    {
        [SerializeField]
        private TouchDown m_PlayerTouchDown;

        public void AddCharacterPlatform()
        {
            m_PlayerTouchDown.AddCharacterPlatform();
        }
    }
}
