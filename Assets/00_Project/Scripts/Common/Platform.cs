using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Platform : MonoBehaviour
    {
        
        [SerializeField]
        private List<Collider2D> m_ColliderBehaviourActives = new();
        [SerializeField]
        private bool m_CanMove;
        private float m_Speed;
        private Vector2 m_Destination;

        [SerializeField]
        private UnityEvent<GameObject> m_OnPlayerAttached = new();
        [SerializeField]
        private UnityEvent m_OnPlatformStop = new();
        [SerializeField]
        private UnityEvent m_OnPlatformStart = new();
        [SerializeField]
        private UnityEvent m_OnPlatformReachDestination = new();

        private bool m_ReachTriggered;
        private LevelDefinition m_LevelDefinition;

        private void Update()
        {
            MoveToDestination();
        }

        private void MoveToDestination()
        {
            if (!m_CanMove || IsReached()) return;
            //transform.Translate(m_Speed * Time.deltaTime * Vector3.right, Space.Self);
            transform.position = Vector2.MoveTowards(transform.position, m_Destination, m_Speed * Time.deltaTime);
            ReachDestination();
        }
        public Currency GetNormalTouchDown()
        {
            return m_LevelDefinition.GetNormalTouchDown();
        }
        public Currency GetPerfectTouchdown()
        {
            return m_LevelDefinition.GetPerfectTouchdown();
        }
        private void ReachDestination()
        {
            if (IsReached() && !m_ReachTriggered)
            {
                OnPlatformReachDestination();
                m_ReachTriggered = true;
            }
        }
        private bool IsReached()
        {
            return Distance() <= 0;
        }
        private float Distance()
        {
            return Vector2.Distance(transform.position, m_Destination);
        }
        public void SetCanMove(bool set)
        {
            SetCanMoveInternal(set);
        }
        private void SetCanMoveInternal(bool set)
        {
            m_CanMove = set;
            if (m_CanMove)
            {
                OnPlatformStartMoveInvoke();
            }
            else
            {
                OnPlatformStopMoveInvoke();
            }
        }
        public void AttachPlayer(GameObject player)
        {
            OnPlayerAttachedInvoke(player);
        }
        private void OnPlayerAttachedInvoke(GameObject player)
        {
            m_OnPlayerAttached?.Invoke(player);
            SetCanMoveInternal(false);
        }

        private void OnPlatformStartMoveInvoke()
        {
            m_OnPlatformStart?.Invoke();
            Debug.Log("Start Platform");
            
        }
        private void OnPlatformStopMoveInvoke()
        {
            m_OnPlatformStop?.Invoke();
            Debug.Log("Stop Platform");
        }
        private void OnPlatformReachDestination()
        {
            m_OnPlatformReachDestination?.Invoke();
        }
        public void StopMove()
        {
            m_CanMove = false;
        }

        public void SetDestination(Vector2 set)
        {
            m_Destination = set;
        }

        public void SetStartPosition(Transform set)
        {
            transform.position = set.position;
        }

        public void SetSpeed(float set)
        {
            m_Speed = set;
        }
        public void SetLevelDefnition(LevelDefinition set)
        {
            m_LevelDefinition = set;
        }
        public void AddOnPlatformStop(UnityAction action)
        {
            //m_OnPlatformStop.RemoveAllListeners();
            m_OnPlatformStop?.AddListener(action);
        }
        public void AddOnPlatformReachDestination(UnityAction action)
        {
            //m_OnPlatformStop.RemoveAllListeners();
            m_OnPlatformReachDestination?.AddListener(action);
        }

        public void ClearActionOnPlatformStop()
        {
            m_OnPlatformStop?.RemoveAllListeners();
        }

        public void SetActiveBehaviourCollider(bool set)
        {
            foreach(Collider2D collider in m_ColliderBehaviourActives)
            {
                collider.enabled = set;
            }
        }
    }
}
