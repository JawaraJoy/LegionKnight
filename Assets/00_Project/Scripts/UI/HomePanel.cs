using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public static partial class PanelId
    {
        public static string HomePanelId = "Home";
    }
    public partial class HomePanel : PanelView
    {
        [SerializeField]
        private string m_GameplaySceneName;
        public override string UniqueId => PanelId.HomePanelId;

        [SerializeField]
        private UnityEvent m_OnStart;
        public void LoadGameplayScene()
        {
            GameManager.Instance.LoadScene(m_GameplaySceneName);
        }

        private void Start()
        {
            m_OnStart.Invoke();
        }
    }
}
