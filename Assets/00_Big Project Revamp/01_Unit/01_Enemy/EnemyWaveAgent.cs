using UnityEngine;

namespace Rush
{
    public class EnemyWaveAgent : MonoBehaviour
    {
        private EnemyWaveHandler m_EnemyWaveHandler;

        private EnemyWaveHandler EnemyWaveHandler
        {
            get
            {
                if (m_EnemyWaveHandler == null)
                {
                    m_EnemyWaveHandler = RushGameManager.Instance.StageManager.EnemyWaveHandler;
                }
                return m_EnemyWaveHandler;
            }
        }

        public void DespawnUnit(Unit unit)
        {
            EnemyWaveHandler.DespawnUnit(unit);
        }
        public void StopPostToFollow()
        {
            if (EnemyWaveHandler.EnemyWavePost != null)
            {
                if (EnemyWaveHandler.TryGetComponent(out FollowPlayer followPlayer))
                {
                    followPlayer.SetPostToFollow(null);
                }
            }
        }
        public void StartPostToFollow()
        {
            if (EnemyWaveHandler.EnemyWavePost != null)
            {
                if (EnemyWaveHandler.TryGetComponent(out FollowPlayer followPlayer))
                {
                    followPlayer.SetPostToFollow(RushPlayer.Instance.transform);
                }
            }
        }
    }
}
