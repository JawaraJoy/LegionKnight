
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class GameStateHandler : MonoBehaviour
    {
        private GameStateField m_CurrentState;
        [SerializeField]
        private GameStateField[] m_StateFields;

        [SerializeField]
        private UnityEvent<GameStateField> m_OnStateEnter;
        [SerializeField]
        private UnityEvent<GameStateField> m_OnStateExit;

        private GameStateField GetGameState(GameStateConfig gameState)
        {
            GameStateField found = m_StateFields.FirstOrDefault(x => x.State.BaseInfo.Id == gameState.BaseInfo.Id);
            if (found == null)
            {
                return null;
            }
            return found;
        }

        private bool HasGameState(GameStateConfig gameState, out GameStateField stateField)
        {
            stateField = GetGameState(gameState);
            return stateField != null;
        }

        private void OnStateEnterInvoke(GameStateField state)
        {
            if (state == null) return;
            m_OnStateEnter?.Invoke(state);
        }
        private void OnStateExitInvoke(GameStateField state)
        {
            if (state == null) return;
            m_OnStateExit?.Invoke(state);
        }
        private void ChangeStateInternal(GameStateConfig state)
        {
            if (HasGameState(state, out GameStateField stateField))
            {
                if (m_CurrentState == null)
                {
                    OnStateExitInvoke(m_CurrentState);
                    m_CurrentState?.OnStateExit.Invoke();
                }
                m_CurrentState = stateField;
                OnStateEnterInvoke(m_CurrentState);
                m_CurrentState?.OnStateEnter.Invoke();
            }
        }
        public void ChangeState(GameStateConfig state)
        {
            ChangeStateInternal(state);
        }
    }
}
