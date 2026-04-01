using UnityEngine;

namespace Rush
{
    public class GameStateManagerAgent : MonoBehaviour
    {
        private GameStateManager m_Manager;

        private GameStateManager Manager
        {
            get
            {
                if (m_Manager == null)
                {
                    m_Manager = RushGameManager.Instance.GameStateManager;
                }
                return m_Manager;
            }
        }
        public void ChangeState(GameStateConfig state)
        {
            Manager.ChangeState(state);
        }
    }
}
