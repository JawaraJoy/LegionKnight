using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class FailTab : UIView
    {
        [SerializeField]
        private Button m_QuitButton;

        private void Awake()
        {
            m_QuitButton.onClick.AddListener(Quit);
        }

        private void Quit()
        {
            Application.Quit();
            HideInternal();
        }
    }
}
