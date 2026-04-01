using Rush;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class HomePanel : PanelView
    {
        [SerializeField]
        private Button m_PlayButton;
        [SerializeField]
        private UnityEvent m_OnStart;
        [SerializeField]
        private UnityEvent m_OnPlayButtonClick;
        private void OnPLayButtonClick()
        {
            //GameManager.Instance.SceneController.LoadSceneConfig(m_GameplayScene);
            //RushGameManager.Instance.GameStateManager.ChangeState(GameStateConfig.Gameplay);
            m_OnPlayButtonClick?.Invoke();
        }

        private void Start()
        {
            m_OnStart.Invoke();

            m_PlayButton.onClick.RemoveListener(OnPLayButtonClick);
            m_PlayButton.onClick.AddListener(OnPLayButtonClick);
        }
    }
}
