using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class PlayerCutScene : MonoBehaviour
    {
        [SerializeField]
        private bool m_HasBeenWatchFirstCutScene = false;

        [SerializeField]
        private UnityEvent m_OnAlreadyWatchFirstCutScene;
        [SerializeField]
        private UnityEvent m_OnNeverWatchFirstCutScene;

        public void Init()
        {
            bool hasBeenWatchFirstCutScene = UnityService.Instance.HasData(nameof(m_HasBeenWatchFirstCutScene));
            if (hasBeenWatchFirstCutScene)
            {
                m_HasBeenWatchFirstCutScene = UnityService.Instance.GetData<bool>(nameof(m_HasBeenWatchFirstCutScene));
            }
        }
        public void CheckIfalredyWatchFirstCutScene()
        {
            if (m_HasBeenWatchFirstCutScene)
            {
                m_OnAlreadyWatchFirstCutScene.Invoke();
            }
            else
            {
                m_OnNeverWatchFirstCutScene?.Invoke();
            }
        }

        public void SetHasBeenWatchFirstCutScene(bool value)
        {
            m_HasBeenWatchFirstCutScene = value;
            //UnityService.Instance.SaveData(nameof(m_HasBeenWatchFirstCutScene), m_HasBeenWatchFirstCutScene);
        }
    }
}
