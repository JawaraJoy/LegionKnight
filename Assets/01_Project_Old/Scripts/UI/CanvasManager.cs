using UnityEngine;

namespace LegionKnight
{
    public partial class CanvasManager : Singleton<CanvasManager>
    {
        [SerializeField]
        private MainCanvasView m_MainCanvas;
        public MainCanvasView MainCanvas => m_MainCanvas;

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
            return GetPanelInternal(uniqueId).IsShown;
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
    }
}
