using UnityEngine;

namespace LegionKnight
{
    public class PlayerCutSceneManagerAgent : MonoBehaviour
    {
        private PlayerCutSceneManager m_Manager;
        private PlayerCutSceneManager GetManager()
        {
            if (m_Manager == null)
            {
                m_Manager = Player.Instance.PlayerCutSceneManager;
            }
            return m_Manager;
        }

        public void Init()
        {
            GetManager().Init();
        }
        public void CheckIfalredyWatchFirstCutScene()
        {
            GetManager().CheckIfalredyWatchFirstCutScene();
        }

        public void SetHasBeenWatchFirstCutScene(bool value)
        {
            GetManager().SetHasBeenWatchFirstCutScene(value);
        }
    }
}
