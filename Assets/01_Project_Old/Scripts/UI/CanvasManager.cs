using UnityEngine;

namespace LegionKnight
{
    public partial class CanvasManager : Singleton<CanvasManager>
    {
        [SerializeField]
        private MainCanvasView m_MainCanvas;
        [SerializeField]
        private ButtonJumpCanvas m_JumpCanvas;
        public MainCanvasView MainCanvas => m_MainCanvas;
        public ButtonJumpCanvas JumpCanvas => m_JumpCanvas;

        protected T GetPanelInternal<T>() where T : PanelView
        {
            return m_MainCanvas.GetPanel<T>();
        }
        public T GetPanel<T>() where T : PanelView
        {
            return m_MainCanvas.GetPanel<T>();
        }
        public PanelView GetPanel(string uniqueId)
        {
            return GetPanelInternal(uniqueId);
        }
        protected PanelView GetPanelInternal(string uniqueId)
        {
            return m_MainCanvas.GetPanel(uniqueId);
        }
        public bool IsShowPanel(string uniqueId)
        {
            return GetPanelInternal(uniqueId).IsShow;
        }
        public void ShowMainCanvas()
        {
            m_MainCanvas.Show();
        }
        public void HideMainCanvas()
        {
            m_MainCanvas.Hide();
        }

        protected void ShowPanelInternal(string uniqueId)
        {
            m_MainCanvas.ShowPanel(uniqueId);
        }
        protected void HidePanelInternal(string uniqueId)
        {
            m_MainCanvas.HidePanel(uniqueId);
        }
        public virtual void ShowPanel(string uniqueId)
        {
            ShowPanelInternal(uniqueId);
        }
        public virtual void HidePanel(string uniqueId)
        {
            HidePanelInternal(uniqueId);
        }

        public void ShowJumpCanvas()
        {
            m_JumpCanvas.Show();
        }
        public void HideJumpCanvas()
        {
            m_JumpCanvas.Hide();
        }
    }
}
