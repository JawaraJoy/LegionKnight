using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [System.Serializable]
    public class GameStateField
    {
        [SerializeField]
        private GameStateConfig m_State;
        [SerializeField]
        private UnityEvent m_OnStateEnter;
        [SerializeField]
        private UnityEvent m_OnStateExit;

        public GameStateConfig State => m_State;
        public UnityEvent OnStateEnter => m_OnStateEnter;
        public UnityEvent OnStateExit => m_OnStateExit;
    }
}
