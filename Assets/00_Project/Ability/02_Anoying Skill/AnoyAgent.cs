using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class AnoyAgent : MonoBehaviour, IAnoy
    {
        [SerializeField]
        private AnoyDefinition m_AnoyDefinition;

        [SerializeField]
        private UnityEvent<int, int> m_OnInterupUpdate;
        [SerializeField]
        private UnityEvent m_OnAnoyStop;
        public AnoyDefinition AnoyDefinition => m_AnoyDefinition;

        public void Register()
        {
            Player.Instance.AddAnoy(this);
            GameManager.Instance.AddOnPerfectTouchDown(AddInteruptOne);
        }
        public void Unregister()
        {
            GameManager.Instance.RemoveOnPerfectTouchDown(AddInteruptOne);
            Player.Instance.RemoveAnoy(this);
        }
        public void StopAnoy()
        {
            m_OnAnoyStop?.Invoke();
            GameManager.Instance.RemoveOnPerfectTouchDown(AddInteruptOne);
            Player.Instance.RemoveAnoy(this);
        }

        private void AddInteruptOne()
        {
            Player.Instance.AddInteruptAnoy(m_AnoyDefinition, 1);
        }

        public void AddInterupt(int interupt)
        {
            Player.Instance.AddInteruptAnoy(m_AnoyDefinition, interupt);
        }

        public void OnInteruptUpdateInvoke(int interuptCount)
        {
            m_OnInterupUpdate?.Invoke(interuptCount, m_AnoyDefinition.InteruptDurability);
        }
    }

    public partial class LevelHandler
    {
        public void AddOnPerfectTouchDown(UnityAction action)
        {
            m_OnPerfectTouchDown.AddListener(action);
        }
        public void RemoveOnPerfectTouchDown(UnityAction action)
        {
            m_OnPerfectTouchDown.RemoveListener(action);
        }
    }

    public partial class GameManager
    {
        public void AddOnPerfectTouchDown(UnityAction action)
        {
            m_LevelManager.AddOnPerfectTouchDown(action);
        }
        public void RemoveOnPerfectTouchDown(UnityAction action)
        {
            m_LevelManager.RemoveOnPerfectTouchDown(action);
        }
    }

}
